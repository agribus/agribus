using Agribus.Core.Ports.Api.DTOs;

namespace Agribus.Core.Tests;

public interface IParseSensorData
{
    public Task<bool> FromRawJson(RawSensorPayload payload, CancellationToken cancellationToken);
}
