using Microsoft.AspNetCore.Mvc;
using WriterID.Dev.Portal.Model.DTOs.Task;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Controllers;

/// <summary>
/// The TasksController class.
/// </summary>
public class TasksController : BaseApiController
{
    /// <summary>
    /// The task service
    /// </summary>
    private readonly ITaskService taskService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TasksController"/> class.
    /// </summary>
    /// <param name="taskService">The service for managing task operations.</param>
    public TasksController(ITaskService taskService)
    {
        this.taskService = taskService;
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="dto">The task creation data.</param>
    /// <returns>A 201 Created response containing the created task.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var taskDto = await taskService.CreateTaskAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = taskDto.Id }, taskDto);
    }

    /// <summary>
    /// Retrieves a task by its identifier.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task if found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var taskDto = await taskService.GetTaskByIdAsync(id);
        return Ok(taskDto);
    }

    /// <summary>
    /// Retrieves all tasks for the current user.
    /// </summary>
    /// <returns>A list of tasks.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var taskDtos = await taskService.GetUserTasksAsync(CurrentUserId);
        return Ok(taskDtos);
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated task.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var taskDto = await taskService.UpdateTaskAsync(id, dto);
        return Ok(taskDto);
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>A no content response if successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await taskService.DeleteTaskAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Starts the execution of a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>An accepted response if the task started successfully.</returns>
    [HttpPost("{id}/execute")]
    public async Task<IActionResult> StartExecution(Guid id)
    {
        await taskService.StartTaskAsync(id);
        return Accepted();
    }
} 