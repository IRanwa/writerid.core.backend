using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model.Queue;

/// <summary>
/// Queue message for requesting dataset analysis.
/// </summary>
public class DatasetAnalysisMessage : QueueMessageBase
{
    /// <summary>
    /// Gets or sets the ID of the dataset to analyze.
    /// </summary>
    [JsonPropertyName("datasetId")]
    public int DatasetId { get; set; }

    /// <summary>
    /// Gets or sets the analysis parameters.
    /// </summary>
    [JsonPropertyName("parameters")]
    public new DatasetAnalysisParameters Parameters { get; set; }
} 