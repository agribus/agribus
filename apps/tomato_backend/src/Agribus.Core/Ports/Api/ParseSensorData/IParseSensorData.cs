using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Api.DTOs;

namespace Agribus.Core.Ports.Api.ParseSensorData;

public interface IParseSensorData
{
    public Task<SensorMeasurement> FromRawJson(
        RawSensorPayload payload,
        CancellationToken cancellationToken = default
    );
}
