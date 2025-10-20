using Ae.Domain.DTOs.Common;
using Ae.Domain.DTOs.Ship;
using Ae.Domain.Entities;
using Ae.Infrastructure.Interfaces;
using Ae.Service.Interfaces;
using Ae.Service.Mappings;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Ae.Service.Services;

public class ShipService : IShipService
{
    private readonly IShipRepository _shipRepository;
    private readonly ILogger<ShipService> _logger;

    public ShipService(IShipRepository shipRepository, ILogger<ShipService> logger)
    {
        _shipRepository = shipRepository;
        _logger = logger;
        ShipMappingConfig.Configure();
    }

    public async Task<PagedResult<ShipResponse>> GetAllShipsAsync(PaginationRequest request)
    {
        try
        {
            var (ships, totalCount) = await _shipRepository.GetAllAsync(request);

            return new PagedResult<ShipResponse>
            {
                Items = ships.Select(s => s.Adapt<ShipResponse>()),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllShipsAsync - PageNumber: {PageNumber}, PageSize: {PageSize}",
                request.PageNumber, request.PageSize);
            throw;
        }
    }

    public async Task<ShipResponse?> GetShipByIdAsync(int id)
    {
        try
        {
            var ship = await _shipRepository.GetByIdAsync(id);
            return ship?.Adapt<ShipResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetShipByIdAsync - Id: {Id}", id);
            throw;
        }
    }

    public async Task<ShipResponse?> GetShipByCodeAsync(string code)
    {
        try
        {
            var ship = await _shipRepository.GetByCodeAsync(code);
            return ship?.Adapt<ShipResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetShipByCodeAsync - Code: {Code}", code);
            throw;
        }
    }

    public async Task<IEnumerable<ShipResponse>> GetShipsByUserIdAsync(int userId)
    {
        try
        {
            var ships = await _shipRepository.GetByUserIdAsync(userId);
            return ships.Select(s => s.Adapt<ShipResponse>()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetShipsByUserIdAsync - UserId: {UserId}", userId);
            throw;
        }
    }

    public async Task<ShipResponse> CreateShipAsync(CreateShipRequest request, string createdBy)
    {
        try
        {
            var ship = request.Adapt<Ship>();

            var createdShip = await _shipRepository.CreateAsync(ship, createdBy);
            return createdShip.Adapt<ShipResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateShipAsync - Code: {Code}, CreatedBy: {CreatedBy}",
                request.Code, createdBy);
            throw;
        }
    }

    public async Task<ShipResponse> UpdateShipAsync(int id, UpdateShipRequest request, string updatedBy)
    {
        try
        {
            var ship = request.Adapt<Ship>();
            ship.Id = id;

            var updatedShip = await _shipRepository.UpdateAsync(ship, updatedBy);
            return updatedShip.Adapt<ShipResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateShipAsync - Id: {Id}, UpdatedBy: {UpdatedBy}",
                id, updatedBy);
            throw;
        }
    }

    public async Task<bool> DeleteShipAsync(int id, string deletedBy)
    {
        try
        {
            return await _shipRepository.DeleteAsync(id, deletedBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteShipAsync - Id: {Id}, DeletedBy: {DeletedBy}",
                id, deletedBy);
            throw;
        }
    }
}
