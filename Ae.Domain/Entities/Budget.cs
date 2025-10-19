namespace Ae.Domain.Entities;

public class Budget : BaseEntity
{
    public int Id { get; set; }
    public int ShipId { get; set; }
    public int ChartOfAccountId { get; set; }
    public DateTime AccountPeriod { get; set; }
    public decimal BudgetValue { get; set; }

    public Ship? Ship { get; set; }
    public ChartOfAccount? ChartOfAccount { get; set; }
    public RecordStatus? RecordStatus { get; set; }
}
