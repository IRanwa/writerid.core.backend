using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Model.Entities;

/// <summary>
/// Represents a machine learning model in the writer identification system.
/// </summary>
public class WriterIdentificationModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the model.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of the model.
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Azure Storage container name where the model files are stored.
    /// </summary>
    public string ContainerName { get; set; }

    /// <summary>
    /// Gets or sets the current processing status of the model.
    /// </summary>
    public ProcessingStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the ID of the dataset used for training this model.
    /// </summary>
    public Guid TrainingDatasetId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who created the model.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the model was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the model was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the model is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the dataset used for training this model.
    /// </summary>
    [ForeignKey(nameof(TrainingDatasetId))]
    public virtual Dataset TrainingDataset { get; set; }

    /// <summary>
    /// Gets or sets the user who created the model.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }
} 