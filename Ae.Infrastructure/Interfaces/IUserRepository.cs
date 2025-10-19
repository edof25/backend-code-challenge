using Ae.Domain.DTOs.Common;
using Ae.Domain.Entities;

namespace Ae.Infrastructure.Interfaces;

public interface IUserRepository
{
    Task<(IEnumerable<User> Users, int TotalCount)> GetAllAsync(PaginationRequest request);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User> CreateAsync(User user, string createdBy);
    Task<User> UpdateAsync(User user, string updatedBy);
    Task<bool> UpdatePasswordAsync(int id, string password, string updatedBy);
    Task<bool> DeleteAsync(int id, string deletedBy);
}
