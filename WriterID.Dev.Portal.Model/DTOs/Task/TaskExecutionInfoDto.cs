namespace WriterID.Dev.Portal.Model.DTOs.Task;

/// <summary>
/// Data transfer object containing task execution information for external processing.
/// </summary>
public class TaskExecutionInfoDto
{
    /// <summary>
    /// Gets or sets the task identifier.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Gets or sets the task container name where query image is stored.
    /// </summary>
    public string TaskContainerName { get; set; }

    /// <summary>
    /// Gets or sets the dataset container name.
    /// </summary>
    public string DatasetContainerName { get; set; }

    /// <summary>
    /// Gets or sets the model container name.
    /// Will be null if using default model.
    /// </summary>
    public string ModelContainerName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use the default model.
    /// </summary>
    public bool UseDefaultModel { get; set; }

    /// <summary>
    /// Gets or sets the list of selected writer IDs for comparison.
    /// </summary>
    public List<string> SelectedWriters { get; set; }

    /// <summary>
    /// Gets or sets the query image file name in the task container.
    /// </summary>
    public string QueryImageFileName { get; set; }

    /// <summary>
    /// Gets or sets the current task status.
    /// </summary>
    public string Status { get; set; }
} 