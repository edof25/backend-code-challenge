using Ae.Domain.Entities;

namespace Ae.Service.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
