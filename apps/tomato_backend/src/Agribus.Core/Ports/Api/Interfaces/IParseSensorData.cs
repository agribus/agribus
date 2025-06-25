using Agribus.Core.Ports.Api.DTOs;

namespace Agribus.Core.Ports.Api.Interfaces;

public interface IParseSensorData
{
    public Task<bool> FromRawJson(RawSensorPayload payload, CancellationToken cancellationToken);
}
