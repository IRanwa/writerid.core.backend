using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model.DTOs.Dataset;

/// <summary>
/// DTO for dataset analysis results.
/// </summary>
public class DatasetAnalysisResultDto
{
    /// <summary>
    /// Gets or sets the number of writers in the dataset.
    /// </summary>
    [JsonPropertyName("num_writers")]
    public int NumWriters { get; set; }

    /// <summary>
    /// Gets or sets the list of writer names.
    /// </summary>
    [JsonPropertyName("writer_names")]
    public List<string> WriterNames { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the minimum number of samples per writer.
    /// </summary>
    [JsonPropertyName("min_samples")]
    public int MinSamples { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of samples per writer.
    /// </summary>
    [JsonPropertyName("max_samples")]
    public int MaxSamples { get; set; }

    /// <summary>
    /// Gets or sets the dictionary containing writer names and their sample counts.
    /// </summary>
    [JsonPropertyName("writer_counts")]
    public Dictionary<string, int> WriterCounts { get; set; } = new Dictionary<string, int>();
} 