using System.Text.Json;
using WriterID.Dev.Portal.Model.Enums;
using WriterID.Dev.Portal.Model.Entities;

namespace WriterID.Dev.Portal.Model.DTOs.Dataset;

/// <summary>
/// Data transfer object for dataset information.
/// </summary>
public class DatasetDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the dataset.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the dataset.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the dataset.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the Azure Storage container name where the dataset files are stored.
    /// </summary>
    public string ContainerName { get; set; }

    /// <summary>
    /// Gets or sets the current processing status of the dataset.
    /// </summary>
    public DatasetStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the analysis results for the dataset in JSON format.
    /// </summary>
    public JsonDocument AnalysisResult { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who created the dataset.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the dataset was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the dataset was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Creates a new instance of DatasetDto from a Dataset entity.
    /// </summary>
    /// <param name="dataset">The dataset entity to convert.</param>
    /// <returns>A new DatasetDto instance.</returns>
    public static DatasetDto FromEntity(Entities.Dataset dataset)
    {
        return new DatasetDto
        {
            Id = dataset.Id,
            Name = dataset.Name,
            Description = dataset.Description,
            ContainerName = dataset.ContainerName,
            Status = dataset.Status,
            AnalysisResult = dataset.AnalysisResult,
            UserId = dataset.UserId,
            CreatedAt = dataset.CreatedAt,
            UpdatedAt = dataset.UpdatedAt
        };
    }
} 