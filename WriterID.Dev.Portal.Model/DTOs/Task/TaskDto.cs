using System.Text.Json;
using WriterID.Dev.Portal.Model.Enums;
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
    public Enums.TaskStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the list of identified writer IDs from the analysis.
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

    /// <summary>
    /// Creates a new instance of TaskDto from a WriterIdentificationTask entity.
    /// </summary>
    /// <param name="task">The task entity to convert.</param>
    /// <returns>A new TaskDto instance.</returns>
    public static TaskDto FromEntity(WriterIdentificationTask task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            ModelId = task.ModelId,
            DatasetId = task.DatasetId,
            Status = task.Status,
            WriterIds = task.WriterIds,
            Result = task.Result,
            UserId = task.UserId,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
} 