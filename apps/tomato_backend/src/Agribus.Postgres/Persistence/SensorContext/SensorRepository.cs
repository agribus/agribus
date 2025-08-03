using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;
using Agribus.Core.Ports.Spi.SensorContext;

namespace Agribus.Postgres.Persistence.SensorContext;

internal class SensorRepository(AgribusDbContext context) : ISensorRepository
{
    public async Task<Sensor?> Exists(
        Guid sensorId,
        string userId,
        CancellationToken cancellationToken
    )
    {
        var sensor = await context.Sensor.FirstOrDefaultAsync(
            s => s.Id == sensorId && s.Greenhouse.UserId == userId,
            cancellationToken
        );
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

    public async Task<bool> DeleteAsync(Sensor sensor, CancellationToken cancellationToken)
    {
        context.Sensor.Remove(sensor);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> IsRegistered(string sourceAddress, CancellationToken cancellationToken)
    {
        return await context.Sensor.FirstOrDefaultAsync(
            s => s.SourceAddress == sourceAddress,
            cancellationToken
        );
    }
}
