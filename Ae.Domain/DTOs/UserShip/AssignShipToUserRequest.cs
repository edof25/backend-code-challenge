namespace Ae.Domain.DTOs.UserShip;

public class AssignShipToUserRequest
{
    public int UserId { get; set; }
    public int ShipId { get; set; }
    public byte RankId { get; set; }
    public DateTime SignOnDate { get; set; } = DateTime.UtcNow;
    public DateTime EndOfContractDate { get; set; }
    public DateTime? SignOffDate { get; set; }
}
