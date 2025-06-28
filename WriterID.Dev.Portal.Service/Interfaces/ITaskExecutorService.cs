using WriterID.Dev.Portal.Model.DTOs.Task;

namespace WriterID.Dev.Portal.Service.Interfaces;

/// <summary>
/// Interface for the task executor service.
/// </summary>
public interface ITaskExecutorService
{
    /// <summary>
    /// Starts the execution of a writer identification task by calling the executor service.
    /// </summary>
    /// <param name="taskId">The ID of the task to execute.</param>
    /// <returns>The initial prediction result that was stored in the database.</returns>
    Task<TaskPredictionResultDto> StartTaskExecutionAsync(Guid taskId);
} 