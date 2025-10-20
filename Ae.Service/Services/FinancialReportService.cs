using Ae.Domain.DTOs.FinancialReport;
using Ae.Infrastructure.Interfaces;
using Ae.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace Ae.Service.Services;

public class FinancialReportService : IFinancialReportService
{
    private readonly IFinancialReportRepository _financialReportRepository;
    private readonly ILogger<FinancialReportService> _logger;

    public FinancialReportService(IFinancialReportRepository financialReportRepository, ILogger<FinancialReportService> logger)
    {
        _financialReportRepository = financialReportRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<FinancialReportResponse>> GetDetailByShipAndPeriodAsync(int shipId, DateTime accountPeriod)
    {
        try
        {
            return await _financialReportRepository.GetDetailByShipAndPeriodAsync(shipId, accountPeriod);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetDetailByShipAndPeriodAsync - ShipId: {ShipId}, AccountPeriod: {AccountPeriod}",
                shipId, accountPeriod);
            throw;
        }
    }

    public async Task<IEnumerable<FinancialReportResponse>> GetSummaryByShipAndPeriodAsync(int shipId, DateTime accountPeriod)
    {
        try
        {
            return await _financialReportRepository.GetSummaryByShipAndPeriodAsync(shipId, accountPeriod);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetSummaryByShipAndPeriodAsync - ShipId: {ShipId}, AccountPeriod: {AccountPeriod}",
                shipId, accountPeriod);
            throw;
        }
    }
}
