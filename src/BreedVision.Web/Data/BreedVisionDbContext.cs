using BreedVision.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BreedVision.Web.Data
{
    public class BreedVisionDbContext : DbContext
    {
        public BreedVisionDbContext(DbContextOptions<BreedVisionDbContext> options)
            : base(options)
        {
        }

        public DbSet<Breed> Breeds => Set<Breed>();
        public DbSet<PredictionRecord> PredictionRecords => Set<PredictionRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Breed>(entity =>
            {
                entity.HasIndex(b => b.Name).IsUnique();
                entity.HasIndex(b => b.NormalizedKey).IsUnique();
                entity.HasIndex(b => b.Species);

                entity.Property(b => b.Name).HasMaxLength(100).IsRequired();
                entity.Property(b => b.Species).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<PredictionRecord>(entity =>
            {
                entity.HasIndex(p => p.PredictionTimestamp);
                entity.HasIndex(p => p.PredictedBreedName);

                entity.HasOne(p => p.Breed)
                      .WithMany(b => b.PredictionRecords)
                      .HasForeignKey(p => p.BreedId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
