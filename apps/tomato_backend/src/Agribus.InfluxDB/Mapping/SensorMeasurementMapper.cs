using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Domain.Enums;
using InfluxDB3.Client.Write;

namespace Agribus.InfluxDB.Mapping;

public static class SensorMeasurementMapper
{
    public static PointData ToPointData(this SensorMeasurement measurement)
    {
        return PointData
            .Measurement(measurement.Type.ToString())
            .SetTag("source_address", measurement.SourceAddress)
            .SetField("value", measurement.Value)
            .SetTimestamp(measurement.Date, WritePrecision.Ns);
    }

    // public static SensorMeasurement ToSensorMeasurement(this FluxRecord record)
    // {
    //     return new SensorMeasurement
    //     {
    //         Date = record.GetTime()?.ToDateTimeUtc() ?? DateTime.UtcNow,
    //         Value = Convert.ToDouble(record.GetValue()),
    //         Type = Enum.Parse<SensorType>(record.GetMeasurement()),
    //         SourceAddress = record.GetValueByKey("source_address")?.ToString() ?? string.Empty,
    //     };
    // }
}
