using Ae.Domain.DTOs.FinancialReport;
using Ae.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ae.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinancialReportController : BaseController
{
    private readonly IFinancialReportService _financialReportService;

    public FinancialReportController(IFinancialReportService financialReportService)
    {
        _financialReportService = financialReportService;
    }

    /// <summary>
    /// Get financial report detail by ship and accounting period
    /// </summary>
    /// <param name="shipId">The ship ID</param>
    /// <param name="accountPeriod">The accounting period</param>
    [HttpGet("ship/{shipId}/detail")]
    [ProducesResponseType(typeof(IEnumerable<FinancialReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDetailByShipAndPeriod(int shipId, [FromQuery] DateTime accountPeriod)
    {
        try
        {
            var report = await _financialReportService.GetDetailByShipAndPeriodAsync(shipId, accountPeriod);
            return Ok(report);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get financial report summary by ship and accounting period
    /// </summary>
    /// <param name="shipId">The ship ID</param>
    /// <param name="accountPeriod">The accounting period</param>
    [HttpGet("ship/{shipId}/summary")]
    [ProducesResponseType(typeof(IEnumerable<FinancialReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSummaryByShipAndPeriod(int shipId, [FromQuery] DateTime accountPeriod)
    {
        try
        {
            var report = await _financialReportService.GetSummaryByShipAndPeriodAsync(shipId, accountPeriod);
            return Ok(report);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
