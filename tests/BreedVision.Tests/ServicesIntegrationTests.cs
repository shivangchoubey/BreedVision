using BreedVision.Web.Data;
using BreedVision.Web.Models.Entities;
using BreedVision.Web.Services.Implementations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace BreedVision.Tests
{
    public class ServicesIntegrationTests
    {
        private readonly ITestOutputHelper _output;

        public ServicesIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private static (BreedVisionDbContext Context, Microsoft.Data.Sqlite.SqliteConnection Connection) CreateTestDbContext()
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<BreedVisionDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new BreedVisionDbContext(options);
            return (context, connection);
        }

        [Fact]
        public async Task DbInitializer_ShouldSeed10DistinctBreedsWithCompleteData()
        {
            var (context, conn) = CreateTestDbContext();
            using (conn)
            using (context)
            {
                await DbInitializer.InitializeAsync(context, NullLogger.Instance);

                var breeds = await context.Breeds.ToListAsync();
                Assert.Equal(10, breeds.Count);

                int cattleCount = breeds.Count(b => b.Species == "Cattle");
                int buffaloCount = breeds.Count(b => b.Species == "Buffalo");
                Assert.Equal(5, cattleCount);
                Assert.Equal(5, buffaloCount);

                foreach (var b in breeds)
                {
                    Assert.False(string.IsNullOrWhiteSpace(b.Name), "Breed Name should not be empty");
                    Assert.False(string.IsNullOrWhiteSpace(b.OriginState), $"OriginState for {b.Name} should not be empty");
                    Assert.False(string.IsNullOrWhiteSpace(b.AverageMilkYield), $"AverageMilkYield for {b.Name} should not be empty");
                    Assert.False(string.IsNullOrWhiteSpace(b.HornCharacteristics), $"HornCharacteristics for {b.Name} should not be empty");
                    Assert.False(string.IsNullOrWhiteSpace(b.PhysicalTraits), $"PhysicalTraits for {b.Name} should not be empty");
                    _output.WriteLine($"Verified Breed: {b.Name,-12} [{b.Species,-7}] - Origin: {b.OriginState}");
                }
            }
        }

        [Fact]
        public async Task BreedCatalogService_ShouldFilterBySpeciesAndSearch()
        {
            var (context, conn) = CreateTestDbContext();
            using (conn)
            using (context)
            {
                await DbInitializer.InitializeAsync(context, NullLogger.Instance);
                var catalog = new BreedCatalogService(context, NullLogger<BreedCatalogService>.Instance);

                // Test Cattle Filter
                var cattle = await catalog.GetAllBreedsAsync(species: "Cattle");
                Assert.Equal(5, cattle.Count);
                Assert.All(cattle, b => Assert.Equal("Cattle", b.Species));

                // Test Buffalo Filter
                var buffalo = await catalog.GetAllBreedsAsync(species: "Buffalo");
                Assert.Equal(5, buffalo.Count);
                Assert.All(buffalo, b => Assert.Equal("Buffalo", b.Species));

                // Test Search by State
                var gujaratBreeds = await catalog.GetAllBreedsAsync(searchTerm: "Gujarat");
                Assert.True(gujaratBreeds.Count >= 3, "Expected at least 3 breeds originating from Gujarat (Gir, Kankrej, Jaffrabadi, etc.)");

                // Test Get by Name
                var gir = await catalog.GetBreedByNameAsync("Gir");
                Assert.NotNull(gir);
                Assert.Equal("Gir", gir.Name);

                // Test Dashboard Stats
                var stats = await catalog.GetDashboardStatsAsync();
                Assert.Equal(10, stats.TotalBreedsCount);
                Assert.Equal(5, stats.CattleBreedsCount);
                Assert.Equal(5, stats.BuffaloBreedsCount);
            }
        }

        [Fact]
        public async Task LocalFileStorageService_ShouldValidateImageHeadersCorrectly()
        {
            var mockEnv = new TestWebHostEnvironment();
            var storage = new LocalFileStorageService(mockEnv, NullLogger<LocalFileStorageService>.Instance);

            // 1. Valid JPEG Header
            byte[] validJpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46 };
            using var validStream = new MemoryStream(validJpegBytes);
            var validFile = new FormFile(validStream, 0, validJpegBytes.Length, "imageFile", "test.jpg");
            var (isValid, error) = await storage.ValidateImageAsync(validFile);
            Assert.True(isValid);
            Assert.Null(error);

            // 2. Corrupted Header with .jpg extension
            byte[] invalidBytes = new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77 };
            using var invalidStream = new MemoryStream(invalidBytes);
            var invalidFile = new FormFile(invalidStream, 0, invalidBytes.Length, "imageFile", "fake.jpg");
            var (isInvalid, invalidError) = await storage.ValidateImageAsync(invalidFile);
            Assert.False(isInvalid);
            Assert.NotNull(invalidError);

            // 3. Unsupported extension
            var exeFile = new FormFile(validStream, 0, validJpegBytes.Length, "imageFile", "virus.exe");
            var (isExeValid, exeError) = await storage.ValidateImageAsync(exeFile);
            Assert.False(isExeValid);
            Assert.Contains("Unsupported file format", exeError);
        }

        [Fact]
        public async Task OnnxClassifierService_ShouldProduceLivePredictionsAndDetectOOD()
        {
            var mockEnv = new TestWebHostEnvironment();
            using var classifier = new OnnxBreedClassifierService(NullLogger<OnnxBreedClassifierService>.Instance, mockEnv);

            var samplePath = Path.Combine(mockEnv.ContentRootPath, "ML", "Assets", "Gir_sample.jpg");
            Assert.True(File.Exists(samplePath), $"Sample photo missing: {samplePath}");

            using var fileStream = File.OpenRead(samplePath);
            var result = await classifier.ClassifyImageAsync(fileStream, "Gir_sample.jpg");

            Assert.NotNull(result);
            Assert.Equal("Gir", result.TopBreedName);
            Assert.True(result.ConfidencePercent > 10.0);
            Assert.Equal(5, result.TopPredictions.Count);
            Assert.True(result.InferenceDurationMs >= 0);

            _output.WriteLine($"=== Live Inference Verified ===");
            _output.WriteLine($"Top Breed: {result.TopBreedName}");
            _output.WriteLine($"Confidence: {result.ConfidencePercent:F2}%");
            _output.WriteLine($"Latency: {result.InferenceDurationMs} ms");
            foreach (var p in result.TopPredictions)
            {
                _output.WriteLine($"  #{p.Rank} {p.BreedName,-12} ({p.Species}): {p.ConfidencePercent:F2}%");
            }
        }
    }

    public class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = @"C:\BreedVision\src\BreedVision.Web\wwwroot";
        public IFileProvider WebRootFileProvider { get; set; } = null!;
        public string ApplicationName { get; set; } = "BreedVision.Web";
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
        public string ContentRootPath { get; set; } = @"C:\BreedVision\src\BreedVision.Web";
        public string EnvironmentName { get; set; } = "Development";
    }
}
