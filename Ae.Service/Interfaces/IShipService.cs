using Ae.Domain.DTOs.Common;
using Ae.Domain.DTOs.Ship;

namespace Ae.Service.Interfaces;

public interface IShipService
{
    Task<PagedResult<ShipResponse>> GetAllShipsAsync(PaginationRequest request);
    Task<ShipResponse?> GetShipByIdAsync(int id);
    Task<ShipResponse?> GetShipByCodeAsync(string code);
    Task<IEnumerable<ShipResponse>> GetShipsByUserIdAsync(int userId);
    Task<ShipResponse> CreateShipAsync(CreateShipRequest request, string createdBy);
    Task<ShipResponse> UpdateShipAsync(int id, UpdateShipRequest request, string updatedBy);
    Task<bool> DeleteShipAsync(int id, string deletedBy);
}
