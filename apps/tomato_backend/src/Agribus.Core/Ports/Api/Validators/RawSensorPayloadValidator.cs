using Agribus.Core.Domain.Enums;
using Agribus.Core.Ports.Api.ParseSensorData.DTOs;
using FluentValidation;

namespace Agribus.Core.Ports.Api.Validators;

public class RawSensorPayloadValidator : AbstractValidator<RawSensorPayload>
{
    public RawSensorPayloadValidator()
    {
        RuleFor(rp => rp.SourceAddress).NotEmpty();
        RuleFor(rp => rp.Type)
            .NotEmpty()
            .Must(BeValidSensorType)
            .WithMessage(
                "'{PropertyName}' must be a valid SensorType value. Temperature; Humidity; Pressure; Motion"
            );
        RuleFor(rp => rp.Value).NotEmpty();
        RuleFor(rp => rp.Timestamp).NotEmpty();
    }

    private bool BeValidSensorTimestamp(string arg)
    {
        return long.TryParse(arg, out long timestamp);
    }

    private bool BeValidSensorType(string arg)
    {
        return Enum.TryParse<SensorType>(arg, true, out _);
    }
}
