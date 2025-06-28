using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model.DTOs.Dataset;

/// <summary>
/// Data transfer object for dataset analysis results.
/// </summary>
public class DatasetAnalysisResultDto
{
    /// <summary>
    /// Gets or sets the dataset ID.
    /// </summary>
    public Guid DatasetId { get; set; }

    /// <summary>
    /// Gets or sets the dataset name.
    /// </summary>
    public string DatasetName { get; set; }

    /// <summary>
    /// Gets or sets the analysis status.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the number of writers found in the dataset.
    /// </summary>
    [JsonPropertyName("num_writers")]
    public int NumWriters { get; set; }

    /// <summary>
    /// Gets or sets the list of writer names found in the dataset.
    /// </summary>
    [JsonPropertyName("writer_names")]
    public List<string> WriterNames { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the minimum number of samples across all writers.
    /// </summary>
    [JsonPropertyName("min_samples")]
    public int MinSamples { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of samples across all writers.
    /// </summary>
    [JsonPropertyName("max_samples")]
    public int MaxSamples { get; set; }

    /// <summary>
    /// Gets or sets the dictionary of writer names and their sample counts.
    /// </summary>
    [JsonPropertyName("writer_counts")]
    public Dictionary<string, int> WriterCounts { get; set; } = new Dictionary<string, int>();

    /// <summary>
    /// Gets or sets the date when the analysis was completed.
    /// </summary>
    public DateTime? AnalyzedAt { get; set; }

    /// <summary>
    /// Gets or sets the list of writers found in the dataset (legacy support).
    /// This property maintains backward compatibility.
    /// </summary>
    [JsonIgnore]
    public List<WriterInfo> Writers 
    { 
        get
        {
            return WriterNames.Select(name => new WriterInfo
            {
                WriterId = name,
                WriterName = name,
                SampleCount = WriterCounts.ContainsKey(name) ? WriterCounts[name] : 0,
                Confidence = 1.0 // Default confidence for analyzed writers
            }).ToList();
        }
        set
        {
            // Update the new properties based on legacy WriterInfo list
            if (value != null && value.Any())
            {
                WriterNames = value.Select(w => w.WriterName).ToList();
                WriterCounts = value.ToDictionary(w => w.WriterName, w => w.SampleCount);
                NumWriters = value.Count;
                MinSamples = value.Min(w => w.SampleCount);
                MaxSamples = value.Max(w => w.SampleCount);
            }
        }
    }
}

/// <summary>
/// Information about a writer found in the dataset analysis (legacy support).
/// </summary>
public class WriterInfo
{
    /// <summary>
    /// Gets or sets the writer ID.
    /// </summary>
    public string WriterId { get; set; }

    /// <summary>
    /// Gets or sets the writer name or label.
    /// </summary>
    public string WriterName { get; set; }

    /// <summary>
    /// Gets or sets the number of samples for this writer.
    /// </summary>
    public int SampleCount { get; set; }

    /// <summary>
    /// Gets or sets the confidence level of writer identification in the dataset.
    /// </summary>
    public double Confidence { get; set; }
} 