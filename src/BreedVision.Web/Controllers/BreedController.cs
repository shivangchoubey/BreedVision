using System.Text.Json;
using BreedVision.Web.Models.Entities;
using BreedVision.Web.Models.ViewModels;
using BreedVision.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BreedVision.Web.Controllers
{
    public class BreedController : Controller
    {
        private readonly IBreedClassifierService _classifierService;
        private readonly IImageStorageService _storageService;
        private readonly IBreedCatalogService _catalogService;
        private readonly IPredictionHistoryService _historyService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<BreedController> _logger;

        public BreedController(
            IBreedClassifierService classifierService,
            IImageStorageService storageService,
            IBreedCatalogService catalogService,
            IPredictionHistoryService historyService,
            IWebHostEnvironment env,
            ILogger<BreedController> logger)
        {
            _classifierService = classifierService;
            _storageService = storageService;
            _catalogService = catalogService;
            _historyService = historyService;
            _env = env;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Identify()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Identify(IFormFile? imageFile, string? userNotes)
        {
            if (imageFile == null)
            {
                ModelState.AddModelError("imageFile", "Please select a cattle or buffalo image to upload.");
                return View();
            }

            var (isValid, errorMessage) = await _storageService.ValidateImageAsync(imageFile);
            if (!isValid)
            {
                ModelState.AddModelError("imageFile", errorMessage ?? "Invalid image file.");
                return View();
            }

            try
            {
                // 1. Save uploaded image to disk
                var savedImagePath = await _storageService.SaveImageAsync(imageFile);

                // 2. Run ONNX Model Inference
                BreedClassificationResult classificationResult;
                using (var stream = imageFile.OpenReadStream())
                {
                    classificationResult = await _classifierService.ClassifyImageAsync(stream, imageFile.FileName);
                }

                // 3. Match predicted breed with database catalog
                var breedProfile = await _catalogService.GetBreedByNameAsync(classificationResult.TopBreedName);

                // 4. Save prediction record to SQL Server
                var record = new PredictionRecord
                {
                    OriginalFileName = Path.GetFileName(imageFile.FileName),
                    SavedImagePath = savedImagePath,
                    PredictedBreedName = classificationResult.TopBreedName,
                    BreedId = breedProfile?.Id,
                    ConfidenceScore = (decimal)classificationResult.ConfidencePercent,
                    IsLowConfidence = classificationResult.IsLowConfidence,
                    TopPredictionsJson = JsonSerializer.Serialize(classificationResult.TopPredictions),
                    PredictionTimestamp = DateTime.UtcNow,
                    InferenceTimeMs = classificationResult.InferenceDurationMs,
                    UserNotes = userNotes,
                    ClientIp = HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                await _historyService.SavePredictionAsync(record);

                // 5. Construct Result ViewModel
                var viewModel = new PredictionResultViewModel
                {
                    IsSuccess = true,
                    OriginalFileName = record.OriginalFileName,
                    ImageUrl = savedImagePath,
                    PredictedBreedName = classificationResult.TopBreedName,
                    ConfidencePercent = classificationResult.ConfidencePercent,
                    IsLowConfidence = classificationResult.IsLowConfidence,
                    BreedProfile = breedProfile,
                    TopPredictions = classificationResult.TopPredictions,
                    InferenceTimeMs = classificationResult.InferenceDurationMs,
                    PredictionRecordId = record.Id,
                    UserNotes = userNotes,
                    Timestamp = record.PredictionTimestamp
                };

                return View("Result", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to classify image {FileName}", imageFile.FileName);
                ModelState.AddModelError(string.Empty, "An error occurred while processing the image. Please ensure the file is an uncorrupted image.");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestSample(string sampleName)
        {
            var allowedSamples = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "gir", "Gir_sample.jpg" },
                { "bhadawari", "Bhadawari_sample.jpg" }
            };

            if (string.IsNullOrWhiteSpace(sampleName) || !allowedSamples.TryGetValue(sampleName, out var sampleFile))
            {
                return RedirectToAction(nameof(Identify));
            }

            var sampleFullPath = Path.Combine(_env.ContentRootPath, "ML", "Assets", sampleFile);
            if (!System.IO.File.Exists(sampleFullPath))
            {
                TempData["ErrorMessage"] = $"Sample file {sampleFile} not found.";
                return RedirectToAction(nameof(Identify));
            }

            try
            {
                // Copy sample to uploads
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

                var uniqueName = $"sample_{sampleName}_{Guid.NewGuid():N}.jpg";
                var destPath = Path.Combine(uploadsDir, uniqueName);
                System.IO.File.Copy(sampleFullPath, destPath, true);
                var savedImagePath = $"/uploads/{uniqueName}";

                // Run classification
                BreedClassificationResult classificationResult;
                using (var stream = System.IO.File.OpenRead(sampleFullPath))
                {
                    classificationResult = await _classifierService.ClassifyImageAsync(stream, sampleFile);
                }

                var breedProfile = await _catalogService.GetBreedByNameAsync(classificationResult.TopBreedName);

                var record = new PredictionRecord
                {
                    OriginalFileName = sampleFile,
                    SavedImagePath = savedImagePath,
                    PredictedBreedName = classificationResult.TopBreedName,
                    BreedId = breedProfile?.Id,
                    ConfidenceScore = (decimal)classificationResult.ConfidencePercent,
                    IsLowConfidence = classificationResult.IsLowConfidence,
                    TopPredictionsJson = JsonSerializer.Serialize(classificationResult.TopPredictions),
                    PredictionTimestamp = DateTime.UtcNow,
                    InferenceTimeMs = classificationResult.InferenceDurationMs,
                    UserNotes = $"Pre-verified sample test for {sampleName.ToUpperInvariant()}"
                };

                await _historyService.SavePredictionAsync(record);

                var viewModel = new PredictionResultViewModel
                {
                    IsSuccess = true,
                    OriginalFileName = sampleFile,
                    ImageUrl = savedImagePath,
                    PredictedBreedName = classificationResult.TopBreedName,
                    ConfidencePercent = classificationResult.ConfidencePercent,
                    IsLowConfidence = classificationResult.IsLowConfidence,
                    BreedProfile = breedProfile,
                    TopPredictions = classificationResult.TopPredictions,
                    InferenceTimeMs = classificationResult.InferenceDurationMs,
                    PredictionRecordId = record.Id,
                    UserNotes = record.UserNotes,
                    Timestamp = record.PredictionTimestamp
                };

                return View("Result", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to run sample inference for {Sample}", sampleName);
                TempData["ErrorMessage"] = "Failed to run sample inference.";
                return RedirectToAction(nameof(Identify));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Result(int id)
        {
            var record = await _historyService.GetPredictionByIdAsync(id);
            if (record == null)
            {
                return NotFound();
            }

            var topPredictions = new List<ClassPrediction>();
            if (!string.IsNullOrEmpty(record.TopPredictionsJson))
            {
                try
                {
                    topPredictions = JsonSerializer.Deserialize<List<ClassPrediction>>(record.TopPredictionsJson) ?? new();
                }
                catch { }
            }

            var viewModel = new PredictionResultViewModel
            {
                IsSuccess = true,
                OriginalFileName = record.OriginalFileName,
                ImageUrl = record.SavedImagePath,
                PredictedBreedName = record.PredictedBreedName,
                ConfidencePercent = (double)record.ConfidenceScore,
                IsLowConfidence = record.IsLowConfidence,
                BreedProfile = record.Breed,
                TopPredictions = topPredictions,
                InferenceTimeMs = record.InferenceTimeMs,
                PredictionRecordId = record.Id,
                UserNotes = record.UserNotes,
                Timestamp = record.PredictionTimestamp
            };

            return View(viewModel);
        }
    }
}

