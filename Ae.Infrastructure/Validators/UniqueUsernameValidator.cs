using Ae.Infrastructure.Interfaces;
using FluentValidation;
using FluentValidation.Validators;

namespace Ae.Infrastructure.Validators;

public class UniqueUsernameValidator<T> : AsyncPropertyValidator<T, string>
{
    private readonly IUserRepository _userRepository;
    private readonly int? _excludeUserId;

    public UniqueUsernameValidator(IUserRepository userRepository, int? excludeUserId = null)
    {
        _userRepository = userRepository;
        _excludeUserId = excludeUserId;
    }

    public override string Name => "UniqueUsernameValidator";

    public override async Task<bool> IsValidAsync(ValidationContext<T> context, string username, CancellationToken cancellation)
    {
        if (string.IsNullOrWhiteSpace(username))
            return true; // Let the Required validator handle this

        var existingUser = await _userRepository.GetByUsernameAsync(username);

        if (existingUser == null)
            return true;

        // If we're updating a user, allow the same username if it belongs to the user being updated
        if (_excludeUserId.HasValue && existingUser.Id == _excludeUserId.Value)
            return true;

        return false;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "Username '{PropertyValue}' is already taken";
    }
}

public static class UniqueUsernameValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeUniqueUsername<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IUserRepository userRepository,
        int? excludeUserId = null)
    {
        return ruleBuilder.SetAsyncValidator(new UniqueUsernameValidator<T>(userRepository, excludeUserId));
    }
}
