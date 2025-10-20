using System.Security.Claims;
using Ae.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ae.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    private IValidationService? _validationService;

    /// <summary>
    /// Gets the ValidationService instance from DI
    /// </summary>
    protected IValidationService ValidationService
    {
        get
        {
            _validationService ??= HttpContext.RequestServices.GetRequiredService<IValidationService>();
            return _validationService;
        }
    }

    /// <summary>
    /// Gets the current authenticated username from the claims or returns "System" as default
    /// </summary>
    protected string GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
    }

    /// <summary>
    /// Validates a request asynchronously and returns a BadRequest if validation fails
    /// </summary>
    /// <typeparam name="T">The type of the request to validate</typeparam>
    /// <param name="request">The request object to validate</param>
    /// <returns>BadRequest with validation errors if validation fails, null if validation succeeds</returns>
    protected async Task<IActionResult?> ValidateAsync<T>(T request)
    {
        return await ValidationService.ValidateAsync(request);
    }
}
