namespace Ae.Domain.DTOs.FinancialReport;

public class FinancialReportResponse
{
    public string ChartOfAccountNumber { get; set; } = string.Empty;
    public string ChartOfAccountName { get; set; } = string.Empty;
    public decimal Actual { get; set; }
    public decimal Budget { get; set; }
    public decimal VarianceActual { get; set; }
    public decimal ActualYTD { get; set; }
    public decimal BudgetYTD { get; set; }
    public decimal VarianceYTD { get; set; }
}
