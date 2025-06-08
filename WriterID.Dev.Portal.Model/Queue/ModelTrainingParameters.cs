using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model.Queue;

/// <summary>
/// Parameters for model training queue message.
/// </summary>
public class ModelTrainingParameters
{
    /// <summary>
    /// Gets or sets the number of epochs to train for.
    /// </summary>
    [JsonPropertyName("epochs")]
    public int Epochs { get; set; }

    /// <summary>
    /// Gets or sets the batch size for training.
    /// </summary>
    [JsonPropertyName("batch_size")]
    public int BatchSize { get; set; }

    /// <summary>
    /// Gets or sets the learning rate for training.
    /// </summary>
    [JsonPropertyName("learning_rate")]
    public double LearningRate { get; set; }
} 