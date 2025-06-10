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
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StartTaskExecutionAsync(Guid taskId);
} 