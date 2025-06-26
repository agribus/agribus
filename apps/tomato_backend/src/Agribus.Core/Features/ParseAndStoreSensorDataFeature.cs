using Agribus.Core.Domain.AggregatesModels;
using Agribus.Core.Enums;
using Agribus.Core.Ports.Api.DTOs;
using Agribus.Core.Ports.Api.Interfaces;
using Agribus.Core.Ports.Api.Validators;
using FluentValidation;

namespace Agribus.Core.Features;

public class ParseAndStoreSensorDataFeature(RawSensorPayloadValidator validator)
    : IParseSensorData,
        IStoreSensorData
{
    public async Task<SensorMeasurement> FromRawJson(
        RawSensorPayload payload,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(payload, cancellationToken);
        var measurement = new SensorMeasurement
        {
            Date = DateTimeOffset.FromUnixTimeMilliseconds(payload.Timestamp).DateTime,
            Value = payload.Value,
            Type = (SensorType)Enum.Parse(typeof(SensorType), payload.Type),
            SourceAdress = payload.SourceAdress,
        };

        return measurement;
    }

    public Task<bool> StoreValidatedData(
        SensorMeasurement measurement,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
