using Agribus.Core.Domain.AggregatesModels.SensorAggregates;

namespace Agribus.Core.Ports.Spi.Measurement;

public interface IStoreMeasurement
{
    Task StoreAsync(SensorMeasurement measurement, CancellationToken cancellationToken = default);
    Task<List<SensorMeasurement>> GetMeasurementsAsync(
        List<string> sourceAddresses,
        CancellationToken cancellationToken = default
    );
}
