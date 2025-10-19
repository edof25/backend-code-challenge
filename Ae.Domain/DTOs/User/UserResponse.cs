namespace Ae.Domain.DTOs;

public class UserResponse
{
    public int Id { get; set; }
    public string? CrewMemberId { get; set; }
    public byte RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public byte RecordStatusId { get; set; }
    public string RecordStatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
