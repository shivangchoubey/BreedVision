using System.Diagnostics;
using System.Text.Json;
using BreedVision.Web.Models.ViewModels;
using BreedVision.Web.Services.Interfaces;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BreedVision.Web.Services.Implementations
{
    /// <summary>
    /// ONNX inference service for the 41-breed Indian bovine classification model.
    /// Uses ujjwal75/indian-bovine-breeds-model (ResNet-50, 41-class softmax).
    /// Input: [1, 3, 224, 224] ImageNet-normalized RGB tensor.
    /// Output: [1, 41] logits over 41 ICAR/Indian bovine breed classes.
    /// </summary>
    public class OnnxBreedClassifierService : IBreedClassifierService, IDisposable
    {
        private readonly ILogger<OnnxBreedClassifierService> _logger;
        private readonly IWebHostEnvironment _env;
        private InferenceSession? _session;
        private List<string>? _classes;         // class index → breed name
        private readonly object _initLock = new();
        private bool _isInitialized = false;

        // ImageNet normalization constants (same for both models)
        private readonly float[] _mean = [0.485f, 0.456f, 0.406f];
        private readonly float[] _std  = [0.229f, 0.224f, 0.225f];

        // Buffalo breeds in the 41-class list (rest are cattle)
        private static readonly HashSet<string> BuffaloBreedSet = new(StringComparer.OrdinalIgnoreCase)
        {
            "Bhadawari", "Jaffrabadi", "Mehsana", "Murrah",
            "Nagpuri", "Nili Ravi", "Nili_Ravi", "Surti", "Toda"
        };

        public IReadOnlyList<string> SupportedBreeds => _classes ?? [];
        public double LowConfidenceThreshold => 15.0;    // softmax % below which we flag OOD
        public float SimilarityRejectionThreshold => 0.15f;

        public OnnxBreedClassifierService(
            ILogger<OnnxBreedClassifierService> logger,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
            Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized) return;
            lock (_initLock)
            {
                if (_isInitialized) return;
                try
                {
                    var assetsDir = Path.Combine(_env.ContentRootPath, "ML", "Assets");
                    var modelPath = Path.Combine(assetsDir, "model_41breed.onnx");
                    var classesPath = Path.Combine(assetsDir, "classes.json");

                    if (!File.Exists(modelPath))
                        throw new FileNotFoundException($"41-breed ONNX model not found at: {modelPath}. Run export_to_onnx.py first.");

                    if (!File.Exists(classesPath))
                        throw new FileNotFoundException($"classes.json not found at: {classesPath}.");

                    _logger.LogInformation("Loading 41-breed ONNX model from {Path}...", modelPath);
                    var opts = new Microsoft.ML.OnnxRuntime.SessionOptions
                    {
                        GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL,
                        ExecutionMode = ExecutionMode.ORT_SEQUENTIAL
                    };
                    _session = new InferenceSession(modelPath, opts);

                    var json = File.ReadAllText(classesPath);
                    _classes = JsonSerializer.Deserialize<List<string>>(json)
                        ?? throw new InvalidDataException("classes.json is empty or malformed.");

                    _isInitialized = true;
                    _logger.LogInformation(
                        "BreedVision 41-Breed Classifier ready: {Count} classes, model input {Shape}.",
                        _classes.Count,
                        string.Join("×", _session.InputMetadata.Values.First().Dimensions));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize 41-breed ONNX Classifier.");
                    throw;
                }
            }
        }

        public async Task<BreedClassificationResult> ClassifyImageAsync(Stream imageStream, string fileName)
        {
            if (!_isInitialized) Initialize();

            var sw = Stopwatch.StartNew();

            // 1. Preprocess: resize shortest edge to 256, center-crop to 224×224
            using var image = await Image.LoadAsync<Rgb24>(imageStream);
            image.Mutate(ctx => ctx
                .Resize(new ResizeOptions { Size = new Size(256, 256), Mode = ResizeMode.Crop })
                .Crop(new Rectangle(16, 16, 224, 224)));

            // 2. Build float tensor [1, 3, 224, 224] with ImageNet normalization
            var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
            image.ProcessPixelRows(acc =>
            {
                for (int y = 0; y < acc.Height; y++)
                {
                    var row = acc.GetRowSpan(y);
                    for (int x = 0; x < acc.Width; x++)
                    {
                        var p = row[x];
                        tensor[0, 0, y, x] = ((p.R / 255f) - _mean[0]) / _std[0];
                        tensor[0, 1, y, x] = ((p.G / 255f) - _mean[1]) / _std[1];
                        tensor[0, 2, y, x] = ((p.B / 255f) - _mean[2]) / _std[2];
                    }
                }
            });

            // 3. Run ONNX inference → logits [1, 41]
            var inputName = _session!.InputMetadata.Keys.First();
            using var outputs = _session.Run(new[] { NamedOnnxValue.CreateFromTensor(inputName, tensor) });
            var logits = outputs.First().AsEnumerable<float>().ToArray();

            // 4. Softmax → probabilities
            var probs = Softmax(logits);

            // 5. Rank all 41 classes
            var ranked = probs
                .Select((p, i) => (Prob: p, Index: i, Name: NormalizeClassName(_classes![i])))
                .OrderByDescending(x => x.Prob)
                .Select((x, rank) => new ClassPrediction
                {
                    BreedName       = x.Name,
                    ConfidencePercent = Math.Round(x.Prob * 100.0, 1),
                    Rank            = rank + 1,
                    Species         = BuffaloBreedSet.Contains(x.Name) ? "Buffalo" : "Cattle"
                })
                .ToList();

            sw.Stop();

            var top = ranked[0];
            bool isLowConf = top.ConfidencePercent < LowConfidenceThreshold;

            _logger.LogInformation(
                "Classified {File} in {Ms}ms → {Breed} ({Conf:F1}%, LowConf:{Low})",
                fileName, sw.ElapsedMilliseconds, top.BreedName, top.ConfidencePercent, isLowConf);

            return new BreedClassificationResult
            {
                TopBreedName      = top.BreedName,
                ConfidencePercent = top.ConfidencePercent,
                IsLowConfidence   = isLowConf,
                TopPredictions    = ranked.Take(5).ToList(),
                InferenceDurationMs = sw.ElapsedMilliseconds
            };
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static float[] Softmax(float[] logits)
        {
            float maxLogit = logits.Max();
            var exps = logits.Select(x => MathF.Exp(x - maxLogit)).ToArray();
            float sum = exps.Sum();
            return exps.Select(e => e / sum).ToArray();
        }

        /// <summary>
        /// Converts model class names like "Red_Sindhi" or "Malnad_gidda" to "Red Sindhi" / "Malnad Gidda".
        /// </summary>
        private static string NormalizeClassName(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return raw;
            var parts = raw.Split('_', StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", parts.Select(p =>
                p.Length > 0 ? char.ToUpperInvariant(p[0]) + p[1..] : p));
        }

        public void Dispose()
        {
            _session?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
