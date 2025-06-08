using System.ComponentModel.DataAnnotations;

namespace WriterID.Dev.Portal.Model.DTOs.Dataset;

/// <summary>
/// Data transfer object for creating a new dataset.
/// </summary>
public class CreateDatasetDto
{
    /// <summary>
    /// Gets or sets the name of the dataset.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the dataset.
    /// </summary>
    [StringLength(1000)]
    public string Description { get; set; }
} 