using BreedVision.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BreedVision.Web.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IPredictionHistoryService _historyService;
        private readonly ILogger<HistoryController> _logger;

        public HistoryController(IPredictionHistoryService historyService, ILogger<HistoryController> logger)
        {
            _historyService = historyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            string? breed,
            string? species,
            bool? lowConfidenceOnly,
            int page = 1)
        {
            var viewModel = await _historyService.GetHistoryAsync(
                search,
                breed,
                species,
                lowConfidenceOnly,
                page,
                pageSize: 12);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _historyService.DeletePredictionAsync(id);
            TempData["SuccessMessage"] = "Prediction record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}

