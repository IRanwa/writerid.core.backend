namespace WriterID.Dev.Portal.Model.DTOs.Dashboard;

/// <summary>
/// Represents the dashboard statistics data transfer object.
/// </summary>
public class DashboardStatsDto
{
    /// <summary>
    /// Gets or sets the total number of tasks.
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Gets or sets the number of completed tasks.
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// Gets or sets the total number of datasets.
    /// </summary>
    public int TotalDatasets { get; set; }

    /// <summary>
    /// Gets or sets the total number of models.
    /// </summary>
    public int TotalModels { get; set; }
} 