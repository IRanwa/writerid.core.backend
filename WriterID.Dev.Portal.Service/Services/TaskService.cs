using AutoMapper;
using Microsoft.Extensions.Logging;
using WriterID.Dev.Portal.Data.Interfaces;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Task;
using WriterID.Dev.Portal.Core.Enums;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The TaskService class.
/// </summary>
public class TaskService : ITaskService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<TaskService> logger;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The mapper instance.</param>
    public TaskService(
        IUnitOfWork unitOfWork,
        ILogger<TaskService> logger,
        IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
        this.mapper = mapper;
    }

    /// <summary>
    /// Creates a new writer identification task.
    /// </summary>
    /// <param name="dto">The task creation data.</param>
    /// <param name="userId">The ID of the user creating the task.</param>
    /// <returns>The created task.</returns>
    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int userId)
    {
        var task = mapper.Map<WriterIdentificationTask>(dto);
        task.UserId = userId;
        task.Status = TaskExecutionStatus.Created;

        await unitOfWork.Tasks.AddAsync(task);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<TaskDto>(task);
    }

    /// <summary>
    /// Retrieves a task by its identifier.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task if found.</returns>
    public async Task<TaskDto> GetTaskByIdAsync(int id)
    {
        var task = await GetRawTaskByIdAsync(id);
        return mapper.Map<TaskDto>(task);
    }

    /// <summary>
    /// Retrieves all tasks for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A list of tasks for the user.</returns>
    public async Task<List<TaskDto>> GetUserTasksAsync(int userId)
    {
        var tasks = await unitOfWork.Tasks.FindAsync(t => t.UserId == userId && t.IsActive);
        var orderedTasks = tasks.OrderByDescending(t => t.CreatedAt).ToList();
        return mapper.Map<List<TaskDto>>(orderedTasks);
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated task.</returns>
    public async Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto dto)
    {
        var task = await GetRawTaskByIdAsync(id);

        mapper.Map(dto, task);
        task.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Tasks.Update(task);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<TaskDto>(task);
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    public async Task DeleteTaskAsync(int id)
    {
        var task = await GetRawTaskByIdAsync(id);

        task.IsActive = false;
        unitOfWork.Tasks.Update(task);
        await unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Starts the execution of a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    public async Task StartTaskAsync(int id)
    {
        var task = await GetRawTaskByIdAsync(id);

        task.Status = TaskExecutionStatus.Processing;
        task.UpdatedAt = DateTime.UtcNow;
        
        unitOfWork.Tasks.Update(task);
        await unitOfWork.SaveChangesAsync();
    }
    
    private async Task<WriterIdentificationTask> GetRawTaskByIdAsync(int id)
    {
        var task = await unitOfWork.Tasks.GetByIdAsync(id);
        if (task == null || !task.IsActive)
        {
            throw new KeyNotFoundException($"Task with ID {id} not found.");
        }
        return task;
    }
} 