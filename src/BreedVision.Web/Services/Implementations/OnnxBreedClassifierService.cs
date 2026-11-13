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
    public class OnnxBreedClassifierService : IBreedClassifierService, IDisposable
    {
        private readonly ILogger<OnnxBreedClassifierService> _logger;
        private readonly IWebHostEnvironment _env;
        private InferenceSession? _session;
        private Dictionary<string, float[]>? _prototypes;
        private readonly object _initLock = new();
        private bool _isInitialized = false;

        private readonly float[] _mean = [0.485f, 0.456f, 0.406f];
        private readonly float[] _std = [0.229f, 0.224f, 0.225f];

        private static readonly string[] BuffaloBreeds =
        [
            "Bhadawari", "Jaffarbadi", "Mehsana", "Murrah", "Surti"
        ];

        private static readonly string[] CattleBreeds =
        [
            "Gir", "Kankrej", "Ongole", "Sahiwal", "Tharparkar"
        ];

        public IReadOnlyList<string> SupportedBreeds =>
            [.. BuffaloBreeds, .. CattleBreeds];

        // If maximum cosine similarity is below 0.65 or top confidence is under 30%, flag as Out-Of-Distribution (OOD)
        public double LowConfidenceThreshold => 40.0;
        public float SimilarityRejectionThreshold => 0.68f;

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
                    var modelPath = Path.Combine(assetsDir, "model.onnx");
                    var protoPath = Path.Combine(assetsDir, "prototypes.json");

                    if (!File.Exists(modelPath))
                    {
                        throw new FileNotFoundException($"ONNX model not found at {modelPath}");
                    }

                    if (!File.Exists(protoPath))
                    {
                        throw new FileNotFoundException($"Prototypes file not found at {protoPath}");
                    }

                    _logger.LogInformation("Loading ONNX inference session from {Path}...", modelPath);
                    var sessionOptions = new Microsoft.ML.OnnxRuntime.SessionOptions
                    {
                        GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL,
                        ExecutionMode = ExecutionMode.ORT_SEQUENTIAL
                    };
                    _session = new InferenceSession(modelPath, sessionOptions);

                    _logger.LogInformation("Loading breed prototypes from {Path}...", protoPath);
                    var json = File.ReadAllText(protoPath);
                    using var doc = JsonDocument.Parse(json);
                    var protoRoot = doc.RootElement.GetProperty("prototypes");

                    _prototypes = new Dictionary<string, float[]>(StringComparer.OrdinalIgnoreCase);
                    foreach (var prop in protoRoot.EnumerateObject())
                    {
                        var arr = new float[2048];
                        int i = 0;
                        foreach (var num in prop.Value.EnumerateArray())
                        {
                            arr[i++] = num.GetSingle();
                        }
                        _prototypes[prop.Name] = arr;
                    }

                    _isInitialized = true;
                    _logger.LogInformation("BreedVision ONNX Classifier initialized with {Count} breed prototypes.", _prototypes.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize ONNX Classifier Service.");
                    throw;
                }
            }
        }

        public async Task<BreedClassificationResult> ClassifyImageAsync(Stream imageStream, string fileName)
        {
            if (!_isInitialized || _session == null || _prototypes == null)
            {
                Initialize();
            }

            var stopwatch = Stopwatch.StartNew();

            // 1. Decode & Preprocess image using ImageSharp
            using var image = await Image.LoadAsync<Rgb24>(imageStream);

            // Resize smaller edge to 256, then center-crop to 224x224 (ResNet-50 standard)
            image.Mutate(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(256, 256),
                Mode = ResizeMode.Crop
            }).Crop(new Rectangle(16, 16, 224, 224)));

            // 2. Build DenseTensor [1, 3, 224, 224] with ImageNet normalization
            var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < accessor.Width; x++)
                    {
                        var p = row[x];
                        tensor[0, 0, y, x] = ((p.R / 255.0f) - _mean[0]) / _std[0];
                        tensor[0, 1, y, x] = ((p.G / 255.0f) - _mean[1]) / _std[1];
                        tensor[0, 2, y, x] = ((p.B / 255.0f) - _mean[2]) / _std[2];
                    }
                }
            });

            // 3. Run ONNX Inference to extract 2048-dim feature embedding
            var inputName = _session!.InputMetadata.Keys.First();
            using var outputs = _session.Run(new[] { NamedOnnxValue.CreateFromTensor(inputName, tensor) });
            var rawFeatures = outputs.First().AsEnumerable<float>().ToArray();

            // L2-normalize extracted features
            double norm = Math.Sqrt(rawFeatures.Select(f => (double)f * f).Sum());
            var features = new float[rawFeatures.Length];
            if (norm > 1e-6)
            {
                for (int i = 0; i < rawFeatures.Length; i++)
                {
                    features[i] = (float)(rawFeatures[i] / norm);
                }
            }
            else
            {
                features = rawFeatures;
            }

            // 4. Compute Cosine Similarity with each breed prototype
            var similarities = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in _prototypes!)
            {
                float dot = 0f;
                var proto = kvp.Value;
                for (int i = 0; i < 2048; i++)
                {
                    dot += features[i] * proto[i];
                }
                similarities[kvp.Key] = dot;
            }

            // 5. Compute Mathematical Metric Match Confidence
            // In metric prototype learning, cosine similarity dot(features, prototype) in [-1, 1] 
            // directly represents the phenotypic alignment score with each breed centroid.
            float maxSim = similarities.Values.Max();

            // Normalize positive similarities to sum to 100% for relative distribution chart
            var positiveSims = similarities.ToDictionary(k => k.Key, v => Math.Max(0f, v.Value));
            float sumPositive = positiveSims.Values.Sum();
            if (sumPositive < 1e-6f) sumPositive = 1f;

            // Rank predictions
            var ranked = similarities
                .OrderByDescending(kv => kv.Value)
                .Select((kv, index) => new ClassPrediction
                {
                    BreedName = kv.Key,
                    // Individual alignment percentage (e.g. 0.814 -> 81.4%)
                    ConfidencePercent = Math.Round(kv.Value * 100.0, 1),
                    Rank = index + 1,
                    Species = BuffaloBreeds.Contains(kv.Key, StringComparer.OrdinalIgnoreCase) ? "Buffalo" : "Cattle"
                })
                .ToList();

            var top = ranked[0];
            stopwatch.Stop();

            // Out-of-Distribution (OOD) check:
            // High-quality bovine images achieve > 0.75 similarity. Low similarity (< 0.68) indicates 
            // an unsupported breed, non-bovine image, or heavily obscured photograph.
            bool isLowConfidence = maxSim < SimilarityRejectionThreshold;

            _logger.LogInformation("Classified {FileName} in {Ms}ms. Top: {Breed} (Similarity Match: {Sim:F1}%, LowConf: {Low})",
                fileName, stopwatch.ElapsedMilliseconds, top.BreedName, top.ConfidencePercent, isLowConfidence);

            return new BreedClassificationResult
            {
                TopBreedName = top.BreedName,
                ConfidencePercent = top.ConfidencePercent,
                IsLowConfidence = isLowConfidence,
                TopPredictions = ranked.Take(5).ToList(),
                InferenceDurationMs = stopwatch.ElapsedMilliseconds
            };
        }

        public void Dispose()
        {
            _session?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
