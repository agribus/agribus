using Agribus.Core.Domain.AggregatesModels.AlertAggregates;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agribus.Postgres.EntityConfigurations;

public class AlertEventsEntityTypeConfiguration : IEntityTypeConfiguration<AlertEvents>
{
    public void Configure(EntityTypeBuilder<AlertEvents> alertEvent)
    {
        alertEvent.HasKey(e => e.Id);
        alertEvent.Property(e => e.Id);

        alertEvent.Property(e => e.MeasureValue).IsRequired();
        alertEvent
            .Property(e => e.OccuredAt)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        alertEvent
            .HasOne(e => e.Alert)
            .WithMany(a => a.AlertEvents)
            .HasForeignKey(e => e.AlertId)
            .OnDelete(DeleteBehavior.Cascade);

        alertEvent
            .HasOne(e => e.Sensor)
            .WithMany(s => s.AlertEvents)
            .HasForeignKey(e => e.SensorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
