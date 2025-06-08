using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model;

/// <summary>
/// Parameters for dataset analysis queue message.
/// </summary>
public class DatasetAnalysisParameters
{
    /// <summary>
    /// Gets or sets the name of the container containing the dataset.
    /// </summary>
    [JsonPropertyName("container_name")]
    public string ContainerName { get; set; }
} 