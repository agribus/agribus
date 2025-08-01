using System.Text.Json;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GreenhouseEntityTypeConfiguration : IEntityTypeConfiguration<Greenhouse>
{
    public void Configure(EntityTypeBuilder<Greenhouse> greenhouseConfiguration)
    {
        greenhouseConfiguration.ToTable(t =>
        {
            t.HasComment("Greenhouses table stores information about greenhouse facilities.");
        });

        greenhouseConfiguration.HasKey(g => g.Id);
        greenhouseConfiguration.Property(g => g.Id);

        greenhouseConfiguration.Property(g => g.Name).IsRequired().HasColumnType("varchar(100)");

        greenhouseConfiguration.Property(g => g.Country).IsRequired().HasColumnType("varchar(100)");

        greenhouseConfiguration.Property(g => g.City).IsRequired().HasColumnType("varchar(100)");

        greenhouseConfiguration
            .Property(g => g.UserId)
            .IsRequired()
            .HasComment("user table is currently stored in an external database (Clerk)");

        greenhouseConfiguration
            .HasMany(g => g.Sensors)
            .WithOne()
            .HasForeignKey(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);

        greenhouseConfiguration
            .Property(g => g.Crops)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v =>
                    JsonSerializer.Deserialize<List<Crop>>(v, JsonSerializerOptions.Default)
                    ?? new List<Crop>(),
                new ValueComparer<List<Crop>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                )
            );
    }
}
