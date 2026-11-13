using BreedVision.Web.Models.Entities;

namespace BreedVision.Web.Models.ViewModels
{
    public class ClassPrediction
    {
        public string BreedName { get; set; } = string.Empty;
        public double ConfidencePercent { get; set; }
        public int Rank { get; set; }
        public string Species { get; set; } = string.Empty;
    }

    public class PredictionResultViewModel
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public string PredictedBreedName { get; set; } = string.Empty;
        public double ConfidencePercent { get; set; }
        public bool IsLowConfidence { get; set; }

        public Breed? BreedProfile { get; set; }
        public List<ClassPrediction> TopPredictions { get; set; } = new();

        public long InferenceTimeMs { get; set; }
        public int? PredictionRecordId { get; set; }
        public string? UserNotes { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class ImageUploadViewModel
    {
        public IFormFile? ImageFile { get; set; }
        public string? Notes { get; set; }
    }

    public class BreedDetailViewModel
    {
        public Breed Breed { get; set; } = null!;
        public List<PredictionRecord> RecentPredictions { get; set; } = new();
        public int TotalPredictionsCount { get; set; }
        public double AverageConfidence { get; set; }
    }

    public class HistoryFilterViewModel
    {
        public string? SearchTerm { get; set; }
        public string? BreedFilter { get; set; }
        public string? SpeciesFilter { get; set; }
        public bool? LowConfidenceOnly { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        public List<PredictionRecord> Records { get; set; } = new();
        public List<string> AvailableBreeds { get; set; } = new();
    }
}
