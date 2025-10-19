namespace Ae.Domain.DTOs.Ship;

public class ShipResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FiscalYear { get; set; } = string.Empty;
    public byte RecordStatusId { get; set; }
    public string RecordStatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
