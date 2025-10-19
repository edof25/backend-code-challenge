using Ae.Domain.DTOs.FinancialReport;

namespace Ae.Service.Interfaces;

public interface IFinancialReportService
{
    Task<IEnumerable<FinancialReportResponse>> GetDetailByShipAndPeriodAsync(int shipId, DateTime accountPeriod);
    Task<IEnumerable<FinancialReportResponse>> GetSummaryByShipAndPeriodAsync(int shipId, DateTime accountPeriod);
}
