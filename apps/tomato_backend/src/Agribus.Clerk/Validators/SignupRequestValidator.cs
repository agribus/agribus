using Agribus.Core.Domain.AggregatesModels.AuthAggregates;
using FluentValidation;

namespace Agribus.Clerk.Validators;

public class SignupRequestValidator : AbstractValidator<SignupRequest>
{
    public SignupRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(4)
            .WithMessage("The username must be at least 4 characters long");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .Must(BeValidEmail)
            .WithMessage("Invalid email address");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("The password must be at least 8 characters long");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match");
    }

    private bool BeValidEmail(string email)
    {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
    }
}
