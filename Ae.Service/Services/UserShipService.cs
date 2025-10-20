using Ae.Domain.DTOs.Common;
using Ae.Domain.DTOs.UserShip;
using Ae.Infrastructure.Interfaces;
using Ae.Service.Interfaces;
using Ae.Service.Mappings;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Ae.Service.Services;

public class UserShipService : IUserShipService
{
    private readonly IUserShipRepository _userShipRepository;
    private readonly ILogger<UserShipService> _logger;

    public UserShipService(IUserShipRepository userShipRepository, ILogger<UserShipService> logger)
    {
        _userShipRepository = userShipRepository;
        _logger = logger;
        UserShipMappingConfig.Configure();
    }

    public async Task<IEnumerable<UserShipResponse>> GetShipsByUserIdAsync(int userId)
    {
        try
        {
            var crewServiceHistories = await _userShipRepository.GetShipsByUserIdAsync(userId);
            return crewServiceHistories.Select(us => us.Adapt<UserShipResponse>()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetShipsByUserIdAsync - UserId: {UserId}", userId);
            throw;
        }
    }

    public async Task<PagedResult<UserShipResponse>> GetUsersByShipIdAsync(int shipId, PaginationRequest request)
    {
        try
        {
            var (crewServiceHistories, totalCount) = await _userShipRepository.GetUsersByShipIdAsync(shipId, request);

            var userShipResponses = crewServiceHistories.Select(us => us.Adapt<UserShipResponse>()).ToList();

            return new PagedResult<UserShipResponse>
            {
                Items = userShipResponses,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUsersByShipIdAsync - ShipId: {ShipId}, PageNumber: {PageNumber}, PageSize: {PageSize}",
                shipId, request.PageNumber, request.PageSize);
            throw;
        }
    }

    public async Task<UserShipResponse> AssignShipToUserAsync(AssignShipToUserRequest request, string createdBy)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AssignShipToUserAsync - UserId: {UserId}, ShipId: {ShipId}, CreatedBy: {CreatedBy}",
                request.UserId, request.ShipId, createdBy);
            throw;
        }
    }

    public async Task<bool> UnassignShipFromUserAsync(int userId, int shipId, string deletedBy)
    {
        try
        {
            return await _userShipRepository.UnassignShipFromUserAsync(userId, shipId, deletedBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UnassignShipFromUserAsync - UserId: {UserId}, ShipId: {ShipId}, DeletedBy: {DeletedBy}",
                userId, shipId, deletedBy);
            throw;
        }
    }
}
