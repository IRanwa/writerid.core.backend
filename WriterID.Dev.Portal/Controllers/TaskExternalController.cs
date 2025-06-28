using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WriterID.Dev.Portal.Model.DTOs.External;
using WriterID.Dev.Portal.Service.Interfaces;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Controllers;

/// <summary>
/// External API controller for task operations by external services.
/// </summary>
[ApiController]
[Route("api/external/tasks")]
[Authorize(AuthenticationSchemes = "ApiKey")]
public class TaskExternalController : ControllerBase
{
    private readonly ITaskService taskService;
    private readonly ILogger<TaskExternalController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskExternalController"/> class.
    /// </summary>
    /// <param name="taskService">The task service.</param>
    /// <param name="logger">The logger.</param>
    public TaskExternalController(ITaskService taskService, ILogger<TaskExternalController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    /// <summary>
    /// Gets task execution information including all container names for external processing.
    /// This endpoint provides the container names and execution details needed by external task executors.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task execution information with container names.</returns>
    [HttpGet("{id}/execution-info")]
    public async Task<IActionResult> GetExecutionInfo(Guid id)
    {
        try
        {
            var executionInfo = await taskService.GetTaskExecutionInfoAsync(id);
            
            logger.LogInformation("External service retrieved execution info for task {TaskId}", id);
            
            return Ok(executionInfo);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning("Task {TaskId} not found for execution info: {Message}", id, ex.Message);
            return NotFound(new { message = "Task not found" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting execution info for task {TaskId}", id);
            return StatusCode(500, new { message = "Internal server error occurred while getting task execution info" });
        }
    }

    /// <summary>
    /// Updates the status of a task and optionally provides results.
    /// </summary>
    /// <param name="request">The status update request.</param>
    /// <returns>A response indicating success or failure.</returns>
    [HttpPut("status")]
    public async Task<IActionResult> UpdateTaskStatus([FromBody] UpdateTaskStatusDto request)
    {
        try
        {
            logger.LogInformation("External service updating task {TaskId} status to {Status}", 
                request.TaskId, request.Status);

            // Update the task status and results if provided
            if (!string.IsNullOrEmpty(request.ResultsJson))
            {
                await taskService.UpdateTaskResultsAsync(request.TaskId, request.ResultsJson, request.Status);
            }
            else
            {
                // Get the task first to verify it exists and update status only
                var task = await taskService.GetTaskByIdAsync(request.TaskId);
                if (task == null)
                {
                    logger.LogWarning("Task {TaskId} not found for status update", request.TaskId);
                    return NotFound(new { message = "Task not found" });
                }

                await taskService.UpdateTaskResultsAsync(request.TaskId, task.ResultsJson, request.Status);
            }

            logger.LogInformation("Successfully updated task {TaskId} status to {Status}", 
                request.TaskId, request.Status);

            return Ok(new 
            { 
                message = "Task status updated successfully",
                taskId = request.TaskId,
                status = request.Status.ToString(),
                updatedAt = DateTime.UtcNow
            });
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning("Task {TaskId} not found for status update: {Message}", request.TaskId, ex.Message);
            return NotFound(new { message = "Task not found" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating task {TaskId} status", request.TaskId);
            return StatusCode(500, new { message = "Internal server error occurred while updating task status" });
        }
    }

    /// <summary>
    /// Gets the status of a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task status information.</returns>
    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetTaskStatus(Guid id)
    {
        try
        {
            var task = await taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                logger.LogWarning("Task {TaskId} not found for status check", id);
                return NotFound(new { message = "Task not found" });
            }

            return Ok(new 
            { 
                taskId = task.Id,
                status = task.Status.ToString(),
                useDefaultModel = task.UseDefaultModel,
                selectedWriters = task.SelectedWriters,
                queryImagePath = task.QueryImagePath,
                resultsJson = task.ResultsJson,
                createdAt = task.CreatedAt,
                updatedAt = task.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting task {TaskId} status", id);
            return StatusCode(500, new { message = "Internal server error occurred while getting task status" });
        }
    }
} 