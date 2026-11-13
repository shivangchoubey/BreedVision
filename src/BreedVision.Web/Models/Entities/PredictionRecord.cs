using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BreedVision.Web.Models.Entities
{
    public class PredictionRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string SavedImagePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PredictedBreedName { get; set; } = string.Empty;

        public int? BreedId { get; set; }

        [ForeignKey(nameof(BreedId))]
        public Breed? Breed { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal ConfidenceScore { get; set; }

        public bool IsLowConfidence { get; set; }

        public string TopPredictionsJson { get; set; } = "[]";

        public DateTime PredictionTimestamp { get; set; } = DateTime.UtcNow;

        public long InferenceTimeMs { get; set; }

        [MaxLength(500)]
        public string? UserNotes { get; set; }

        [MaxLength(50)]
        public string? ClientIp { get; set; }
    }
}
