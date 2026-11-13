using System;
using System.IO;
using System.Threading.Tasks;
using BreedVision.Tests;

Console.WriteLine("==========================================================");
Console.WriteLine("          BreedVision Full Verification Test Suite        ");
Console.WriteLine("==========================================================");

var output = new ConsoleTestOutputHelper();
var onnxTester = new OnnxModelVerificationTests(output);
var servicesTester = new ServicesIntegrationTests(output);

try
{
    Console.WriteLine("\n[1/6] ONNX Model Structure & Metadata Inspection...");
    onnxTester.ModelFile_ShouldExistAndLoadSuccessfully();
    Console.WriteLine("--> PASS: ResNet-50 ONNX model valid.");

    Console.WriteLine("\n[2/6] Prototypes Dictionary (10 Breeds, 2048-dim)...");
    onnxTester.Prototypes_ShouldExistAndHave10BreedsWith2048Dim();
    Console.WriteLine("--> PASS: 10 Indian breeds loaded with 2048-dim prototypes.");

    Console.WriteLine("\n[3/6] Database Seeding (ICAR-NBAGR Standards)...");
    await servicesTester.DbInitializer_ShouldSeed10DistinctBreedsWithCompleteData();
    Console.WriteLine("--> PASS: Database seeding confirmed (5 Cattle, 5 Buffalo).");

    Console.WriteLine("\n[4/6] Breed Catalog & Filter Services...");
    await servicesTester.BreedCatalogService_ShouldFilterBySpeciesAndSearch();
    Console.WriteLine("--> PASS: Filtering by species, keyword, and stats verified.");

    Console.WriteLine("\n[5/6] Image Storage & Magic Byte Security Validation...");
    await servicesTester.LocalFileStorageService_ShouldValidateImageHeadersCorrectly();
    Console.WriteLine("--> PASS: Image validation (JPEG/PNG/WEBP magic bytes & size) verified.");

    Console.WriteLine("\n[6/6] End-to-End Live ML Inference (Gir Sample)...");
    await servicesTester.OnnxClassifierService_ShouldProduceLivePredictionsAndDetectOOD();
    Console.WriteLine("--> PASS: Live inference & confidence distribution verified.");

    Console.WriteLine("\n==========================================================");
    Console.WriteLine("   ALL TESTS PASSED SUCCESSFULLY (6/6)                    ");
    Console.WriteLine("==========================================================");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nTEST FAILED: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Console.ResetColor();
    Environment.Exit(1);
}

public class ConsoleTestOutputHelper : Xunit.Abstractions.ITestOutputHelper
{
    public void WriteLine(string message) => Console.WriteLine(message);
    public void WriteLine(string format, params object[] args) => Console.WriteLine(format, args);
}
