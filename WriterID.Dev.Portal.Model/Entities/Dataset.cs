using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    /// Gets or sets the Azure Storage container name where the dataset files are stored.
    /// </summary>
    [Required]
    public string ContainerName { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    [Required]
    public ProcessingStatus Status { get; set; }

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
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }
}