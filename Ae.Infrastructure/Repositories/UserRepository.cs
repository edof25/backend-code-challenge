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

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        UserMappingConfig.Configure();
    }

    public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllAsync(PaginationRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "dbo.User_Get",
            new
            {
                SearchTerm = request.SearchTerm,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            },
            commandType: CommandType.StoredProcedure
        );

        var totalCount = await multi.ReadFirstAsync<int>();
        var users = await multi.ReadAsync<UserResult>();

        return (users.Select(r => r.Adapt<User>()), totalCount);
    }

    public async Task<User?> GetByIdAsync(int id)
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

    public async Task<User?> GetByUsernameAsync(string username)
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

    public async Task<User> CreateAsync(User user, string createdBy)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryFirstAsync<UserResult>(
            "dbo.User_Create",
            new
            {
                user.CrewMemberId,
                user.RoleId,
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

        return result.Adapt<User>();
    }

    public async Task<User> UpdateAsync(User user, string updatedBy)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryFirstAsync<UserResult>(
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

        return result.Adapt<User>();
    }

    public async Task<bool> UpdatePasswordAsync(int id, string password, string updatedBy)
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

    public async Task<bool> DeleteAsync(int id, string deletedBy)
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
}
