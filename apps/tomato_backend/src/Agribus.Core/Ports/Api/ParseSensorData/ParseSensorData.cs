using Agribus.Core.Domain.AggregatesModels.SensorAggregates;
using Agribus.Core.Domain.Enums;
using Agribus.Core.Ports.Api.DTOs;
using Agribus.Core.Ports.Api.Validators;
using FluentValidation;

namespace Agribus.Core.Ports.Api.ParseSensorData;

public class ParseSensorData(RawSensorPayloadValidator validator) : IParseSensorData
{
    public async Task<SensorMeasurement> FromRawJson(
        RawSensorPayload payload,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(payload, cancellationToken);
        var sensorType = MapType(payload.Type);
        var date = DateTimeOffset.FromUnixTimeMilliseconds(payload.Timestamp).UtcDateTime;
        var measurement = new SensorMeasurement
        {
            Date = date,
            Value = payload.Value,
            Type = sensorType,
            SourceAdress = payload.SourceAddress,
        };

        return measurement;
    }

    private SensorType MapType(string type) =>
        type.ToLower() switch
        {
            "temperature" => SensorType.Temperature,
            "humidity" => SensorType.Humidity,
            "pressure" => SensorType.Pressure,
            "motion" => SensorType.Motion,
            "rssi" => SensorType.Rssi,
            "neighbors" => SensorType.Neighbors,
            _ => SensorType.Unknown,
        };
}
