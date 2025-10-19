using Ae.Domain.Entities;

namespace Ae.Infrastructure.Interfaces;

public interface IUserShipRepository
{
    Task<IEnumerable<CrewServiceHistory>> GetShipsByUserIdAsync(int userId);
    Task<IEnumerable<CrewServiceHistory>> GetUsersByShipIdAsync(int shipId);
    Task<CrewServiceHistory?> GetByIdAsync(int id);
    Task<CrewServiceHistory> AssignShipToUserAsync(int userId, int shipId, byte rankId, DateTime signOnDate, DateTime endOfContractDate, DateTime? signOffDate, string createdBy);
    Task<bool> UnassignShipFromUserAsync(int userId, int shipId, string deletedBy);
    Task<bool> IsShipAssignedToUserAsync(int userId, int shipId);
}
