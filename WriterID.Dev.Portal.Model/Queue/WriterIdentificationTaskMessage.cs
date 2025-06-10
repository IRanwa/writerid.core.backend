using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model.Queue;

/// <summary>
/// Queue message for requesting writer identification task execution.
/// </summary>
public class WriterIdentificationTaskMessage : QueueMessageBase
{
    /// <summary>
    /// Gets or sets the ID of the task to execute.
    /// </summary>
    [JsonPropertyName("taskId")]
    public Guid TaskId { get; set; }

    /// <summary>
    /// Gets or sets the task execution parameters.
    /// </summary>
    [JsonPropertyName("parameters")]
    public new WriterIdentificationTaskParameters Parameters { get; set; }
}

/// <summary>
/// Parameters for writer identification task execution.
/// </summary>
public class WriterIdentificationTaskParameters
{
    /// <summary>
    /// Gets or sets a value indicating whether to use the default model.
    /// If true, the default model from platform-config container (default_model.pth) is used.
    /// </summary>
    [JsonPropertyName("use_default_model")]
    public bool UseDefaultModel { get; set; }

    /// <summary>
    /// Gets or sets the container name of the custom model.
    /// This is only used when UseDefaultModel is false.
    /// </summary>
    [JsonPropertyName("model_container_name")]
    public string ModelContainerName { get; set; }

    /// <summary>
    /// Gets or sets the container name of the dataset to analyze.
    /// </summary>
    [JsonPropertyName("dataset_container_name")]
    public string DatasetContainerName { get; set; }

    /// <summary>
    /// Gets or sets the minimum confidence threshold for writer identification.
    /// </summary>
    [JsonPropertyName("confidence_threshold")]
    public double ConfidenceThreshold { get; set; } = 0.7;

    /// <summary>
    /// Gets or sets whether to perform preprocessing on the dataset.
    /// </summary>
    [JsonPropertyName("preprocess")]
    public bool Preprocess { get; set; } = true;
} 