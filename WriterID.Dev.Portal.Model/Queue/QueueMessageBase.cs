using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model.Queue;

/// <summary>
/// Base class for queue messages.
/// </summary>
public abstract class QueueMessageBase
{
    /// <summary>
    /// Gets or sets the type of task to be performed.
    /// </summary>
    [JsonPropertyName("task")]
    public string TaskType { get; set; }

    /// <summary>
    /// Gets or sets the task parameters.
    /// </summary>
    [JsonPropertyName("parameters")]
    public object Parameters { get; set; }
} 