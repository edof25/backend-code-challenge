using Ae.Domain.DTOs.Common;
using Ae.Domain.Entities;

namespace Ae.Infrastructure.Interfaces;

public interface IShipRepository
{
    Task<(IEnumerable<Ship> Ships, int TotalCount)> GetAllAsync(PaginationRequest request);
    Task<Ship?> GetByIdAsync(int id);
    Task<Ship?> GetByCodeAsync(string code);
    Task<IEnumerable<Ship>> GetByUserIdAsync(int userId);
    Task<Ship> CreateAsync(Ship ship, string createdBy);
    Task<Ship> UpdateAsync(Ship ship, string updatedBy);
    Task<bool> DeleteAsync(int id, string deletedBy);
}
