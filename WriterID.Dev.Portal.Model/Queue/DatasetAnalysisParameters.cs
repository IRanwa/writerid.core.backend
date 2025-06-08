using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model.Queue;

/// <summary>
/// Parameters for dataset analysis queue message.
/// </summary>
public class DatasetAnalysisParameters
{
    /// <summary>
    /// Gets or sets the minimum confidence threshold for writer identification.
    /// </summary>
    [JsonPropertyName("confidence_threshold")]
    public double ConfidenceThreshold { get; set; }

    /// <summary>
    /// Gets or sets whether to perform preprocessing on the dataset.
    /// </summary>
    [JsonPropertyName("preprocess")]
    public bool Preprocess { get; set; }

    /// <summary>
    /// Gets or sets whether to extract additional features from the dataset.
    /// </summary>
    [JsonPropertyName("extract_features")]
    public bool ExtractFeatures { get; set; }
} 