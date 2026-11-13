using System.Diagnostics;
using BreedVision.Web.Models;
using BreedVision.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BreedVision.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBreedCatalogService _catalogService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IBreedCatalogService catalogService, ILogger<HomeController> logger)
        {
            _catalogService = catalogService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _catalogService.GetDashboardStatsAsync();
            return View(stats);
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
