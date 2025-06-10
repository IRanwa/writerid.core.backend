using Microsoft.AspNetCore.Mvc;
using WriterID.Dev.Portal.Model.DTOs.Task;
using WriterID.Dev.Portal.Core.Enums;
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
    /// Gets the analysis results for a dataset including available writers for selection.
    /// </summary>
    /// <param name="datasetId">The dataset identifier.</param>
    /// <returns>The dataset analysis results with writers list.</returns>
    [HttpGet("dataset/{datasetId}/analysis")]
    public async Task<IActionResult> GetDatasetAnalysis(Guid datasetId)
    {
        var analysis = await taskService.GetDatasetAnalysisAsync(datasetId);
        return Ok(analysis);
    }

    /// <summary>
    /// Creates a new writer identification task with selected writers and query image, then starts prediction.
    /// </summary>
    /// <param name="dto">The task creation data.</param>
    /// <returns>201 Created if successful, 400 Bad Request if execution fails.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var success = await taskService.CreateTaskAsync(dto, CurrentUserId);
        
        if (success)
        {
            return StatusCode(201, new { message = "Task created and prediction started successfully" });
        }
        else
        {
            return BadRequest(new { message = "Task created but failed to start prediction execution" });
        }
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
    /// Starts the execution of a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>An accepted response if the task started successfully.</returns>
    [HttpPost("{id}/execute")]
    public async Task<IActionResult> StartExecution(Guid id)
    {
        await taskService.StartTaskAsync(id);
        return Accepted(new { message = "Task execution started", taskId = id });
    }

    /// <summary>
    /// Updates task results after processing completion.
    /// Called by the task executor to provide results.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="request">The results update request.</param>
    /// <returns>An ok response if the update was successful.</returns>
    [HttpPost("{id}/results")]
    public async Task<IActionResult> UpdateResults(Guid id, [FromBody] TaskResultsUpdateRequest request)
    {
        await taskService.UpdateTaskResultsAsync(id, request.ResultsJson, request.Status);
        return Ok(new { message = "Task results updated", taskId = id });
    }

    /// <summary>
    /// Gets task execution details for the executor service.
    /// This endpoint provides the information needed by the task executor to process the task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task execution details.</returns>
    [HttpGet("{id}/execution-details")]
    public async Task<IActionResult> GetExecutionDetails(Guid id)
    {
        var taskDto = await taskService.GetTaskByIdAsync(id);
        
        var executionDetails = new
        {
            taskId = taskDto.Id,
            useDefaultModel = taskDto.UseDefaultModel,
            modelId = taskDto.ModelId,
            datasetId = taskDto.DatasetId,
            selectedWriters = taskDto.SelectedWriters,
            queryImagePath = taskDto.QueryImagePath,
            status = taskDto.Status.ToString(),
            createdAt = taskDto.CreatedAt,
            name = taskDto.Name,
            description = taskDto.Description
        };

        return Ok(executionDetails);
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
}

/// <summary>
/// Request model for updating task results.
/// </summary>
public class TaskResultsUpdateRequest
{
    /// <summary>
    /// Gets or sets the results in JSON format.
    /// </summary>
    public string ResultsJson { get; set; }

    /// <summary>
    /// Gets or sets the final status of the task.
    /// </summary>
    public ProcessingStatus Status { get; set; }
} 