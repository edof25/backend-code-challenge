using Ae.Domain.DTOs;
using Ae.Domain.DTOs.Common;
using Ae.Domain.Configuration;
using Ae.Domain.Entities;
using Ae.Infrastructure.Interfaces;
using Ae.Service.Interfaces;
using Ae.Service.Mappings;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ae.Service.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
        UserMappingConfig.Configure();
    }

    public async Task<PagedResult<UserResponse>> GetAllUsersAsync(PaginationRequest request)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllUsersAsync - PageNumber: {PageNumber}, PageSize: {PageSize}",
                request.PageNumber, request.PageSize);
            throw;
        }
    }

    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user?.Adapt<UserResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserByIdAsync - Id: {Id}", id);
            throw;
        }
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, string createdBy)
    {
        try
        {
            var user = request.Adapt<User>();
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var createdUser = await _userRepository.CreateAsync(user, createdBy);
            return createdUser.Adapt<UserResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateUserAsync - Username: {Username}, CreatedBy: {CreatedBy}",
                request.Username, createdBy);
            throw;
        }
    }

    public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request, string updatedBy)
    {
        try
        {
            var user = request.Adapt<User>();
            user.Id = id;

            var updatedUser = await _userRepository.UpdateAsync(user, updatedBy);
            return updatedUser.Adapt<UserResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateUserAsync - Id: {Id}, UpdatedBy: {UpdatedBy}",
                id, updatedBy);
            throw;
        }
    }

    public async Task<bool> UpdatePasswordAsync(int id, UpdatePasswordRequest request, string updatedBy)
    {
        try
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            return await _userRepository.UpdatePasswordAsync(id, hashedPassword, updatedBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdatePasswordAsync - Id: {Id}, UpdatedBy: {UpdatedBy}",
                id, updatedBy);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id, string deletedBy)
    {
        try
        {
            return await _userRepository.DeleteAsync(id, deletedBy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteUserAsync - Id: {Id}, DeletedBy: {DeletedBy}",
                id, deletedBy);
            throw;
        }
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null)
            {
                return null;
            }

            var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isValidPassword)
            {
                return null;
            }

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LoginAsync - Username: {Username}",
                request.Username);
            throw;
        }
    }
}
