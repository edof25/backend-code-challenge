using System.Data;
using Ae.Domain.DTOs.FinancialReport;
using Ae.Infrastructure.Data;
using Ae.Infrastructure.Interfaces;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Ae.Infrastructure.Repositories;

public class FinancialReportRepository : IFinancialReportRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<FinancialReportRepository> _logger;

    public FinancialReportRepository(IDbConnectionFactory connectionFactory, ILogger<FinancialReportRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<FinancialReportResponse>> GetDetailByShipAndPeriodAsync(int shipId, DateTime accountPeriod)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryAsync<FinancialReportResponse>(
                "dbo.FinancialReport_GetDetailByShipAndPeriod",
                new
                {
                    ShipId = shipId,
                    AccountPeriod = accountPeriod
                },
                commandType: CommandType.StoredProcedure
            );

            return result;
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
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryAsync<FinancialReportResponse>(
                "dbo.FinancialReport_GetSummaryByShipAndPeriod",
                new
                {
                    ShipId = shipId,
                    AccountPeriod = accountPeriod
                },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetSummaryByShipAndPeriodAsync - ShipId: {ShipId}, AccountPeriod: {AccountPeriod}",
                shipId, accountPeriod);
            throw;
        }
    }
}
