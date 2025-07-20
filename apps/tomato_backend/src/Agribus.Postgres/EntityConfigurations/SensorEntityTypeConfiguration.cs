using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SensorEntityTypeConfiguration : IEntityTypeConfiguration<Sensor>
{
    public void Configure(EntityTypeBuilder<Sensor> sensorConfiguration)
    {
        sensorConfiguration.ToTable(t =>
        {
            t.HasComment("Sensors table stores information about sensors used in the system.");
            t.HasCheckConstraint(
                "CK_Sensor_Model_IsValid",
                sql: $"sensor_model IN ('{string.Join("', '", Enum.GetNames(typeof(SensorModel)))}')"
            );
        });

        sensorConfiguration.HasKey(sensor => sensor.Id);
        sensorConfiguration.Property(sensor => sensor.Id);

        sensorConfiguration.Property(sensor => sensor.Name).IsRequired();
        sensorConfiguration
            .Property(sensor => sensor.SourceAddress)
            .IsRequired()
            .HasColumnType("varchar(50)");
        sensorConfiguration
            .Property(sensor => sensor.Model)
            .IsRequired()
            .HasColumnType("varchar(50)");
        sensorConfiguration.Property(sensor => sensor.IsActive).IsRequired().HasDefaultValue(true);
        sensorConfiguration
            .HasOne(sensor => sensor.Greenhouse)
            .WithMany(g => g.Sensors)
            .HasForeignKey(sensor => sensor.GreenhouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
