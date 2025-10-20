using System.Text.RegularExpressions;
using FluentValidation;

namespace Ae.Domain.Validators.Custom;

public static class PasswordComplexityValidator
{
    public static IRuleBuilderOptions<T, string> MustHavePasswordComplexity<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long");
    }
}
