using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using WriterID.Dev.Portal.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WriterID.Dev.Portal.Model.Entities;

/// <summary>
/// Represents a writer identification task in the system.
/// </summary>
public class WriterIdentificationTask
{
    /// <summary>
    /// Gets or sets the unique identifier for the task.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

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
    /// Gets or sets a value indicating whether this task uses the default model.
    /// If true, the default model from platform-config container (default_model.pth) is used.
    /// </summary>
    public bool UseDefaultModel { get; set; } = true;

    /// <summary>
    /// Gets or sets the ID of the custom model used for writer identification.
    /// This is null when UseDefaultModel is true.
    /// </summary>
    public Guid? ModelId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the dataset to analyze.
    /// </summary>
    public Guid DatasetId { get; set; }

    /// <summary>
    /// Gets or sets the list of selected writer IDs for comparison.
    /// These are the specific writers selected from the dataset analysis results.
    /// </summary>
    public List<string> SelectedWriters { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the query image file path or blob URL.
    /// This is the image to be compared against the selected writers.
    /// </summary>
    [MaxLength(500)]
    public string QueryImagePath { get; set; }

    /// <summary>
    /// Gets or sets the current processing status of the task.
    /// </summary>
    public ProcessingStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the task results in JSON format.
    /// Contains the writer identification results with confidence scores.
    /// </summary>
    public string ResultsJson { get; set; }

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
    /// Gets or sets the custom model used for writer identification.
    /// This is null when UseDefaultModel is true.
    /// </summary>
    [ForeignKey(nameof(ModelId))]
    public virtual WriterIdentificationModel Model { get; set; }

    /// <summary>
    /// Gets or sets the dataset being analyzed.
    /// </summary>
    [ForeignKey(nameof(DatasetId))]
    public virtual Dataset Dataset { get; set; }

    /// <summary>
    /// Gets or sets the user who created the task.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }
} 