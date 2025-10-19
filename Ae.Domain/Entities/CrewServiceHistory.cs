namespace Ae.Domain.Entities;

public class CrewServiceHistory : BaseEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public byte RankId { get; set; }
    public int ShipId { get; set; }
    public DateTime SignOnDate { get; set; }
    public DateTime? SignOffDate { get; set; }
    public DateTime EndOfContractDate { get; set; }

    public User? User { get; set; }
    public Rank? Rank { get; set; }
    public Ship? Ship { get; set; }
    public RecordStatus? RecordStatus { get; set; }
}
