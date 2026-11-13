using System.ComponentModel.DataAnnotations;

namespace BreedVision.Web.Models.Entities
{
    public class Breed
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string NormalizedKey { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Species { get; set; } = string.Empty; // "Cattle" or "Buffalo"

        [MaxLength(100)]
        public string ScientificName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string OriginState { get; set; } = string.Empty;

        [MaxLength(100)]
        public string PrimaryUtility { get; set; } = string.Empty; // "Dairy", "Draught", "Dual-Purpose"

        [MaxLength(150)]
        public string AverageMilkYield { get; set; } = string.Empty;

        [MaxLength(500)]
        public string HornCharacteristics { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string PhysicalTraits { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ConservationStatus { get; set; } = "Indigenous (ICAR-NBAGR Registered)";

        [MaxLength(255)]
        public string ReferenceImageUrl { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<PredictionRecord> PredictionRecords { get; set; } = new List<PredictionRecord>();
    }
}
