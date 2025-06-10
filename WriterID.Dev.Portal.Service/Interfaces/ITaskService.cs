using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Task;

namespace WriterID.Dev.Portal.Service.Interfaces;

/// <summary>
/// The ITaskService interface.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Creates a new writer identification task.
    /// </summary>
    /// <param name="dto">The task creation data.</param>
    /// <param name="userId">The ID of the user creating the task.</param>
    /// <returns>The created task.</returns>
    Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int userId);

    /// <summary>
    /// Retrieves a task by its identifier.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task if found.</returns>
    Task<TaskDto> GetTaskByIdAsync(int id);

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
    Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto dto);

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    Task DeleteTaskAsync(int id);

    /// <summary>
    /// Starts the execution of a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    Task StartTaskAsync(int id);
} 