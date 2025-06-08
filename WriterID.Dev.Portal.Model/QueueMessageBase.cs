using System.Text.Json.Serialization;

namespace WriterID.Dev.Portal.Model;

/// <summary>
/// Base class for queue messages.
/// </summary>
public abstract class QueueMessageBase
{
    /// <summary>
    /// Gets or sets the task type.
    /// </summary>
    [JsonPropertyName("task")]
    public string Task { get; set; }

    /// <summary>
    /// Gets or sets the task parameters.
    /// </summary>
    [JsonPropertyName("parameters")]
    public object Parameters { get; set; }
} 