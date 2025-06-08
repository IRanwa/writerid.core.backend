using Microsoft.AspNetCore.Mvc;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Task;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Controllers;

/// <summary>
/// The TasksController class.
/// </summary>
public class TasksController : BaseApiController
{
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
    [ProducesResponseType(typeof(WriterIdentificationTask), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var task = await taskService.CreateTaskAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    /// <summary>
    /// Retrieves a task by its identifier.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task if found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WriterIdentificationTask), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await taskService.GetTaskByIdAsync(id);
        return Ok(task);
    }

    /// <summary>
    /// Retrieves all tasks for the current user.
    /// </summary>
    /// <returns>A list of tasks.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<WriterIdentificationTask>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await taskService.GetUserTasksAsync(CurrentUserId);
        return Ok(tasks);
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated task.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(WriterIdentificationTask), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var task = await taskService.UpdateTaskAsync(id, dto);
        return Ok(task);
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>A no content response if successful.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
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
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartExecution(int id)
    {
        await taskService.StartTaskAsync(id);
        return Accepted();
    }
} 