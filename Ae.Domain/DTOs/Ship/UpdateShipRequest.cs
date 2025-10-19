namespace Ae.Domain.DTOs.Ship;

public class UpdateShipRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FiscalYear { get; set; } = string.Empty;
}
