using Agribus.Core.Domain.AggregatesModels;
using Agribus.Core.Domain.AggregatesModels.GreenhouseAggregates;
using Agribus.Core.Domain.AggregatesModels.SensorAggregates;

namespace Agribus.Postgres.Persistence;

public class AgribusDbContext(DbContextOptions<AgribusDbContext> options) : DbContext(options)
{
    public DbSet<Sensor> Sensor { get; set; }
    public DbSet<Greenhouse> Greenhouse { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgribusDbContext).Assembly);

        foreach (
            var entityType in modelBuilder
                .Model.GetEntityTypes()
                .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType))
        )
        {
            modelBuilder
                .Entity(entityType.ClrType)
                .Property(nameof(BaseEntity.Id))
                .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder
                .Entity(entityType.ClrType)
                .Property(nameof(BaseEntity.CreatedAt))
                .HasDefaultValueSql("NOW()");
            modelBuilder
                .Entity(entityType.ClrType)
                .Property(nameof(BaseEntity.LastModified))
                .HasDefaultValueSql("NOW()")
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}
