using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Domain.Enums;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;

namespace Agribus.InfluxDB.Mapping;

public static class SensorMeasurementMapper
{
    public static PointData ToPointData(this SensorMeasurement measurement)
    {
        return PointData
            .Measurement(measurement.Type.ToString())
            .Tag("source_address", measurement.SourceAdress)
            .Field("value", measurement.Value)
            .Timestamp(measurement.Date, WritePrecision.Ns);
    }

    public static SensorMeasurement ToSensorMeasurement(this FluxRecord record)
    {
        return new SensorMeasurement
        {
            Date = record.GetTime()?.ToDateTimeUtc() ?? DateTime.UtcNow,
            Value = Convert.ToDouble(record.GetValue()),
            Type = Enum.Parse<SensorType>(record.GetMeasurement()),
            SourceAdress = record.GetValueByKey("source_address")?.ToString() ?? string.Empty,
        };
    }
}
