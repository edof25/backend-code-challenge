using Microsoft.AspNetCore.Mvc;

namespace Ae.Api.Services;

/// <summary>
/// Service for handling FluentValidation asynchronously
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validates a request asynchronously and returns a BadRequest if validation fails
    /// </summary>
    /// <typeparam name="T">The type of the request to validate</typeparam>
    /// <param name="request">The request object to validate</param>
    /// <returns>BadRequest with validation errors if validation fails, null if validation succeeds</returns>
    Task<IActionResult?> ValidateAsync<T>(T request);
}
