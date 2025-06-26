using Agribus.Core.Domain.AggregatesModels;
using InfluxDB.Client.Api.Domain;
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
}
