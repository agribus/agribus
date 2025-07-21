using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Api.SensorUsecases.DTOs;

namespace Agribus.Core.Ports.Spi.SensorContext;

public interface ISensorRepository
{
    Task<Sensor?> Exists(Guid originalSensorId, Guid userId, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Sensor sensor, UpdateSensorDto dto, CancellationToken cancellationToken);
}
