using Ae.Domain.DTOs;
using Ae.Domain.DTOs.Common;

namespace Ae.Service.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserResponse>> GetAllUsersAsync(PaginationRequest request);
    Task<UserResponse?> GetUserByIdAsync(int id);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, string createdBy);
    Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request, string updatedBy);
    Task<bool> UpdatePasswordAsync(int id, UpdatePasswordRequest request, string updatedBy);
    Task<bool> DeleteUserAsync(int id, string deletedBy);
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
