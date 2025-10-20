using Ae.Domain.DTOs;
using Ae.Infrastructure.Interfaces;
using FluentValidation;

namespace Ae.Infrastructure.Validators.User;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required");

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .WithMessage("Birth date is required");

        RuleFor(x => x.Nationality)
            .NotEmpty()
            .WithMessage("Nationality is required");

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role is required");

        When(x => !string.IsNullOrWhiteSpace(x.CrewMemberId), () =>
        {
            RuleFor(x => x.CrewMemberId)
                .Length(5, 50)
                .WithMessage("Crew Member ID must be between 5 and 50 characters");
        });
    }
}
