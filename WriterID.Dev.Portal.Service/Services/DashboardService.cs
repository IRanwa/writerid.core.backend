using WriterID.Dev.Portal.Core.Enums;
using WriterID.Dev.Portal.Data.Interfaces;
using WriterID.Dev.Portal.Model.DTOs.Dashboard;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The DashboardService class.
/// </summary>
public class DashboardService : IDashboardService
{
    /// <summary>
    /// The unit of work
    /// </summary>
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public DashboardService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }



    /// <summary>
    /// Gets the dashboard statistics for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to get statistics for.</param>
    /// <returns>A task containing the dashboard statistics for the user.</returns>
    public async Task<DashboardStatsDto> GetUserDashboardStatsAsync(int userId)
    {
        var tasks = await unitOfWork.Repository<WriterIdentificationTask>().FindAsync(t => t.UserId == userId && t.IsActive);
        var datasets = await unitOfWork.Repository<Dataset>().FindAsync(d => d.UserId == userId && d.IsActive);
        var models = await unitOfWork.Repository<WriterIdentificationModel>().FindAsync(m => m.UserId == userId && m.IsActive);

        var stats = new DashboardStatsDto
        {
            TotalTasks = tasks.Count(),
            CompletedTasks = tasks.Count(t => t.Status == ProcessingStatus.Completed),
            TotalDatasets = datasets.Count(),
            TotalModels = models.Count()
        };

        return stats;
    }
} 