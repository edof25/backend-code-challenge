using Ae.Domain.DTOs.FinancialReport;

namespace Ae.Infrastructure.Interfaces;

public interface IFinancialReportRepository
{
    Task<IEnumerable<FinancialReportResponse>> GetDetailByShipAndPeriodAsync(int shipId, DateTime accountPeriod);
    Task<IEnumerable<FinancialReportResponse>> GetSummaryByShipAndPeriodAsync(int shipId, DateTime accountPeriod);
}
