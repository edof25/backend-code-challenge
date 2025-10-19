using System.Data;
using Ae.Domain.DTOs.Common;
using Ae.Domain.Entities;
using Ae.Infrastructure.Data;
using Ae.Infrastructure.Interfaces;
using Ae.Infrastructure.Mappings;
using Ae.Infrastructure.Models;
using Dapper;
using Mapster;

namespace Ae.Infrastructure.Repositories;

public class ShipRepository : IShipRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ShipRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        ShipMappingConfig.Configure();
    }

    public async Task<(IEnumerable<Ship> Ships, int TotalCount)> GetAllAsync(PaginationRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "dbo.Ship_Get",
            new
            {
                SearchTerm = request.SearchTerm,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            },
            commandType: CommandType.StoredProcedure
        );

        var totalCount = await multi.ReadFirstAsync<int>();
        var ships = await multi.ReadAsync<ShipResult>();

        return (ships.Select(r => r.Adapt<Ship>()), totalCount);
    }

    public async Task<Ship?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "dbo.Ship_Get",
            new { Id = id },
            commandType: CommandType.StoredProcedure
        );

        await multi.ReadFirstAsync<int>();

        var result = await multi.ReadFirstOrDefaultAsync<ShipResult>();

        return result?.Adapt<Ship>();
    }

    public async Task<Ship?> GetByCodeAsync(string code)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "dbo.Ship_Get",
            new { Code = code },
            commandType: CommandType.StoredProcedure
        );

        await multi.ReadFirstAsync<int>();

        var result = await multi.ReadFirstOrDefaultAsync<ShipResult>();

        return result?.Adapt<Ship>();
    }

    public async Task<IEnumerable<Ship>> GetByUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "dbo.Ship_Get",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure
        );

        await multi.ReadFirstAsync<int>();

        var ships = await multi.ReadAsync<ShipResult>();

        return ships.Select(r => r.Adapt<Ship>());
    }

    public async Task<Ship> CreateAsync(Ship ship, string createdBy)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryFirstAsync<ShipResult>(
            "dbo.Ship_Create",
            new
            {
                ship.Code,
                ship.Name,
                ship.FiscalYear,
                CreatedBy = createdBy
            },
            commandType: CommandType.StoredProcedure
        );

        return result.Adapt<Ship>();
    }

    public async Task<Ship> UpdateAsync(Ship ship, string updatedBy)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryFirstAsync<ShipResult>(
            "dbo.Ship_Update",
            new
            {
                ship.Id,
                ship.Code,
                ship.Name,
                ship.FiscalYear,
                UpdatedBy = updatedBy
            },
            commandType: CommandType.StoredProcedure
        );

        return result.Adapt<Ship>();
    }

    public async Task<bool> DeleteAsync(int id, string deletedBy)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryFirstOrDefaultAsync<SuccessResult>(
            "dbo.Ship_Delete",
            new
            {
                Id = id,
                DeletedBy = deletedBy
            },
            commandType: CommandType.StoredProcedure
        );

        return result?.Success == 1;
    }
}
