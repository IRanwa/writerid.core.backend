using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using WriterID.Dev.Portal.Model.Configuration;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// Service for communicating with the task executor.
/// </summary>
public class TaskExecutorService : ITaskExecutorService
{
    private readonly HttpClient httpClient;
    private readonly TaskExecutorSettings settings;
    private readonly ILogger<TaskExecutorService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskExecutorService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client for making requests.</param>
    /// <param name="settings">The task executor settings.</param>
    /// <param name="logger">The logger instance.</param>
    public TaskExecutorService(
        HttpClient httpClient,
        IOptions<TaskExecutorSettings> settings,
        ILogger<TaskExecutorService> logger)
    {
        this.httpClient = httpClient;
        this.settings = settings.Value;
        this.logger = logger;

        // Configure HTTP client timeout
        this.httpClient.Timeout = TimeSpan.FromSeconds(this.settings.TimeoutSeconds);
    }

    /// <summary>
    /// Starts the execution of a writer identification task by calling the executor service.
    /// </summary>
    /// <param name="taskId">The ID of the task to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StartTaskExecutionAsync(Guid taskId)
    {
        try
        {
            var requestBody = new
            {
                task_id = taskId.ToString()
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            logger.LogInformation("Sending task execution request to {Url} for task {TaskId}", 
                settings.PredictUrl, taskId);

            var response = await httpClient.PostAsync(settings.PredictUrl, content);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Successfully started task execution for task {TaskId}", taskId);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError("Failed to start task execution for task {TaskId}. Status: {StatusCode}, Response: {Response}",
                    taskId, response.StatusCode, errorContent);
                
                throw new HttpRequestException(
                    $"Task execution request failed with status {response.StatusCode}: {errorContent}");
            }
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            logger.LogError(ex, "Timeout occurred while starting task execution for task {TaskId}", taskId);
            throw new TimeoutException($"Task execution request timed out for task {taskId}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while starting task execution for task {TaskId}", taskId);
            throw;
        }
    }
} 