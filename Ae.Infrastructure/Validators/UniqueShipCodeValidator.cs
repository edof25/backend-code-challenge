using Ae.Infrastructure.Interfaces;
using FluentValidation;
using FluentValidation.Validators;

namespace Ae.Infrastructure.Validators;

public class UniqueShipCodeValidator<T> : AsyncPropertyValidator<T, string>
{
    private readonly IShipRepository _shipRepository;
    private readonly int? _excludeShipId;

    public UniqueShipCodeValidator(IShipRepository shipRepository, int? excludeShipId = null)
    {
        _shipRepository = shipRepository;
        _excludeShipId = excludeShipId;
    }

    public override string Name => "UniqueShipCodeValidator";

    public override async Task<bool> IsValidAsync(ValidationContext<T> context, string code, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(code))
            return true; // Let the Required validator handle this

        var existingShip = await _shipRepository.GetByCodeAsync(code);

        if (existingShip == null)
            return true;

        // If we're updating a ship, allow the same code if it belongs to the ship being updated
        if (_excludeShipId.HasValue && existingShip.Id == _excludeShipId.Value)
            return true;

        return false;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "Ship code '{PropertyValue}' is already taken";
    }
}

public static class UniqueShipCodeValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeUniqueShipCode<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IShipRepository shipRepository,
        int? excludeShipId = null)
    {
        return ruleBuilder.SetAsyncValidator(new UniqueShipCodeValidator<T>(shipRepository, excludeShipId));
    }
}
