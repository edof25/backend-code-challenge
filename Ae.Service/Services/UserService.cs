using Ae.Domain.DTOs;
using Ae.Domain.DTOs.Common;
using Ae.Domain.Configuration;
using Ae.Domain.Entities;
using Ae.Infrastructure.Interfaces;
using Ae.Service.Interfaces;
using Ae.Service.Mappings;
using Mapster;
using BCrypt.Net;
using Microsoft.Extensions.Options;

namespace Ae.Service.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public UserService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
        UserMappingConfig.Configure();
    }

    public async Task<PagedResult<UserResponse>> GetAllUsersAsync(PaginationRequest request)
    {
        var (users, totalCount) = await _userRepository.GetAllAsync(request);

        return new PagedResult<UserResponse>
        {
            Items = users.Select(u => u.Adapt<UserResponse>()),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user?.Adapt<UserResponse>();
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, string createdBy)
    {
        var user = request.Adapt<User>();
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var createdUser = await _userRepository.CreateAsync(user, createdBy);
        return createdUser.Adapt<UserResponse>();
    }

    public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request, string updatedBy)
    {
        var user = request.Adapt<User>();
        user.Id = id;

        var updatedUser = await _userRepository.UpdateAsync(user, updatedBy);
        return updatedUser.Adapt<UserResponse>();
    }

    public async Task<bool> UpdatePasswordAsync(int id, UpdatePasswordRequest request, string updatedBy)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        return await _userRepository.UpdatePasswordAsync(id, hashedPassword, updatedBy);
    }

    public async Task<bool> DeleteUserAsync(int id, string deletedBy)
    {
        return await _userRepository.DeleteAsync(id, deletedBy);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        // Get user by username
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null)
        {
            return null;
        }

        // Verify password
        var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!isValidPassword)
        {
            return null;
        }

        // Generate JWT token
        var token = _tokenService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            RoleId = user.RoleId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes)
        };
    }
}
