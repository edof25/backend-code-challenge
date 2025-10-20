using Ae.Domain.DTOs.Ship;
using Ae.Infrastructure.Interfaces;
using FluentValidation;

namespace Ae.Infrastructure.Validators.Ship;

public class CreateShipRequestValidator : AbstractValidator<CreateShipRequest>
{
    public CreateShipRequestValidator(IShipRepository shipRepository)
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Ship code is required")
            .Length(3, 20)
            .WithMessage("Ship code must be between 3 and 20 characters")
            .MustBeUniqueShipCode(shipRepository);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Ship name is required");

        RuleFor(x => x.FiscalYear)
            .NotEmpty()
            .WithMessage("Fiscal year is required")
            .Matches(@"^\d{4}$")
            .WithMessage("Fiscal year must be in number format");
    }
}
