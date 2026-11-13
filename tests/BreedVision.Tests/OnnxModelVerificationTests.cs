using System.Text.Json;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;
using Xunit.Abstractions;

namespace BreedVision.Tests
{
    public class OnnxModelVerificationTests
    {
        private readonly ITestOutputHelper _output;

        public OnnxModelVerificationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private static string GetAssetsDir()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "src", "BreedVision.Web", "ML", "Assets"));
            if (!Directory.Exists(path))
            {
                path = @"C:\BreedVision\src\BreedVision.Web\ML\Assets";
            }
            return path;
        }

        [Fact]
        public void ModelFile_ShouldExistAndLoadSuccessfully()
        {
            var assetsDir = GetAssetsDir();
            var modelPath = Path.Combine(assetsDir, "model.onnx");
            Assert.True(File.Exists(modelPath), $"Model file not found at: {modelPath}");

            using var session = new InferenceSession(modelPath);
            Assert.NotNull(session);

            _output.WriteLine("=== ONNX Model Loaded Successfully ===");
            foreach (var input in session.InputMetadata)
            {
                _output.WriteLine($"Input: '{input.Key}', Type: {input.Value.ElementType}, Shape: [{string.Join(", ", input.Value.Dimensions)}]");
            }

            foreach (var output in session.OutputMetadata)
            {
                _output.WriteLine($"Output: '{output.Key}', Type: {output.Value.ElementType}, Shape: [{string.Join(", ", output.Value.Dimensions)}]");
            }
        }

        [Fact]
        public void Prototypes_ShouldExistAndHave10BreedsWith2048Dim()
        {
            var assetsDir = GetAssetsDir();
            var protoPath = Path.Combine(assetsDir, "prototypes.json");
            Assert.True(File.Exists(protoPath), $"prototypes.json not found at: {protoPath}");

            var json = File.ReadAllText(protoPath);
            using var doc = JsonDocument.Parse(json);
            var protoRoot = doc.RootElement.GetProperty("prototypes");

            int count = 0;
            foreach (var prop in protoRoot.EnumerateObject())
            {
                count++;
                int dim = prop.Value.GetArrayLength();
                Assert.Equal(2048, dim);
                _output.WriteLine($"  Breed: {prop.Name}, Prototype Dim: {dim}");
            }

            Assert.Equal(10, count);
        }

        [Theory]
        [InlineData("Gir_sample.jpg", "Gir")]
        [InlineData("Bhadawari_sample.jpg", "Bhadawari")]
        public void Inference_OnSampleImages_ShouldMatchExpectedBreed(string sampleFileName, string expectedBreed)
        {
            var assetsDir = GetAssetsDir();
            var modelPath = Path.Combine(assetsDir, "model.onnx");
            var protoPath = Path.Combine(assetsDir, "prototypes.json");
            var samplePath = Path.Combine(assetsDir, sampleFileName);

            Assert.True(File.Exists(samplePath), $"Sample file not found: {samplePath}");

            // Load prototypes
            var json = File.ReadAllText(protoPath);
            using var doc = JsonDocument.Parse(json);
            var protoRoot = doc.RootElement.GetProperty("prototypes");
            var prototypes = new Dictionary<string, float[]>();
            foreach (var prop in protoRoot.EnumerateObject())
            {
                var arr = new float[2048];
                int idx = 0;
                foreach (var num in prop.Value.EnumerateArray())
                {
                    arr[idx++] = num.GetSingle();
                }
                prototypes[prop.Name] = arr;
            }

            // Load ONNX Session
            using var session = new InferenceSession(modelPath);
            var inputName = session.InputMetadata.Keys.First();

            // Preprocess Image (Resize 256 -> CenterCrop 224, standard ResNet-50)
            using var image = Image.Load<Rgb24>(samplePath);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(256, 256),
                Mode = ResizeMode.Crop
            }).Crop(new Rectangle(16, 16, 224, 224)));

            var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
            float[] mean = { 0.485f, 0.456f, 0.406f };
            float[] std = { 0.229f, 0.224f, 0.225f };

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < accessor.Width; x++)
                    {
                        var pixel = row[x];
                        tensor[0, 0, y, x] = ((pixel.R / 255.0f) - mean[0]) / std[0];
                        tensor[0, 1, y, x] = ((pixel.G / 255.0f) - mean[1]) / std[1];
                        tensor[0, 2, y, x] = ((pixel.B / 255.0f) - mean[2]) / std[2];
                    }
                }
            });

            // Run Inference
            using var results = session.Run(new[] { NamedOnnxValue.CreateFromTensor(inputName, tensor) });
            var features = results.First().AsEnumerable<float>().ToArray();
            Assert.Equal(2048, features.Length);

            // Compute L2 norm of features
            double featureNorm = Math.Sqrt(features.Select(f => (double)f * f).Sum());
            if (featureNorm > 1e-6)
            {
                for (int i = 0; i < features.Length; i++)
                {
                    features[i] = (float)(features[i] / featureNorm);
                }
            }

            // Calculate Dot Product / Cosine Similarity with all 10 prototypes
            var similarities = new Dictionary<string, float>();
            foreach (var kvp in prototypes)
            {
                float dot = 0f;
                for (int i = 0; i < 2048; i++)
                {
                    dot += features[i] * kvp.Value[i];
                }
                similarities[kvp.Key] = dot;
            }

            // Calibrated Softmax over similarities (temperature = 0.08)
            float maxSim = similarities.Values.Max();
            float temperature = 0.08f;
            var expDict = similarities.ToDictionary(k => k.Key, v => Math.Exp((v.Value - maxSim) / temperature));
            double sumExp = expDict.Values.Sum();
            var confidences = expDict.ToDictionary(k => k.Key, v => (v.Value / sumExp) * 100.0);

            var ranked = similarities.OrderByDescending(kv => kv.Value).ToList();
            _output.WriteLine($"\n=== Results for {sampleFileName} (Expected: {expectedBreed}) ===");
            foreach (var item in ranked)
            {
                _output.WriteLine($"  {item.Key,-12}: Sim = {item.Value:F4} | Softmax Conf = {confidences[item.Key]:F2}%");
            }

            Assert.Equal(expectedBreed, ranked[0].Key);
            _output.WriteLine($"--> SUCCESS: Top prediction '{ranked[0].Key}' matches expected '{expectedBreed}'!");
        }
    }
}
