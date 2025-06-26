using Agribus.Core.Domain.AggregatesModels;
using Agribus.Core.Ports.Api.DTOs;

namespace Agribus.Core.Ports.Api.Interfaces;

public interface IParseSensorData
{
    public Task<SensorMeasurement> FromRawJson(
        RawSensorPayload payload,
        CancellationToken cancellationToken
    );
}
