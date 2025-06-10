namespace WriterID.Dev.Portal.Model.Configuration;

/// <summary>
/// Configuration settings for the task executor service.
/// </summary>
public class TaskExecutorSettings
{
    /// <summary>
    /// Gets or sets the base URL of the task executor service.
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:5000";

    /// <summary>
    /// Gets or sets the predict endpoint path.
    /// </summary>
    public string PredictEndpoint { get; set; } = "/predict";

    /// <summary>
    /// Gets or sets the timeout in seconds for HTTP requests.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 300;

    /// <summary>
    /// Gets the full URL for the predict endpoint.
    /// </summary>
    public string PredictUrl => $"{BaseUrl.TrimEnd('/')}{PredictEndpoint}";
} 