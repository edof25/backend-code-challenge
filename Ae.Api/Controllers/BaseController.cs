using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Ae.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Gets the current authenticated username from the claims or returns "System" as default
    /// </summary>
    protected string GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
    }
}
