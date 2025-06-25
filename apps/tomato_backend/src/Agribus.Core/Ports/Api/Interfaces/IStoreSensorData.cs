namespace Agribus.Core.Tests;

public interface IStoreSensorData
{
    public Task StoreValidatedData(CancellationToken cancellationToken);
}
