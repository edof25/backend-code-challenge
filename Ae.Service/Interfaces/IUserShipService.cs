using Ae.Domain.DTOs.UserShip;

namespace Ae.Service.Interfaces;

public interface IUserShipService
{
    Task<IEnumerable<UserShipResponse>> GetShipsByUserIdAsync(int userId);
    Task<IEnumerable<UserShipResponse>> GetUsersByShipIdAsync(int shipId);
    Task<UserShipResponse> AssignShipToUserAsync(AssignShipToUserRequest request, string createdBy);
    Task<bool> UnassignShipFromUserAsync(int userId, int shipId, string deletedBy);
}
