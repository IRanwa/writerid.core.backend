using System.Text.Json;
using System.Text.Json.Serialization;
using WriterID.Dev.Portal.Core.Enums;
using WriterID.Dev.Portal.Model.Entities;

namespace WriterID.Dev.Portal.Model.DTOs.Task;

/// <summary>
/// Data transfer object for writer identification task.
/// </summary>
public class TaskDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the task.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the task.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this task uses the default model.
    /// </summary>
    public bool UseDefaultModel { get; set; }

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
    /// </summary>
    public List<string> SelectedWriters { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the query image file path or blob URL.
    /// </summary>
    public string QueryImagePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current processing status of the task.
    /// </summary>
    public ProcessingStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the task results in JSON format.
    /// </summary>
    public string ResultsJson { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the user who created the task.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the task was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the task was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets the parsed prediction result from the ResultsJson.
    /// Returns null if the task is not completed or results are not available.
    /// </summary>
    [JsonIgnore]
    public PredictionDto? PredictionResult
    {
        get
        {
            if (string.IsNullOrEmpty(ResultsJson) || Status != ProcessingStatus.Completed)
                return null;

            try
            {
                var predictionResult = JsonSerializer.Deserialize<TaskPredictionResultDto>(ResultsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return predictionResult?.Prediction;
            }
            catch
            {
                return null;
            }
        }
    }
} 