using Agribus.Core.Domain.AggregatesModels.AuthAggregates;
using FluentValidation;

namespace Agribus.Clerk.Validators;

public class PasswordChangeRequestValidator : AbstractValidator<PasswordChangeRequest>
{
    public PasswordChangeRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required")
            .MinimumLength(8)
            .WithMessage("The password must be at least 8 characters long");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("The password must be at least 8 characters long");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match");
    }
}
