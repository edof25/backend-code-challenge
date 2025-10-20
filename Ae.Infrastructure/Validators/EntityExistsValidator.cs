using Ae.Infrastructure.Interfaces;
using FluentValidation;
using FluentValidation.Validators;

namespace Ae.Infrastructure.Validators;

public class UserExistsValidator<T> : AsyncPropertyValidator<T, int>
{
    private readonly IUserRepository _userRepository;

    public UserExistsValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override string Name => "UserExistsValidator";

    public override async Task<bool> IsValidAsync(ValidationContext<T> context, int userId, CancellationToken cancellation)
    {
        if (userId <= 0)
            return true; // Let the GreaterThan validator handle this

        var user = await _userRepository.GetByIdAsync(userId);
        return user != null;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "User with ID '{PropertyValue}' does not exist";
    }
}

public class ShipExistsValidator<T> : AsyncPropertyValidator<T, int>
{
    private readonly IShipRepository _shipRepository;

    public ShipExistsValidator(IShipRepository shipRepository)
    {
        _shipRepository = shipRepository;
    }

    public override string Name => "ShipExistsValidator";

    public override async Task<bool> IsValidAsync(ValidationContext<T> context, int shipId, CancellationToken cancellation)
    {
        if (shipId <= 0)
            return true; // Let the GreaterThan validator handle this

        var ship = await _shipRepository.GetByIdAsync(shipId);
        return ship != null;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "Ship with ID '{PropertyValue}' does not exist";
    }
}

public static class EntityExistsValidatorExtensions
{
    public static IRuleBuilderOptions<T, int> MustExistAsUser<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        IUserRepository userRepository)
    {
        return ruleBuilder.SetAsyncValidator(new UserExistsValidator<T>(userRepository));
    }

    public static IRuleBuilderOptions<T, int> MustExistAsShip<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        IShipRepository shipRepository)
    {
        return ruleBuilder.SetAsyncValidator(new ShipExistsValidator<T>(shipRepository));
    }
}
