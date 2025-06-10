using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model.Queue;

/// <summary>
/// Queue message for requesting model training.
/// </summary>
public class ModelTrainingMessage : QueueMessageBase
{
    /// <summary>
    /// Gets or sets the ID of the model to train.
    /// </summary>
    [JsonPropertyName("modelId")]
    public Guid ModelId { get; set; }

    /// <summary>
    /// Gets or sets the training parameters.
    /// </summary>
    [JsonPropertyName("parameters")]
    public new ModelTrainingParameters Parameters { get; set; }
} 