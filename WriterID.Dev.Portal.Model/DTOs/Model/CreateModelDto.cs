using System.ComponentModel.DataAnnotations;

namespace WriterID.Dev.Portal.Model.DTOs.Model;

/// <summary>
/// Data transfer object for creating a new model.
/// </summary>
public class CreateModelDto
{
    /// <summary>
    /// Gets or sets the name of the model.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the model.
    /// </summary>
    [StringLength(1000)]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the ID of the dataset to use for training.
    /// </summary>
    [Required]
    public int TrainingDatasetId { get; set; }
} 