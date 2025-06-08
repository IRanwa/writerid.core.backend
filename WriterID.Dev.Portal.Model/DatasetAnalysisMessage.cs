using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model;

/// <summary>
/// Queue message for requesting dataset analysis.
/// </summary>
public class DatasetAnalysisMessage : QueueMessageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetAnalysisMessage"/> class.
    /// </summary>
    /// <param name="containerName">The name of the container containing the dataset.</param>
    public DatasetAnalysisMessage(string containerName)
    {
        Task = "analyze_dataset";
        Parameters = new DatasetAnalysisParameters
        {
            ContainerName = containerName
        };
    }
} 