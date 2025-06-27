using Agribus.Core.Domain.AggregatesModels;

namespace Agribus.Core.Ports.Spi.Measurement;

public interface IStoreMeasurement
{
    Task StoreAsync(SensorMeasurement measurement, CancellationToken cancellationToken = default);
}
