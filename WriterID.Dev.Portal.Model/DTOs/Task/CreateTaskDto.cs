using System.ComponentModel.DataAnnotations;

namespace WriterID.Dev.Portal.Model.DTOs.Task;

/// <summary>
/// Data transfer object for creating a new writer identification task.
/// </summary>
public class CreateTaskDto
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
    /// Gets or sets the ID of the model to use for writer identification.
    /// </summary>
    [Required]
    public Guid ModelId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the dataset to analyze.
    /// </summary>
    [Required]
    public Guid DatasetId { get; set; }
} 