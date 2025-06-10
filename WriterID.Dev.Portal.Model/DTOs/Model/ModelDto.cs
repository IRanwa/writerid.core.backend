using System.Text.Json;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Model.DTOs.Model;

/// <summary>
/// Data transfer object for a writer identification model.
/// </summary>
public class ModelDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the model.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the model.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the model.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the current status of the model.
    /// </summary>
    public ProcessingStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the ID of the dataset used for training this model.
    /// </summary>
    public int TrainingDatasetId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who created the model.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the model was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the model was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
} 