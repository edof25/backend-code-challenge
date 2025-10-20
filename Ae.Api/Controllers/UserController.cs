using Ae.Domain.DTOs;
using Ae.Domain.DTOs.Common;
using Ae.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ae.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Validate request
        var validationError = await ValidateAsync(request);
        if (validationError != null) return validationError;

        var response = await _userService.LoginAsync(request);

        if (response == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        return Ok(response);
    }

    /// <summary>
    /// Get all users with pagination, sorting, and search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term to filter by username, first name, last name, or nationality</param>
    /// <param name="sortBy">Sort by column: Username, FirstName, LastName, RoleName, Nationality, BirthDate, CreatedAt</param>
    /// <param name="sortOrder">Sort order: ASC or DESC (default: ASC)</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "ASC")
    {
        var request = new PaginationRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortOrder = sortOrder ?? "ASC"
        };

        var pagedResult = await _userService.GetAllUsersAsync(request);
        return Ok(pagedResult);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(user);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        // Validate request
        var validationError = await ValidateAsync(request);
        if (validationError != null) return validationError;

        try
        {
            var createdBy = GetCurrentUsername();

            var user = await _userService.CreateUserAsync(request, createdBy);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        // Validate request
        var validationError = await ValidateAsync(request);
        if (validationError != null) return validationError;

        try
        {
            var updatedBy = GetCurrentUsername();

            var user = await _userService.UpdateUserAsync(id, request, updatedBy);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update user password
    /// </summary>
    [HttpPatch("{id}/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordRequest request)
    {
        // Validate request
        var validationError = await ValidateAsync(request);
        if (validationError != null) return validationError;

        try
        {
            var updatedBy = GetCurrentUsername();

            var success = await _userService.UpdatePasswordAsync(id, request, updatedBy);

            if (!success)
            {
                return NotFound(new { message = "User not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deletedBy = GetCurrentUsername();

            var success = await _userService.DeleteUserAsync(id, deletedBy);

            if (!success)
            {
                return NotFound(new { message = "User not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
