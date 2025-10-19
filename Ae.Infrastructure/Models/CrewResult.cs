namespace Ae.Infrastructure.Models;

public class CrewResult
{
    public int UserId { get; set; }
    public string? CrewMemberId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public byte RankId { get; set; }
    public string? RankName { get; set; }
    public DateTime SignOnDate { get; set; }
    public DateTime? SignOffDate { get; set; }
    public DateTime EndOfContractDate { get; set; }
    public byte RecordStatusId { get; set; }
    public string? RecordStatusName { get; set; }
}
