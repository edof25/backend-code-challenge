namespace Ae.Domain.Entities;

public class Ship : BaseEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FiscalYear { get; set; } = string.Empty;

    public RecordStatus? RecordStatus { get; set; }
}
