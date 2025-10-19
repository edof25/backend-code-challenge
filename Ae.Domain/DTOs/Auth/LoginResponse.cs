namespace Ae.Domain.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public byte RoleId { get; set; }
    public DateTime ExpiresAt { get; set; }
}
