using Agribus.Core.Enums;
using Agribus.Core.Ports.Api.DTOs;
using FluentValidation;

namespace Agribus.Core.Ports.Api.Validators;

public class RawSensorPayloadValidator : AbstractValidator<RawSensorPayload>
{
    public RawSensorPayloadValidator()
    {
        RuleFor(rp => rp.SensorId).NotEmpty();
        RuleFor(rp => rp.Type)
            .NotEmpty()
            .Must(BeValidSensorType)
            .WithMessage("'{PropertyName}' must be a valid SensorType value. Temperature; Humidity; Pressure; Motion");
        RuleFor(rp => rp.Value)
            .NotEmpty();
        RuleFor(rp => rp.Timestamp).NotEmpty();
    }

    private bool BeValidSensorType(string arg)
    {
        if (string.IsNullOrWhiteSpace(arg))
            return false;

        return Enum.TryParse<SensorType>(arg, true, out _);
    }

    private void ValidateValueBasedOnType(
        SensorType payloadType,
        double value,
        ValidationContext<RawSensorPayload> context
    )
    {
        switch (payloadType)
        {
            case SensorType.Motion:
                Console.WriteLine("validation motion");
                break;
            default:
                break;
        }
    }
}
