using Microsoft.AspNetCore.Mvc;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Controllers;

/// <summary>
/// The DashboardController class.
/// </summary>
public class DashboardController : BaseApiController
{
    /// <summary>
    /// The dashboard service
    /// </summary>
    private readonly IDashboardService dashboardService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardController"/> class.
    /// </summary>
    /// <param name="dashboardService">The service for managing dashboard operations.</param>
    public DashboardController(IDashboardService dashboardService)
    {
        this.dashboardService = dashboardService;
    }

    /// <summary>
    /// Gets the dashboard statistics for the current user.
    /// </summary>
    /// <returns>The dashboard statistics for the current user.</returns>
    [HttpGet("stats")]
    public async Task<IActionResult> GetUserDashboardStats()
    {
        var stats = await dashboardService.GetUserDashboardStatsAsync(CurrentUserId);
        return Ok(stats);
    }
} 