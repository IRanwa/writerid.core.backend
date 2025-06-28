using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Task;
using WriterID.Dev.Portal.Model.DTOs.Dataset;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Service.Interfaces;

/// <summary>
/// The ITaskService interface.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Gets the analysis results for a dataset including the list of writers.
    /// </summary>
    /// <param name="datasetId">The dataset identifier.</param>
    /// <returns>The dataset analysis results with writers list.</returns>
    Task<DatasetAnalysisResultDto> GetDatasetAnalysisAsync(Guid datasetId);

    /// <summary>
    /// Creates a new writer identification task with selected writers and query image, then executes the prediction.
    /// </summary>
    /// <param name="dto">The task creation data.</param>
    /// <param name="userId">The ID of the user creating the task.</param>
    /// <returns>The initial prediction result if successful, null if failed.</returns>
    Task<TaskPredictionResultDto?> CreateTaskAsync(CreateTaskDto dto, int userId);

    /// <summary>
    /// Retrieves a task by its identifier.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task if found.</returns>
    Task<TaskDto> GetTaskByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all tasks for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A list of tasks for the user.</returns>
    Task<List<TaskDto>> GetUserTasksAsync(int userId);

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated task.</returns>
    Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto dto);

    /// <summary>
    /// Updates task results after processing completion.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="resultsJson">The results in JSON format.</param>
    /// <param name="status">The final status of the task.</param>
    Task UpdateTaskResultsAsync(Guid id, string resultsJson, ProcessingStatus status);

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    Task DeleteTaskAsync(Guid id);

    /// <summary>
    /// Starts the execution of a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The initial prediction result.</returns>
    Task<TaskPredictionResultDto> StartTaskAsync(Guid id);

    /// <summary>
    /// Gets task execution information including all container names for external processing.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <returns>The task execution information.</returns>
    Task<TaskExecutionInfoDto> GetTaskExecutionInfoAsync(Guid taskId);

    /// <summary>
    /// Gets the prediction results for a completed task.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <returns>The prediction results if the task is completed, null otherwise.</returns>
    Task<TaskPredictionResultDto?> GetTaskPredictionResultsAsync(Guid taskId);

    /// <summary>
    /// Submits prediction results for a task and marks it as completed.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="predictionResult">The prediction results.</param>
    Task SubmitTaskPredictionAsync(Guid taskId, TaskPredictionResultDto predictionResult);
} 