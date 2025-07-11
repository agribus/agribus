using System.Text.Json;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
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

        // Configure the one-to-many relationship with Sensors
        greenhouseConfiguration
            .HasMany(g => g.Sensors)
            .WithOne()
            .HasForeignKey(s => s.Id) // Assuming Sensor has GreenhouseId property
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the one-to-many relationship with Crops
        greenhouseConfiguration
            .Property(g => g.Crops)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v =>
                    JsonSerializer.Deserialize<List<Crop>>(v, JsonSerializerOptions.Default)
                    ?? new List<Crop>()
            );

        // Uncomment to configure the User relationship
        /*
        greenhouseConfiguration.HasOne(g => g.User)
            .WithMany()
            .HasForeignKey(g => g.UserId)
            .IsRequired(false);
        */
    }
}
