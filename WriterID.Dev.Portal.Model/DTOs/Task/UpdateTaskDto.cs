using System.ComponentModel.DataAnnotations;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Model.DTOs.Task;

/// <summary>
/// Data transfer object for updating a task.
/// </summary>
public class UpdateTaskDto
{
    /// <summary>
    /// Gets or sets the name of the task.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    [StringLength(1000)]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use the default model.
    /// </summary>
    public bool UseDefaultModel { get; set; }

    /// <summary>
    /// Gets or sets the ID of the custom model to use for writer identification.
    /// This is only used when UseDefaultModel is false.
    /// </summary>
    public Guid? ModelId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the dataset to analyze.
    /// </summary>
    public Guid DatasetId { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public ProcessingStatus Status { get; set; }
} 