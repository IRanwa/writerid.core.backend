using WriterID.Dev.Portal.Model.DTOs.Dashboard;

namespace WriterID.Dev.Portal.Service.Interfaces;

/// <summary>
/// Interface for dashboard service operations.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Gets the dashboard statistics for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to get statistics for.</param>
    /// <returns>A task containing the dashboard statistics for the user.</returns>
    Task<DashboardStatsDto> GetUserDashboardStatsAsync(int userId);
} 