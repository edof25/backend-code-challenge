using Ae.Domain.Enums;

namespace Ae.Domain.Entities;

public class ChartOfAccount : BaseEntity
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public AccountType AccountType { get; set; }
    public int Level { get; set; }

    public ChartOfAccount? Parent { get; set; }
    public ICollection<ChartOfAccount>? Children { get; set; }
    public RecordStatus? RecordStatus { get; set; }
}
