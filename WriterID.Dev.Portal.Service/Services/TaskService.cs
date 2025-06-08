using Microsoft.Extensions.Logging;
using WriterID.Dev.Portal.Data.Interfaces;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Task;
using WriterID.Dev.Portal.Model.Enums;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The TaskService class.
/// </summary>
public class TaskService : ITaskService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<TaskService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="logger">The logger instance.</param>
    public TaskService(
        IUnitOfWork unitOfWork,
        ILogger<TaskService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    /// <summary>
    /// Creates a new writer identification task.
    /// </summary>
    /// <param name="dto">The task creation data.</param>
    /// <param name="userId">The ID of the user creating the task.</param>
    /// <returns>The created task.</returns>
    public async Task<WriterIdentificationTask> CreateTaskAsync(CreateTaskDto dto, int userId)
    {
        var task = new WriterIdentificationTask
        {
            Name = dto.Name,
            Description = dto.Description,
            ModelId = dto.ModelId,
            DatasetId = dto.DatasetId,
            Status = WriterID.Dev.Portal.Model.Enums.TaskStatus.Created,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await unitOfWork.Tasks.AddAsync(task);
        await unitOfWork.SaveChangesAsync();

        return task;
    }

    /// <summary>
    /// Retrieves a task by its identifier.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task if found.</returns>
    public async Task<WriterIdentificationTask> GetTaskByIdAsync(int id)
    {
        var task = await unitOfWork.Tasks.GetByIdAsync(id);

        if (task == null)
        {
            throw new KeyNotFoundException($"Task with ID {id} not found.");
        }

        return task;
    }

    /// <summary>
    /// Retrieves all tasks for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A list of tasks for the user.</returns>
    public async Task<List<WriterIdentificationTask>> GetUserTasksAsync(int userId)
    {
        var tasks = await unitOfWork.Tasks.FindAsync(t => t.UserId == userId);
        return tasks.OrderByDescending(t => t.CreatedAt).ToList();
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated task.</returns>
    public async Task<WriterIdentificationTask> UpdateTaskAsync(int id, UpdateTaskDto dto)
    {
        var task = await GetTaskByIdAsync(id);

        task.Name = dto.Name;
        task.Description = dto.Description;
        task.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Tasks.Update(task);
        await unitOfWork.SaveChangesAsync();

        return task;
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    public async Task DeleteTaskAsync(int id)
    {
        var task = await GetTaskByIdAsync(id);

        unitOfWork.Tasks.Remove(task);
        await unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Starts the execution of a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    public async Task StartTaskAsync(int id)
    {
        var task = await GetTaskByIdAsync(id);

        task.Status = WriterID.Dev.Portal.Model.Enums.TaskStatus.Processing;
        task.UpdatedAt = DateTime.UtcNow;
        
        unitOfWork.Tasks.Update(task);
        await unitOfWork.SaveChangesAsync();
    }
} 