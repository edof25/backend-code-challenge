namespace Ae.Domain.DTOs.UserShip;

public class UserShipResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? CrewMemberId { get; set; }
    public string UserFirstName { get; set; } = string.Empty;
    public string UserLastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int Age
    {
        get
        {
            if (BirthDate == DateTime.MinValue)
                return 0;

            var today = DateTime.Now;
            var age = today.Year - BirthDate.Year;

            if (today.Month < BirthDate.Month ||
                (today.Month == BirthDate.Month && today.Day < BirthDate.Day))
            {
                age--;
            }

            return age;
        }
    }
    public string Nationality { get; set; } = string.Empty;
    public byte RankId { get; set; }
    public string RankName { get; set; } = string.Empty;
    public DateTime SignOnDate { get; set; }
    public DateTime? SignOffDate { get; set; }
    public DateTime EndOfContractDate { get; set; }
    public string Status
    {
        get
        {
            var today = DateTime.Now;

            if (SignOnDate <= today && !SignOffDate.HasValue && EndOfContractDate > today)
                return "Onboard";
            else if (SignOnDate > today)
                return "Planned";
            else if (!SignOffDate.HasValue && (today - EndOfContractDate).TotalDays >= 30)
                return "Relief Due";
            else
                return "Signed Off";
        }
    }
    public int ShipId { get; set; }
    public string ShipCode { get; set; } = string.Empty;
    public string ShipName { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
    public byte RecordStatusId { get; set; }
    public string RecordStatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
