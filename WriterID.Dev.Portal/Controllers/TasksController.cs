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
    /// <returns>201 Created with prediction result if successful, 400 Bad Request if execution fails.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var predictionResult = await taskService.CreateTaskAsync(dto, CurrentUserId);
        
        if (predictionResult != null)
        {
            return StatusCode(201, new { 
                message = "Task created and prediction started successfully",
                prediction = predictionResult
            });
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
    /// <returns>An accepted response with prediction result if the task started successfully.</returns>
    [HttpPost("{id}/execute")]
    public async Task<IActionResult> StartExecution(Guid id)
    {
        var predictionResult = await taskService.StartTaskAsync(id);
        return Accepted(new { 
            message = "Task execution started", 
            taskId = id,
            prediction = predictionResult
        });
    }

    /// <summary>
    /// Submits prediction results for a task.
    /// Called by the task executor to provide prediction results directly.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="predictionResult">The prediction results.</param>
    /// <returns>An ok response if the prediction was stored successfully.</returns>
    [HttpPost("{id}/prediction")]
    public async Task<IActionResult> SubmitPredictionResults(Guid id, [FromBody] TaskPredictionResultDto predictionResult)
    {
        await taskService.SubmitTaskPredictionAsync(id, predictionResult);
        return Ok(new { message = "Prediction results submitted successfully", taskId = id });
    }

    /// <summary>
    /// Gets the prediction results for a completed task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The prediction results if available.</returns>
    [HttpGet("{id}/prediction")]
    public async Task<IActionResult> GetPredictionResults(Guid id)
    {
        var predictionResult = await taskService.GetTaskPredictionResultsAsync(id);
        
        if (predictionResult == null)
        {
            return NotFound(new { message = "Prediction results not available for this task" });
        }

        return Ok(predictionResult);
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