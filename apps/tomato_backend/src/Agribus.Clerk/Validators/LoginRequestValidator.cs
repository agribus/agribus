using Agribus.Core.Domain.AggregatesModels.AuthAggregates;
using FluentValidation;

namespace Agribus.Clerk.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email address");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}
