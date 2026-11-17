using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using BreedVision.Web.Data;
using BreedVision.Web.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

Console.WriteLine("=====================================================================");
Console.WriteLine("          BreedVision Complete Verification & Diagnostic Suite       ");
Console.WriteLine("=====================================================================");

int passed = 0;
int total = 0;

void RunTest(string name, Action action)
{
    total++;
    try
    {
        Console.Write($"[TEST {total}] {name,-55} ... ");
        action();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("PASSED");
        Console.ResetColor();
        passed++;
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("FAILED");
        Console.WriteLine($"   Exception: {ex.GetType().Name}: {ex.Message}");
        Console.ResetColor();
    }
}

async Task RunTestAsync(string name, Func<Task> action)
{
    total++;
    try
    {
        Console.Write($"[TEST {total}] {name,-55} ... ");
        await action();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("PASSED");
        Console.ResetColor();
        passed++;
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("FAILED");
        Console.WriteLine($"   Exception: {ex.GetType().Name}: {ex.Message}");
        Console.ResetColor();
    }
}

// 1. Model Asset Verification
RunTest("41-Breed ONNX Model exists and initializes InferenceSession", () =>
{
    var modelPath = @"C:\BreedVision\src\BreedVision.Web\ML\Assets\model_41breed.onnx";
    if (!File.Exists(modelPath)) throw new FileNotFoundException("model_41breed.onnx not found", modelPath);
    using var session = new InferenceSession(modelPath);
    if (session.InputMetadata.Count == 0) throw new InvalidOperationException("No inputs in model");
    var outMeta = session.OutputMetadata.Values.First();
    int numClasses = outMeta.Dimensions[1];
    if (numClasses != 41) throw new InvalidOperationException($"Expected 41 output classes, found {numClasses}");
});

// 2. Classes JSON Verification
RunTest("classes.json contains all 41 ICAR-NBAGR bovine breeds", () =>
{
    var clsPath = @"C:\BreedVision\src\BreedVision.Web\ML\Assets\classes.json";
    var json = File.ReadAllText(clsPath);
    var list = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<string>>(json);
    if (list == null || list.Count != 41) throw new InvalidOperationException($"Expected 41 classes, found {list?.Count}");
    if (!list.Contains("Red_Sindhi")) throw new InvalidOperationException("Missing Red_Sindhi class");
    if (!list.Contains("Rathi")) throw new InvalidOperationException("Missing Rathi class");
    if (!list.Contains("Sahiwal")) throw new InvalidOperationException("Missing Sahiwal class");
});

// 3. In-Memory SQLite Seed Verification
await RunTestAsync("In-Memory SQLite Seeding verifies 41 breeds catalog", async () =>
{
    var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
    connection.Open();
    using (connection)
    {
        var options = new DbContextOptionsBuilder<BreedVisionDbContext>().UseSqlite(connection).Options;
        using var ctx = new BreedVisionDbContext(options);
        await ctx.Database.EnsureCreatedAsync();
        await DbInitializer.InitializeAsync(ctx, NullLogger.Instance);
        var count = await ctx.Breeds.CountAsync();
        if (count != 41) throw new InvalidOperationException($"Expected 41 breeds in DB, found {count}");
        var redSindhi = await ctx.Breeds.FirstOrDefaultAsync(b => b.Name == "Red Sindhi");
        if (redSindhi == null) throw new InvalidOperationException("Red Sindhi not found in seeded DB");
        var rathi = await ctx.Breeds.FirstOrDefaultAsync(b => b.Name == "Rathi");
        if (rathi == null) throw new InvalidOperationException("Rathi not found in seeded DB");
    }
});

// 4. SQLite Database Query & Decimal AverageAsync Fix Verification
await RunTestAsync("SQLite GetDashboardStatsAsync executes correctly on 41 breeds", async () =>
{
    var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
    connection.Open();
    using (connection)
    {
        var options = new DbContextOptionsBuilder<BreedVisionDbContext>().UseSqlite(connection).Options;
        using var ctx = new BreedVisionDbContext(options);
        await ctx.Database.EnsureCreatedAsync();
        await DbInitializer.InitializeAsync(ctx, NullLogger.Instance);
        var catalog = new BreedCatalogService(ctx, NullLogger<BreedCatalogService>.Instance);
        var stats = await catalog.GetDashboardStatsAsync();
        if (stats.TotalBreedsCount != 41) throw new InvalidOperationException($"Expected 41 breeds count, got {stats.TotalBreedsCount}");
        if (stats.BuffaloBreedsCount != 8) throw new InvalidOperationException($"Expected 8 buffalo breeds, got {stats.BuffaloBreedsCount}");
        if (stats.CattleBreedsCount != 33) throw new InvalidOperationException($"Expected 33 cattle breeds, got {stats.CattleBreedsCount}");
    }
});

// 5. Live ONNX Inference on Research Sample
await RunTestAsync("Live 41-Breed ONNX inference produces Top-1 prediction", async () =>
{
    var mockEnv = new BreedVision.Tests.TestWebHostEnvironment();
    using var classifier = new OnnxBreedClassifierService(NullLogger<OnnxBreedClassifierService>.Instance, mockEnv);
    var samplePath = Path.Combine(mockEnv.ContentRootPath, "ML", "Assets", "Gir_sample.jpg");
    using var fs = File.OpenRead(samplePath);
    var res = await classifier.ClassifyImageAsync(fs, "Gir_sample.jpg");
    Console.WriteLine($"\n   [Gir sample Top-1: '{res.TopBreedName}' ({res.ConfidencePercent}%), Top-5: {string.Join(", ", res.TopPredictions.Select(p => $"{p.BreedName}:{p.ConfidencePercent}%"))}]");
    if (res.TopPredictions.Count != 5) throw new InvalidOperationException("Expected 5 predictions");
});

// 6. Test Uploaded Cow Images with 41-Breed Classifier
await RunTestAsync("Inference on uploaded test images", async () =>
{
    var mockEnv = new BreedVision.Tests.TestWebHostEnvironment();
    using var classifier = new OnnxBreedClassifierService(NullLogger<OnnxBreedClassifierService>.Instance, mockEnv);
    var uploadsDir = Path.Combine(mockEnv.ContentRootPath, "wwwroot", "uploads");
    var imgFiles = Directory.GetFiles(uploadsDir, "*.jpg");
    foreach (var file in imgFiles)
    {
        using var fs = File.OpenRead(file);
        var res = await classifier.ClassifyImageAsync(fs, Path.GetFileName(file));
        Console.WriteLine($"\n   [{Path.GetFileName(file)} -> Top-1: '{res.TopBreedName}' ({res.ConfidencePercent:F1}%), Species: {res.TopPredictions.First().Species}, Top-3: {string.Join(", ", res.TopPredictions.Take(3).Select(p => $"{p.BreedName}({p.ConfidencePercent}%)"))}]");
    }
});

Console.WriteLine("=====================================================================");
Console.WriteLine($"Test Results: {passed}/{total} Passed ({(double)passed/total*100:F0}%)");
Console.WriteLine("=====================================================================");




