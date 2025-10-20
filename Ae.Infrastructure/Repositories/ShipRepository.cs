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

public class ShipRepository : IShipRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<ShipRepository> _logger;

    public ShipRepository(IDbConnectionFactory connectionFactory, ILogger<ShipRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        ShipMappingConfig.Configure();
    }

    public async Task<(IEnumerable<Ship> Ships, int TotalCount)> GetAllAsync(PaginationRequest request)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "dbo.Ship_Get",
                new
                {
                    request.SearchTerm,
                    request.PageNumber,
                    request.PageSize
                },
                commandType: CommandType.StoredProcedure
            );

            var totalCount = await multi.ReadFirstAsync<int>();
            var ships = await multi.ReadAsync<ShipResult>();

            return (ships.Select(r => r.Adapt<Ship>()), totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllAsync - PageNumber: {PageNumber}, PageSize: {PageSize}",
                request.PageNumber, request.PageSize);
            throw;
        }
    }

    public async Task<Ship?> GetByIdAsync(int id)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetByIdAsync - Id: {Id}", id);
            throw;
        }
    }

    public async Task<Ship?> GetByCodeAsync(string code)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetByCodeAsync - Code: {Code}", code);
            throw;
        }
    }

    public async Task<IEnumerable<Ship>> GetByUserIdAsync(int userId)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetByUserIdAsync - UserId: {UserId}", userId);
            throw;
        }
    }

    public async Task<Ship> CreateAsync(Ship ship, string createdBy)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateAsync - Code: {Code}, CreatedBy: {CreatedBy}",
                ship.Code, createdBy);
            throw;
        }
    }

    public async Task<Ship> UpdateAsync(Ship ship, string updatedBy)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateAsync - Id: {Id}, UpdatedBy: {UpdatedBy}",
                ship.Id, updatedBy);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, string deletedBy)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteAsync - Id: {Id}, DeletedBy: {DeletedBy}",
                id, deletedBy);
            throw;
        }
    }
}
