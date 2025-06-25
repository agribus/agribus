using Agribus.Core.Ports.Api.DTOs;
using Agribus.Core.Ports.Api.Interfaces;
using Agribus.Core.Ports.Api.Validators;
using Agribus.Core.Tests;
using FluentValidation;

namespace Agribus.Core.Features;

public class ParseAndStoreSensorDataFeature(RawSensorPayloadValidator validator) : IParseSensorData, IStoreSensorData
{
    public async Task<bool> FromRawJson(
        RawSensorPayload payload,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(payload, cancellationToken);
        return await Task.FromResult(true);
    }

    public Task StoreValidatedData(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
