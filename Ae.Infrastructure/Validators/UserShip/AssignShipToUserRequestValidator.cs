using Ae.Domain.DTOs.UserShip;
using Ae.Infrastructure.Interfaces;
using FluentValidation;

namespace Ae.Infrastructure.Validators.UserShip;

public class AssignShipToUserRequestValidator : AbstractValidator<AssignShipToUserRequest>
{
    public AssignShipToUserRequestValidator(IUserRepository userRepository, IShipRepository shipRepository)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required")
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0")
            .MustExistAsUser(userRepository);

        RuleFor(x => x.ShipId)
            .NotEmpty()
            .WithMessage("Ship ID is required")
            .GreaterThan(0)
            .WithMessage("Ship ID must be greater than 0")
            .MustExistAsShip(shipRepository);

        RuleFor(x => x.RankId)
            .NotEmpty()
            .WithMessage("Rank ID is required")
            .GreaterThan((byte)0)
            .WithMessage("Rank ID must be greater than 0");

        RuleFor(x => x.SignOnDate)
            .NotEmpty()
            .WithMessage("Sign-on date is required");

        RuleFor(x => x.EndOfContractDate)
            .NotEmpty()
            .WithMessage("End of contract date is required");

        When(x => x.SignOffDate.HasValue, () =>
        {
            RuleFor(x => x.SignOffDate!.Value)
                .GreaterThanOrEqualTo(x => x.SignOnDate)
                .WithMessage("Sign-off date must be on or after sign-on date");
        });
    }
}
