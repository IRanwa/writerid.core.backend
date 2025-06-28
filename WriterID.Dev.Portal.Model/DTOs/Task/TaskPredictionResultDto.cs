using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model.DTOs.Task;

/// <summary>
/// Data transfer object for task execution prediction results.
/// </summary>
public class TaskPredictionResultDto
{
    /// <summary>
    /// Gets or sets the task identifier.
    /// </summary>
    [JsonPropertyName("task_id")]
    public Guid TaskId { get; set; }

    /// <summary>
    /// Gets or sets the query image filename.
    /// </summary>
    [JsonPropertyName("query_image")]
    public string QueryImage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the prediction result.
    /// </summary>
    [JsonPropertyName("prediction")]
    public PredictionDto Prediction { get; set; } = new();
}

/// <summary>
/// Data transfer object for prediction details.
/// </summary>
public class PredictionDto
{
    /// <summary>
    /// Gets or sets the predicted writer ID.
    /// </summary>
    [JsonPropertyName("writer_id")]
    public string WriterId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the confidence score of the prediction.
    /// </summary>
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }
} 