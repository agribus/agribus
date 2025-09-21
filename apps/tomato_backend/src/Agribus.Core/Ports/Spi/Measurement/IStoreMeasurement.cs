using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Ports.Api.ParseSensorData.DTOs;

namespace Agribus.Core.Ports.Spi.Measurement;

public interface IStoreMeasurement
{
    Task StoreAsync(SensorMeasurement measurement, CancellationToken cancellationToken = default);
    Task<LatestMeasurementsResponseDto> GetMeasurementsAsync(
        List<Sensor> sensors,
        CancellationToken cancellationToken = default
    );

    Task<ChartTimeseriesResponseDto> GetChartTimeseriesAsync(
        List<Sensor> sensors,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default
    );
}
