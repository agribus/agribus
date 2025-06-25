using Agribus.Core.Ports.Api.DTOs;
using Agribus.Core.Ports.Api.Validators;
using FluentValidation;

namespace Agribus.Core.Tests;

public class ParseSensorData : IParseSensorData
{
    private readonly RawSensorPayloadValidator _payloadValidator;

    public ParseSensorData(RawSensorPayloadValidator payloadValidator)
    {
        _payloadValidator = payloadValidator;
    }

    public async Task<bool> FromRawJson(
        RawSensorPayload payload,
        CancellationToken cancellationToken
    )
    {
        await _payloadValidator.ValidateAndThrowAsync(payload, cancellationToken);
        return await Task.FromResult(true);
    }
}
