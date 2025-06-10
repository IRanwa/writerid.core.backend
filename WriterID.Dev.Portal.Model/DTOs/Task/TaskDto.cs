using System.Text.Json;
using WriterID.Dev.Portal.Core.Enums;
using WriterID.Dev.Portal.Model.Entities;

namespace WriterID.Dev.Portal.Model.DTOs.Task;

/// <summary>
/// Data transfer object for writer identification task.
/// </summary>
public class TaskDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the task.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the task.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the ID of the model used for writer identification.
    /// </summary>
    public int ModelId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the dataset to analyze.
    /// </summary>
    public int DatasetId { get; set; }

    /// <summary>
    /// Gets or sets the current processing status of the task.
    /// </summary>
    public ProcessingStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the list of writer IDs associated with this task.
    /// </summary>
    public List<string> WriterIds { get; set; }

    /// <summary>
    /// Gets or sets the detailed results of the writer identification in JSON format.
    /// </summary>
    public JsonDocument Result { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who created the task.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the task was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the task was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
} 