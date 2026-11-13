using BreedVision.Web.Models.ViewModels;
using BreedVision.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BreedVision.Web.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IBreedCatalogService _catalogService;

        public CatalogController(IBreedCatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? species, string? search)
        {
            var breeds = await _catalogService.GetAllBreedsAsync(species, search);
            ViewBag.SelectedSpecies = species;
            ViewBag.SearchTerm = search;
            return View(breeds);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var breed = await _catalogService.GetBreedByIdAsync(id);
            if (breed == null)
            {
                return NotFound();
            }

            var viewModel = new BreedDetailViewModel
            {
                Breed = breed,
                RecentPredictions = breed.PredictionRecords.ToList(),
                TotalPredictionsCount = breed.PredictionRecords.Count,
                AverageConfidence = breed.PredictionRecords.Any()
                    ? (double)breed.PredictionRecords.Average(p => p.ConfidenceScore)
                    : 0.0
            };

            return View(viewModel);
        }
    }
}
