using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;
using Agribus.Core.Ports.Spi.SensorContext;

namespace Agribus.Postgres.Persistence.SensorContext;

internal class SensorRepository(AgribusDbContext context) : ISensorRepository
{
    public async Task<Sensor?> Exists(
        Guid originalSensorId,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var sensor = await context.Sensor.FirstOrDefaultAsync(
            s => s.Id == originalSensorId,
            cancellationToken
        );
        // TODO check if sensor belongs to a greenhouse of the user
        return sensor;
    }

    public async Task<bool> UpdateAsync(
        Sensor sensor,
        UpdateSensorDto dto,
        CancellationToken cancellationToken
    )
    {
        sensor.Update(dto);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
