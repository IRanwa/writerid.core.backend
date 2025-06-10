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
    /// Gets or sets the list of writers found in the dataset.
    /// </summary>
    public List<WriterInfo> Writers { get; set; } = new List<WriterInfo>();

    /// <summary>
    /// Gets or sets the date when the analysis was completed.
    /// </summary>
    public DateTime? AnalyzedAt { get; set; }
}

/// <summary>
/// Information about a writer found in the dataset analysis.
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