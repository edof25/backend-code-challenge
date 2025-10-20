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

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IDbConnectionFactory connectionFactory, ILogger<UserRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        UserMappingConfig.Configure();
    }

    public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllAsync(PaginationRequest request)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "dbo.User_Get",
                new
                {
                    request.SearchTerm,
                    request.SortBy,
                    request.SortOrder,
                    request.PageNumber,
                    request.PageSize
                },
                commandType: CommandType.StoredProcedure
            );

            var totalCount = await multi.ReadFirstAsync<int>();
            var users = await multi.ReadAsync<UserResult>();

            return (users.Select(r => r.Adapt<User>()), totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllAsync - PageNumber: {PageNumber}, PageSize: {PageSize}",
                request.PageNumber, request.PageSize);
            throw;
        }
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "dbo.User_Get",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            await multi.ReadFirstAsync<int>();

            var result = await multi.ReadFirstOrDefaultAsync<UserResult>();

            return result?.Adapt<User>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetByIdAsync - Id: {Id}", id);
            throw;
        }
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "dbo.User_Get",
                new { Username = username },
                commandType: CommandType.StoredProcedure
            );

            await multi.ReadFirstAsync<int>();

            var result = await multi.ReadFirstOrDefaultAsync<UserResult>();

            return result?.Adapt<User>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetByUsernameAsync - Username: {Username}", username);
            throw;
        }
    }

    public async Task<User> CreateAsync(User user, string createdBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "dbo.User_Create",
                new
                {
                    user.RoleId,
                    user.CrewMemberId,
                    user.Username,
                    user.Password,
                    user.FirstName,
                    user.LastName,
                    user.BirthDate,
                    user.Nationality,
                    CreatedBy = createdBy
                },
                commandType: CommandType.StoredProcedure
            );

            await multi.ReadFirstAsync<int>();

            var result = await multi.ReadFirstOrDefaultAsync<UserResult>();

            return result.Adapt<User>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateAsync - Username: {Username}, CreatedBy: {CreatedBy}",
                user.Username, createdBy);
            throw;
        }
    }

    public async Task<User> UpdateAsync(User user, string updatedBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "dbo.User_Update",
                new
                {
                    user.Id,
                    user.CrewMemberId,
                    user.RoleId,
                    user.Username,
                    user.FirstName,
                    user.LastName,
                    user.BirthDate,
                    user.Nationality,
                    UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            await multi.ReadFirstAsync<int>();

            var result = await multi.ReadFirstOrDefaultAsync<UserResult>();

            return result.Adapt<User>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateAsync - Id: {Id}, UpdatedBy: {UpdatedBy}",
                user.Id, updatedBy);
            throw;
        }
    }

    public async Task<bool> UpdatePasswordAsync(int id, string password, string updatedBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<SuccessResult>(
                "dbo.User_UpdatePassword",
                new
                {
                    Id = id,
                    Password = password,
                    UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return result?.Success == 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdatePasswordAsync - Id: {Id}, UpdatedBy: {UpdatedBy}",
                id, updatedBy);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, string deletedBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<SuccessResult>(
                "dbo.User_Delete",
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
