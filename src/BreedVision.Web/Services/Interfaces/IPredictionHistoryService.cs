using BreedVision.Web.Models.Entities;
using BreedVision.Web.Models.ViewModels;

namespace BreedVision.Web.Services.Interfaces
{
    public interface IPredictionHistoryService
    {
        Task<PredictionRecord> SavePredictionAsync(PredictionRecord record);
        Task<HistoryFilterViewModel> GetHistoryAsync(
            string? searchTerm, 
            string? breedFilter, 
            string? speciesFilter, 
            bool? lowConfidenceOnly, 
            int page, 
            int pageSize);
        Task<PredictionRecord?> GetPredictionByIdAsync(int id);
        Task<bool> DeletePredictionAsync(int id);
    }
}

