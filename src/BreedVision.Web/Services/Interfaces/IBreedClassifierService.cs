using BreedVision.Web.Models.ViewModels;

namespace BreedVision.Web.Services.Interfaces
{
    public class BreedClassificationResult
    {
        public string TopBreedName { get; set; } = string.Empty;
        public double ConfidencePercent { get; set; }
        public bool IsLowConfidence { get; set; }
        public List<ClassPrediction> TopPredictions { get; set; } = new();
        public long InferenceDurationMs { get; set; }
    }

    public interface IBreedClassifierService
    {
        Task<BreedClassificationResult> ClassifyImageAsync(Stream imageStream, string fileName);
        IReadOnlyList<string> SupportedBreeds { get; }
        double LowConfidenceThreshold { get; }
    }
}
