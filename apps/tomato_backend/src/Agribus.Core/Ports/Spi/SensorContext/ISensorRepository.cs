using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;

namespace Agribus.Core.Ports.Spi.SensorContext;

public interface ISensorRepository
{
    Task<Sensor?> Exists(Guid sensorId, string userId, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Sensor sensor, UpdateSensorDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Sensor sensor, CancellationToken cancellationToken);
    Task<bool> IsRegistered(string sourceAddress, CancellationToken cancellationToken);
    Task<Sensor[]> GetByGreenhouseIdAsync(
        Guid greenhouseId,
        string userId,
        CancellationToken cancellationToken
    );
}
