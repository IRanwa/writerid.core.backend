using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model;

/// <summary>
/// Parameters for model training queue message.
/// </summary>
public class ModelTrainingParameters
{
    /// <summary>
    /// Gets or sets the name of the container containing the dataset.
    /// </summary>
    [JsonPropertyName("dataset_container_name")]
    public string DatasetContainerName { get; set; }

    /// <summary>
    /// Gets or sets the name of the container for storing the model.
    /// </summary>
    [JsonPropertyName("model_container_name")]
    public string ModelContainerName { get; set; }
} 