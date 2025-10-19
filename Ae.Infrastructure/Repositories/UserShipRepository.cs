using System.Data;
using Ae.Domain.Entities;
using Ae.Infrastructure.Data;
using Ae.Infrastructure.Interfaces;
using Ae.Infrastructure.Mappings;
using Ae.Infrastructure.Models;
using Dapper;
using Mapster;

namespace Ae.Infrastructure.Repositories;

public class UserShipRepository : IUserShipRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserShipRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        UserShipMappingConfig.Configure();
    }

    public async Task<IEnumerable<CrewServiceHistory>> GetShipsByUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryAsync<UserShipResult>(
            "dbo.CrewServiceHistory_Get",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure
        );

        return result.Select(r => r.Adapt<CrewServiceHistory>());
    }

    public async Task<IEnumerable<CrewServiceHistory>> GetUsersByShipIdAsync(int shipId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryAsync<UserShipResult>(
            "dbo.CrewServiceHistory_Get",
            new { ShipId = shipId },
            commandType: CommandType.StoredProcedure
        );

        return result.Select(r => r.Adapt<CrewServiceHistory>());
    }

    public async Task<CrewServiceHistory?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryFirstOrDefaultAsync<UserShipResult>(
            "dbo.CrewServiceHistory_Get",
            new { Id = id },
            commandType: CommandType.StoredProcedure
        );

        return result?.Adapt<CrewServiceHistory>();
    }

    public async Task<CrewServiceHistory> AssignShipToUserAsync(int userId, int shipId, byte rankId, DateTime signOnDate, DateTime endOfContractDate, DateTime? signOffDate, string createdBy)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryFirstAsync<UserShipResult>(
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

        return result.Adapt<CrewServiceHistory>();
    }

    public async Task<bool> UnassignShipFromUserAsync(int userId, int shipId, string deletedBy)
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

    public async Task<bool> IsShipAssignedToUserAsync(int userId, int shipId)
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
}
