using Agribus.Core.Domain.AggregatesModels.AlertAggregates;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Domain.Enums;
using Agribus.Postgres.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agribus.Postgres.EntityConfigurations;

public class AlertEntityTypeConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> alertConfiguration)
    {
        alertConfiguration.HasKey(a => a.Id);
        alertConfiguration.Property(a => a.Id);

        alertConfiguration.Property(a => a.Name).IsRequired();

        alertConfiguration.Property(a => a.MeasureType).HasConversion<string>().IsRequired();
        alertConfiguration.Property(a => a.RuleType).HasConversion<string>().IsRequired();
        alertConfiguration.Property(a => a.Enabled).HasDefaultValue(true);

        alertConfiguration.Property(x => x.ThresholdValue);
        alertConfiguration.Property(x => x.RangeMinValue);
        alertConfiguration.Property(x => x.RangeMaxValue);

        alertConfiguration
            .HasOne<Greenhouse>(a => a.Greenhouse)
            .WithMany(g => g.Alerts)
            .HasForeignKey(a => a.GreenhouseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        alertConfiguration.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_RuleType_IsValid",
                sql: $"rule_type IN ({EnumToSqlHelper.GetEnumValuesSql<AlertRuleType>()})"
            );

            t.HasCheckConstraint(
                "CK_MeasureType_IsValid",
                sql: $"measure_type IN ({EnumToSqlHelper.GetEnumValuesSql<SensorType>()})"
            );
        });
    }
}
