using Agribus.Core.Domain.AggregatesModels.AlertAggregates;
using Agribus.Core.Domain.Enums;
using Agribus.Core.Ports.Api.AlertUsecases.DTOs;
using FluentValidation;

namespace Agribus.Core.Ports.Api.AlertUsecases.Validators;

public class AlertValidator : AbstractValidator<Alert>
{
    public AlertValidator()
    {
        RuleFor(x => x.GreenhouseId).NotEmpty();

        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.MeasureType).IsInEnum();

        RuleFor(x => x.RuleType).IsInEnum();

        When(
            x => x.RuleType == AlertRuleType.Above || x.RuleType == AlertRuleType.Below,
            () =>
            {
                RuleFor(x => x.ThresholdValue)
                    .NotNull()
                    .WithMessage("ThresholdValue is required for Above/BelowThreshold rules.");
            }
        );

        When(
            x => x.RuleType == AlertRuleType.Outside || x.RuleType == AlertRuleType.Inside,
            () =>
            {
                RuleFor(x => x.RangeMinValue)
                    .NotNull()
                    .WithMessage("RangeMinValue is required for range rules.");

                RuleFor(x => x.RangeMaxValue)
                    .NotNull()
                    .WithMessage("RangeMaxValue is required for range rules.");

                RuleFor(x => x)
                    .Must(dto => dto.RangeMinValue < dto.RangeMaxValue)
                    .WithMessage("RangeMinValue must be strictly less than RangeMaxValue.");
            }
        );
    }
}
