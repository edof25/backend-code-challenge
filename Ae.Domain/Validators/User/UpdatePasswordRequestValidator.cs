using Ae.Domain.DTOs;
using Ae.Domain.Validators.Custom;
using FluentValidation;

namespace Ae.Domain.Validators.User;

public class UpdatePasswordRequestValidator : AbstractValidator<UpdatePasswordRequest>
{
    public UpdatePasswordRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MustHavePasswordComplexity();
    }
}
