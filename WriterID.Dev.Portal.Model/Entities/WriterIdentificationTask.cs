using System.Text.Json;
using WriterID.Dev.Portal.Model.Enums;

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