using BreedVision.Web.Data;
using BreedVision.Web.Models.Entities;
using BreedVision.Web.Models.ViewModels;
using BreedVision.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BreedVision.Web.Services.Implementations
{
    public class PredictionHistoryService : IPredictionHistoryService
    {
        private readonly BreedVisionDbContext _context;
        private readonly IImageStorageService _storageService;
        private readonly ILogger<PredictionHistoryService> _logger;

        public PredictionHistoryService(
            BreedVisionDbContext context,
            IImageStorageService storageService,
            ILogger<PredictionHistoryService> logger)
        {
            _context = context;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<PredictionRecord> SavePredictionAsync(PredictionRecord record)
        {
            _context.PredictionRecords.Add(record);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Saved prediction record #{Id} for breed '{Breed}'", record.Id, record.PredictedBreedName);
            return record;
        }

        public async Task<HistoryFilterViewModel> GetHistoryAsync(
            string? searchTerm,
            string? breedFilter,
            string? speciesFilter,
            bool? lowConfidenceOnly,
            int page,
            int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 6, 48);

            var query = _context.PredictionRecords
                .Include(p => p.Breed)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(p => p.OriginalFileName.ToLower().Contains(term) ||
                                         p.PredictedBreedName.ToLower().Contains(term) ||
                                         (p.UserNotes != null && p.UserNotes.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(breedFilter))
            {
                query = query.Where(p => p.PredictedBreedName.ToLower() == breedFilter.Trim().ToLower());
            }

            if (!string.IsNullOrWhiteSpace(speciesFilter))
            {
                query = query.Where(p => p.Breed != null && p.Breed.Species.ToLower() == speciesFilter.Trim().ToLower());
            }

            if (lowConfidenceOnly.HasValue && lowConfidenceOnly.Value)
            {
                query = query.Where(p => p.IsLowConfidence);
            }

            var totalRecords = await query.CountAsync();
            var records = await query
                .OrderByDescending(p => p.PredictionTimestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var availableBreeds = await _context.Breeds
                .OrderBy(b => b.Name)
                .Select(b => b.Name)
                .ToListAsync();

            return new HistoryFilterViewModel
            {
                SearchTerm = searchTerm,
                BreedFilter = breedFilter,
                SpeciesFilter = speciesFilter,
                LowConfidenceOnly = lowConfidenceOnly,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Records = records,
                AvailableBreeds = availableBreeds
            };
        }

        public async Task<PredictionRecord?> GetPredictionByIdAsync(int id)
        {
            return await _context.PredictionRecords
                .Include(p => p.Breed)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> DeletePredictionAsync(int id)
        {
            var record = await _context.PredictionRecords.FindAsync(id);
            if (record == null) return false;

            // Delete associated saved image from disk
            if (!string.IsNullOrEmpty(record.SavedImagePath))
            {
                _storageService.DeleteImage(record.SavedImagePath);
            }

            _context.PredictionRecords.Remove(record);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted prediction record #{Id}", id);
            return true;
        }
    }
}
