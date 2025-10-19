namespace Ae.Infrastructure.Models;

public class UserShipResult
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ShipId { get; set; }
    public DateTime AssignedDate { get; set; }
    public byte RecordStatusId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // From Users table
    public string? CrewMemberId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public int? Age { get; set; }
    public string? Nationality { get; set; }

    // From CrewServiceHistories table
    public byte? RankId { get; set; }
    public string? RankName { get; set; }
    public DateTime? SignOnDate { get; set; }
    public DateTime? SignOffDate { get; set; }
    public DateTime? EndOfContractDate { get; set; }

    // From Ships table
    public string? ShipCode { get; set; }
    public string? ShipName { get; set; }

    // From RecordStatus table
    public string? RecordStatusName { get; set; }
}
