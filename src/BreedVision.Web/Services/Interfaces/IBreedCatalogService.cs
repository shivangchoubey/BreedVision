using BreedVision.Web.Models.Entities;

namespace BreedVision.Web.Services.Interfaces
{
    public class DashboardStatsViewModel
    {
        public int TotalBreedsCount { get; set; }
        public int CattleBreedsCount { get; set; }
        public int BuffaloBreedsCount { get; set; }
        public int TotalPredictionsCount { get; set; }
        public double AverageConfidence { get; set; }
        public List<PredictionRecord> RecentPredictions { get; set; } = new();
        public List<Breed> FeaturedBreeds { get; set; } = new();
    }

    public interface IBreedCatalogService
    {
        Task<List<Breed>> GetAllBreedsAsync(string? species = null, string? searchTerm = null);
        Task<Breed?> GetBreedByIdAsync(int id);
        Task<Breed?> GetBreedByNameAsync(string name);
        Task<DashboardStatsViewModel> GetDashboardStatsAsync();
    }
}
