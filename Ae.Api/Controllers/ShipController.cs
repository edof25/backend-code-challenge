using Ae.Domain.DTOs.Common;
using Ae.Domain.DTOs.Ship;
using Ae.Domain.DTOs.UserShip;
using Ae.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ae.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShipController : BaseController
{
    private readonly IShipService _shipService;
    private readonly IUserShipService _userShipService;

    public ShipController(IShipService shipService, IUserShipService userShipService)
    {
        _shipService = shipService;
        _userShipService = userShipService;
    }

    /// <summary>
    /// Get all ships with pagination and search
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term to filter by ship code, name, or fiscal year</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ShipResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        var request = new PaginationRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };

        var pagedResult = await _shipService.GetAllShipsAsync(request);
        return Ok(pagedResult);
    }

    /// <summary>
    /// Get ship by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ShipResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var ship = await _shipService.GetShipByIdAsync(id);

        if (ship == null)
        {
            return NotFound(new { message = "Ship not found" });
        }

        return Ok(ship);
    }

    /// <summary>
    /// Get ship by code
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(ShipResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        var ship = await _shipService.GetShipByCodeAsync(code);

        if (ship == null)
        {
            return NotFound(new { message = "Ship not found" });
        }

        return Ok(ship);
    }

    /// <summary>
    /// Get ships by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<ShipResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var ships = await _shipService.GetShipsByUserIdAsync(userId);
        return Ok(ships);
    }

    /// <summary>
    /// Get users assigned to a ship
    /// </summary>
    [HttpGet("{shipId}/crew")]
    [ProducesResponseType(typeof(IEnumerable<UserShipResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersByShipId(int shipId)
    {
        var users = await _userShipService.GetUsersByShipIdAsync(shipId);
        return Ok(users);
    }

    /// <summary>
    /// Create a new ship
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ShipResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateShipRequest request)
    {
        try
        {
            var createdBy = GetCurrentUsername();

            var ship = await _shipService.CreateShipAsync(request, createdBy);
            return CreatedAtAction(nameof(GetById), new { id = ship.Id }, ship);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing ship
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ShipResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateShipRequest request)
    {
        try
        {
            var updatedBy = GetCurrentUsername();

            var ship = await _shipService.UpdateShipAsync(id, request, updatedBy);
            return Ok(ship);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a ship
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deletedBy = GetCurrentUsername();

            var success = await _shipService.DeleteShipAsync(id, deletedBy);

            if (!success)
            {
                return NotFound(new { message = "Ship not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Assign a ship to a user
    /// </summary>
    [HttpPost("assign")]
    [ProducesResponseType(typeof(UserShipResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignShipToUser([FromBody] AssignShipToUserRequest request)
    {
        try
        {
            var createdBy = GetCurrentUsername();

            var userShip = await _userShipService.AssignShipToUserAsync(request, createdBy);
            return CreatedAtAction(nameof(GetUsersByShipId), new { shipId = userShip.ShipId }, userShip);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Unassign a ship from a user
    /// </summary>
    [HttpDelete("unassign/user/{userId}/ship/{shipId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnassignShipFromUser(int userId, int shipId)
    {
        try
        {
            var deletedBy = GetCurrentUsername();

            var success = await _userShipService.UnassignShipFromUserAsync(userId, shipId, deletedBy);

            if (!success)
            {
                return NotFound(new { message = "Assignment not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
