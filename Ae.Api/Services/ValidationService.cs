using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Ae.Api.Services;

/// <summary>
/// Service for handling FluentValidation asynchronously
/// </summary>
public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Validates a request asynchronously and returns a BadRequest if validation fails
    /// </summary>
    /// <typeparam name="T">The type of the request to validate</typeparam>
    /// <param name="request">The request object to validate</param>
    /// <returns>BadRequest with validation errors if validation fails, null if validation succeeds</returns>
    public async Task<IActionResult?> ValidateAsync<T>(T request)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();

        // If no validator is registered for this type, skip validation
        if (validator == null)
        {
            return null;
        }

        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new
            {
                field = e.PropertyName,
                message = e.ErrorMessage
            });

            return new BadRequestObjectResult(new { errors });
        }

        return null;
    }
}
