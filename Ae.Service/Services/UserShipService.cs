using Ae.Domain.DTOs.UserShip;
using Ae.Infrastructure.Interfaces;
using Ae.Service.Interfaces;
using Ae.Service.Mappings;
using Mapster;

namespace Ae.Service.Services;

public class UserShipService : IUserShipService
{
    private readonly IUserShipRepository _userShipRepository;

    public UserShipService(IUserShipRepository userShipRepository)
    {
        _userShipRepository = userShipRepository;
        UserShipMappingConfig.Configure();
    }

    public async Task<IEnumerable<UserShipResponse>> GetShipsByUserIdAsync(int userId)
    {
        var crewServiceHistories = await _userShipRepository.GetShipsByUserIdAsync(userId);
        return crewServiceHistories.Select(us => us.Adapt<UserShipResponse>()).ToList();
    }

    public async Task<IEnumerable<UserShipResponse>> GetUsersByShipIdAsync(int shipId)
    {
        var crewServiceHistories = await _userShipRepository.GetUsersByShipIdAsync(shipId);
        return crewServiceHistories.Select(us => us.Adapt<UserShipResponse>()).ToList();
    }

    public async Task<UserShipResponse> AssignShipToUserAsync(AssignShipToUserRequest request, string createdBy)
    {
        var crewServiceHistory = await _userShipRepository.AssignShipToUserAsync(
            request.UserId,
            request.ShipId,
            request.RankId,
            request.SignOnDate,
            request.EndOfContractDate,
            request.SignOffDate,
            createdBy
        );

        return crewServiceHistory.Adapt<UserShipResponse>();
    }

    public async Task<bool> UnassignShipFromUserAsync(int userId, int shipId, string deletedBy)
    {
        return await _userShipRepository.UnassignShipFromUserAsync(userId, shipId, deletedBy);
    }
}
