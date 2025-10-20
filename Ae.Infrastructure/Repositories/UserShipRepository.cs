using System.Data;
using Ae.Domain.DTOs.Common;
using Ae.Domain.Entities;
using Ae.Infrastructure.Data;
using Ae.Infrastructure.Interfaces;
using Ae.Infrastructure.Mappings;
using Ae.Infrastructure.Models;
using Dapper;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Ae.Infrastructure.Repositories;

public class UserShipRepository : IUserShipRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<UserShipRepository> _logger;

    public UserShipRepository(IDbConnectionFactory connectionFactory, ILogger<UserShipRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        UserShipMappingConfig.Configure();
    }

    public async Task<IEnumerable<CrewServiceHistory>> GetShipsByUserIdAsync(int userId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryAsync<UserShipResult>(
                "dbo.CrewServiceHistory_Get",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );

            return result.Select(r => r.Adapt<CrewServiceHistory>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetShipsByUserIdAsync - UserId: {UserId}", userId);
            throw;
        }
    }

    public async Task<(IEnumerable<CrewServiceHistory> CrewServiceHistories, int TotalCount)> GetUsersByShipIdAsync(int shipId, PaginationRequest request)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "dbo.CrewServiceHistory_Get",
                new
                {
                    ShipId = shipId,
                    request.SearchTerm,
                    request.SortBy,
                    request.SortOrder,
                    request.PageNumber,
                    request.PageSize
                },
                commandType: CommandType.StoredProcedure
            );

            var totalCount = await multi.ReadFirstAsync<int>();
            var results = await multi.ReadAsync<UserShipResult>();

            return (results.Select(r => r.Adapt<CrewServiceHistory>()), totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUsersByShipIdAsync - ShipId: {ShipId}, PageNumber: {PageNumber}, PageSize: {PageSize}",
                shipId, request.PageNumber, request.PageSize);
            throw;
        }
    }

    public async Task<CrewServiceHistory?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "dbo.CrewServiceHistory_Get",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            await multi.ReadFirstAsync<int>();

            var result = await multi.ReadFirstOrDefaultAsync<UserShipResult>();

            return result?.Adapt<CrewServiceHistory>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetByIdAsync - Id: {Id}", id);
            throw;
        }
    }

    public async Task<CrewServiceHistory> AssignShipToUserAsync(int userId, int shipId, byte rankId, DateTime signOnDate, DateTime endOfContractDate, DateTime? signOffDate, string createdBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "dbo.CrewServiceHistory_Create",
                new
                {
                    UserId = userId,
                    ShipId = shipId,
                    RankId = rankId,
                    SignOnDate = signOnDate,
                    SignOffDate = signOffDate,
                    EndOfContractDate = endOfContractDate,
                    CreatedBy = createdBy
                },
                commandType: CommandType.StoredProcedure
            );

            await multi.ReadFirstAsync<int>();

            var result = await multi.ReadFirstOrDefaultAsync<UserShipResult>();

            return result.Adapt<CrewServiceHistory>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AssignShipToUserAsync - UserId: {UserId}, ShipId: {ShipId}, CreatedBy: {CreatedBy}",
                userId, shipId, createdBy);
            throw;
        }
    }

    public async Task<bool> UnassignShipFromUserAsync(int userId, int shipId, string deletedBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<SuccessResult>(
                "dbo.CrewServiceHistory_Delete",
                new
                {
                    UserId = userId,
                    ShipId = shipId,
                    DeletedBy = deletedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return result?.Success == 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UnassignShipFromUserAsync - UserId: {UserId}, ShipId: {ShipId}, DeletedBy: {DeletedBy}",
                userId, shipId, deletedBy);
            throw;
        }
    }

    public async Task<bool> IsShipAssignedToUserAsync(int userId, int shipId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<UserShipResult>(
                "dbo.CrewServiceHistory_Get",
                new
                {
                    UserId = userId,
                    ShipId = shipId
                },
                commandType: CommandType.StoredProcedure
            );

            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in IsShipAssignedToUserAsync - UserId: {UserId}, ShipId: {ShipId}",
                userId, shipId);
            throw;
        }
    }
}
