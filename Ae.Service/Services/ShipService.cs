using Ae.Domain.DTOs.Common;
using Ae.Domain.DTOs.Ship;
using Ae.Domain.Entities;
using Ae.Infrastructure.Interfaces;
using Ae.Service.Interfaces;
using Ae.Service.Mappings;
using Mapster;

namespace Ae.Service.Services;

public class ShipService : IShipService
{
    private readonly IShipRepository _shipRepository;

    public ShipService(IShipRepository shipRepository)
    {
        _shipRepository = shipRepository;
        ShipMappingConfig.Configure();
    }

    public async Task<PagedResult<ShipResponse>> GetAllShipsAsync(PaginationRequest request)
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

    public async Task<ShipResponse?> GetShipByIdAsync(int id)
    {
        var ship = await _shipRepository.GetByIdAsync(id);
        return ship?.Adapt<ShipResponse>();
    }

    public async Task<ShipResponse?> GetShipByCodeAsync(string code)
    {
        var ship = await _shipRepository.GetByCodeAsync(code);
        return ship?.Adapt<ShipResponse>();
    }

    public async Task<IEnumerable<ShipResponse>> GetShipsByUserIdAsync(int userId)
    {
        var ships = await _shipRepository.GetByUserIdAsync(userId);
        return ships.Select(s => s.Adapt<ShipResponse>()).ToList();
    }

    public async Task<ShipResponse> CreateShipAsync(CreateShipRequest request, string createdBy)
    {
        var ship = request.Adapt<Ship>();

        var createdShip = await _shipRepository.CreateAsync(ship, createdBy);
        return createdShip.Adapt<ShipResponse>();
    }

    public async Task<ShipResponse> UpdateShipAsync(int id, UpdateShipRequest request, string updatedBy)
    {
        var ship = request.Adapt<Ship>();
        ship.Id = id;

        var updatedShip = await _shipRepository.UpdateAsync(ship, updatedBy);
        return updatedShip.Adapt<ShipResponse>();
    }

    public async Task<bool> DeleteShipAsync(int id, string deletedBy)
    {
        return await _shipRepository.DeleteAsync(id, deletedBy);
    }
}
