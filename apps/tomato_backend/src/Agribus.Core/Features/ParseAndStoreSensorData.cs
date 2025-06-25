using Agribus.Core.Ports.Api.DTOs;

namespace Agribus.Core.Tests;

public class ParseAndStoreSensorData : IParseAndStoreSensorData
{
    
    public Task<bool> FromRawJson(RawSensorPayload payload, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StoreValidatedData(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public interface IParseAndStoreSensorData
{
    public Task<bool> FromRawJson(RawSensorPayload payload, CancellationToken cancellationToken);
    public Task StoreValidatedData(CancellationToken cancellationToken);
}