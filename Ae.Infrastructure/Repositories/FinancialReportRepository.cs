using System.Data;
using Ae.Domain.DTOs.FinancialReport;
using Ae.Infrastructure.Data;
using Ae.Infrastructure.Interfaces;
using Dapper;

namespace Ae.Infrastructure.Repositories;

public class FinancialReportRepository : IFinancialReportRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public FinancialReportRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<FinancialReportResponse>> GetDetailByShipAndPeriodAsync(int shipId, DateTime accountPeriod)
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

    public async Task<IEnumerable<FinancialReportResponse>> GetSummaryByShipAndPeriodAsync(int shipId, DateTime accountPeriod)
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
}
