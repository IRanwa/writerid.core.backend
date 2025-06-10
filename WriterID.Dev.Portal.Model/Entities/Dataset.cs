using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Model.Entities;

/// <summary>
/// Represents a dataset in the writer identification system.
/// </summary>
public class Dataset
{
    /// <summary>
    /// Gets or sets the unique identifier for the dataset.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the dataset.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the dataset.
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the Azure Storage container name where the dataset files are stored.
    /// </summary>
    [Required]
    public string ContainerName { get; set; }

    /// <summary>
    /// Gets or sets the file name of the uploaded dataset.
    /// </summary>
    [Required]
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the size of the uploaded file in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the current processing status of the dataset.
    /// </summary>
    [Required]
    public DatasetStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the processing status for analysis operations.
    /// </summary>
    [Required]
    public ProcessingStatus ProcessingStatus { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who created the dataset.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the dataset was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the dataset was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the dataset is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the user who created the dataset.
    /// </summary>
    public virtual User User { get; set; }
} 