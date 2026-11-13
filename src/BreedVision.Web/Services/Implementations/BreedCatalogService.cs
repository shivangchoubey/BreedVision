using BreedVision.Web.Data;
using BreedVision.Web.Models.Entities;
using BreedVision.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BreedVision.Web.Services.Implementations
{
    public class BreedCatalogService : IBreedCatalogService
    {
        private readonly BreedVisionDbContext _context;
        private readonly ILogger<BreedCatalogService> _logger;

        public BreedCatalogService(BreedVisionDbContext context, ILogger<BreedCatalogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Breed>> GetAllBreedsAsync(string? species = null, string? searchTerm = null)
        {
            var query = _context.Breeds.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(species))
            {
                query = query.Where(b => b.Species.ToLower() == species.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(b => b.Name.ToLower().Contains(term) ||
                                         b.OriginState.ToLower().Contains(term) ||
                                         b.PrimaryUtility.ToLower().Contains(term) ||
                                         b.PhysicalTraits.ToLower().Contains(term));
            }

            return await query.OrderBy(b => b.Species).ThenBy(b => b.Name).ToListAsync();
        }

        public async Task<Breed?> GetBreedByIdAsync(int id)
        {
            return await _context.Breeds
                .AsNoTracking()
                .Include(b => b.PredictionRecords.OrderByDescending(p => p.PredictionTimestamp).Take(5))
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Breed?> GetBreedByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            var norm = name.Trim().ToLower();
            return await _context.Breeds
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.NormalizedKey == norm || b.Name.ToLower() == norm);
        }

        public async Task<DashboardStatsViewModel> GetDashboardStatsAsync()
        {
            var totalBreeds = await _context.Breeds.CountAsync();
            var cattleCount = await _context.Breeds.CountAsync(b => b.Species == "Cattle");
            var buffaloCount = await _context.Breeds.CountAsync(b => b.Species == "Buffalo");

            var totalPredictions = await _context.PredictionRecords.CountAsync();
            double avgConfidence = 0;
            if (totalPredictions > 0)
            {
                avgConfidence = (double)await _context.PredictionRecords.AverageAsync(p => p.ConfidenceScore);
            }

            var recentPredictions = await _context.PredictionRecords
                .AsNoTracking()
                .Include(p => p.Breed)
                .OrderByDescending(p => p.PredictionTimestamp)
                .Take(6)
                .ToListAsync();

            var featuredBreeds = await _context.Breeds
                .AsNoTracking()
                .Take(4)
                .ToListAsync();

            return new DashboardStatsViewModel
            {
                TotalBreedsCount = totalBreeds,
                CattleBreedsCount = cattleCount,
                BuffaloBreedsCount = buffaloCount,
                TotalPredictionsCount = totalPredictions,
                AverageConfidence = Math.Round(avgConfidence, 1),
                RecentPredictions = recentPredictions,
                FeaturedBreeds = featuredBreeds
            };
        }
    }
}
