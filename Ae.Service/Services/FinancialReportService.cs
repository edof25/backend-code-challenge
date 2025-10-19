using Ae.Domain.DTOs.FinancialReport;
using Ae.Infrastructure.Interfaces;
using Ae.Service.Interfaces;

namespace Ae.Service.Services;

public class FinancialReportService : IFinancialReportService
{
    private readonly IFinancialReportRepository _financialReportRepository;

    public FinancialReportService(IFinancialReportRepository financialReportRepository)
    {
        _financialReportRepository = financialReportRepository;
    }

    public async Task<IEnumerable<FinancialReportResponse>> GetDetailByShipAndPeriodAsync(int shipId, DateTime accountPeriod)
    {
        return await _financialReportRepository.GetDetailByShipAndPeriodAsync(shipId, accountPeriod);
    }

    public async Task<IEnumerable<FinancialReportResponse>> GetSummaryByShipAndPeriodAsync(int shipId, DateTime accountPeriod)
    {
        return await _financialReportRepository.GetSummaryByShipAndPeriodAsync(shipId, accountPeriod);
    }
}
