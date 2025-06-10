using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Model.Entities;

/// <summary>
/// Represents a writer identification task in the system.
/// </summary>
public class WriterIdentificationTask
{
    /// <summary>
    /// Gets or sets the unique identifier for the task.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the task.
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    [MaxLength(1000)]
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
    public TaskExecutionStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who created the task.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the task was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the task was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the task is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the model used for writer identification.
    /// </summary>
    public virtual WriterIdentificationModel Model { get; set; }

    /// <summary>
    /// Gets or sets the dataset being analyzed.
    /// </summary>
    public virtual Dataset Dataset { get; set; }

    /// <summary>
    /// Gets or sets the user who created the task.
    /// </summary>
    public virtual User User { get; set; }
} 