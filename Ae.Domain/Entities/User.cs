namespace Ae.Domain.Entities;

public class User : BaseEntity
{
    public int Id { get; set; }
    public string? CrewMemberId { get; set; }
    public byte RoleId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Nationality { get; set; } = string.Empty;

    public Role? Role { get; set; }
    public RecordStatus? RecordStatus { get; set; }
}
