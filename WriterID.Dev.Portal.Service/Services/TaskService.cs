using AutoMapper;
using Microsoft.Extensions.Logging;
using WriterID.Dev.Portal.Data.Interfaces;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Task;
using WriterID.Dev.Portal.Model.DTOs.Dataset;
using WriterID.Dev.Portal.Core.Enums;
using WriterID.Dev.Portal.Service.Interfaces;
using System.Text.Json;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The TaskService class.
/// </summary>
public class TaskService : ITaskService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ITaskExecutorService taskExecutorService;
    private readonly IBlobService blobService;
    private readonly ILogger<TaskService> logger;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="taskExecutorService">The task executor service.</param>
    /// <param name="blobService">The blob service for accessing Azure Storage.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The mapper instance.</param>
    public TaskService(
        IUnitOfWork unitOfWork,
        ITaskExecutorService taskExecutorService,
        IBlobService blobService,
        ILogger<TaskService> logger,
        IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.taskExecutorService = taskExecutorService;
        this.blobService = blobService;
        this.logger = logger;
        this.mapper = mapper;
    }

    /// <summary>
    /// Gets the analysis results for a dataset including the list of writers.
    /// </summary>
    /// <param name="datasetId">The dataset identifier.</param>
    /// <returns>The dataset analysis results with writers list.</returns>
    public async Task<DatasetAnalysisResultDto> GetDatasetAnalysisAsync(Guid datasetId)
    {
        var dataset = await unitOfWork.Datasets.GetByIdAsync(datasetId);
        if (dataset == null || !dataset.IsActive)
        {
            throw new KeyNotFoundException($"Dataset with ID {datasetId} not found.");
        }

        if (dataset.Status != ProcessingStatus.Completed)
        {
            throw new InvalidOperationException("Analysis results are only available for completed datasets.");
        }

        // Try to download the analysis results from Azure Storage
        var analysisResultsJson = await blobService.DownloadFileAsStringAsync(dataset.ContainerName, "analysis-results.json");
        
        if (string.IsNullOrEmpty(analysisResultsJson))
        {
            logger.LogWarning("Analysis results file not found for dataset {DatasetId}", datasetId);
            
            return new DatasetAnalysisResultDto
            {
                DatasetId = dataset.Id,
                DatasetName = dataset.Name,
                Status = dataset.Status.ToString(),
                AnalyzedAt = dataset.UpdatedAt,
                NumWriters = 0,
                WriterNames = new List<string>(),
                MinSamples = 0,
                MaxSamples = 0,
                WriterCounts = new Dictionary<string, int>()
            };
        }

        // Deserialize the JSON directly to the DTO structure
        var analysisResult = JsonSerializer.Deserialize<DatasetAnalysisResultDto>(analysisResultsJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Set the dataset metadata
        analysisResult.DatasetId = dataset.Id;
        analysisResult.DatasetName = dataset.Name;
        analysisResult.Status = dataset.Status.ToString();
        analysisResult.AnalyzedAt = dataset.UpdatedAt;
        
        logger.LogInformation("Retrieved {WriterCount} writers for dataset {DatasetId}", analysisResult.NumWriters, datasetId);
        
        return analysisResult;
    }

    /// <summary>
    /// Creates a new writer identification task with selected writers and query image, then executes the prediction.
    /// </summary>
    /// <param name="dto">The task creation data.</param>
    /// <param name="userId">The ID of the user creating the task.</param>
    /// <returns>The initial prediction result if successful, null if failed.</returns>
    public async Task<TaskPredictionResultDto?> CreateTaskAsync(CreateTaskDto dto, int userId)
    {
        // Validate dataset exists
        var dataset = await unitOfWork.Datasets.GetByIdAsync(dto.DatasetId);
        if (dataset == null || !dataset.IsActive)
        {
            throw new KeyNotFoundException($"Dataset with ID {dto.DatasetId} not found.");
        }

        // Validate model if custom model is specified
        if (!dto.UseDefaultModel && dto.ModelId.HasValue)
        {
            var model = await unitOfWork.Models.GetByIdAsync(dto.ModelId.Value);
            if (model == null || !model.IsActive)
                throw new KeyNotFoundException($"Model with ID {dto.ModelId.Value} not found.");
            
            if (model.UserId != userId)
                throw new UnauthorizedAccessException("You can only use models that you own.");
        }

        var task = mapper.Map<WriterIdentificationTask>(dto);
        task.UserId = userId;
        task.Status = ProcessingStatus.Created;

        await unitOfWork.Tasks.AddAsync(task);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Created writer identification task {TaskId} with {WriterCount} selected writers", 
            task.Id, dto.SelectedWriters.Count);

        // Create task container and upload query image
        var containerName = await blobService.CreateTaskContainerAsync(task.Id);
        var queryImagePath = await blobService.UploadBase64ImageAsync(containerName, "query.png", dto.QueryImageBase64);
        
        // Update task with the query image path
        task.QueryImagePath = queryImagePath;
        unitOfWork.Tasks.Update(task);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Uploaded query image for task {TaskId} to {ImagePath}", task.Id, queryImagePath);

        // Immediately start task execution
        try
        {
            // Update task status to processing
            task.Status = ProcessingStatus.Processing;
            task.UpdatedAt = DateTime.UtcNow;
            
            unitOfWork.Tasks.Update(task);
            await unitOfWork.SaveChangesAsync();

            // Call the task executor service to start prediction
            var initialPrediction = await taskExecutorService.StartTaskExecutionAsync(task.Id);

            // Store the initial prediction result in the database
            await SubmitTaskPredictionAsync(task.Id, initialPrediction);

            logger.LogInformation("Started prediction execution for task {TaskId} using {ModelType}", 
                task.Id, 
                task.UseDefaultModel ? "default model" : $"custom model (ID: {task.ModelId})");

            // Return the initial prediction result
            return initialPrediction;
        }
        catch (Exception ex)
        {
            // If execution fails, update task status to failed
            logger.LogError(ex, "Failed to start prediction execution for task {TaskId}", task.Id);
            
            task.Status = ProcessingStatus.Failed;
            task.UpdatedAt = DateTime.UtcNow;
            
            unitOfWork.Tasks.Update(task);
            await unitOfWork.SaveChangesAsync();
            
            // Return null to indicate failure
            return null;
        }
    }

    /// <summary>
    /// Retrieves a task by its identifier.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task if found.</returns>
    public async Task<TaskDto> GetTaskByIdAsync(Guid id)
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
    public async Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto dto)
    {
        var task = await GetRawTaskByIdAsync(id);

        task.Status = dto.Status;
        task.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Tasks.Update(task);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<TaskDto>(task);
    }

    /// <summary>
    /// Updates task results after processing completion.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="resultsJson">The results in JSON format.</param>
    /// <param name="status">The final status of the task.</param>
    public async Task UpdateTaskResultsAsync(Guid id, string resultsJson, ProcessingStatus status)
    {
        var task = await GetRawTaskByIdAsync(id);

        task.ResultsJson = resultsJson;
        task.Status = status;
        task.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Tasks.Update(task);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Updated task {TaskId} with status {Status}", id, status);
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    public async Task DeleteTaskAsync(Guid id)
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
    /// <returns>The initial prediction result.</returns>
    public async Task<TaskPredictionResultDto> StartTaskAsync(Guid id)
    {
        var task = await GetRawTaskByIdAsync(id);
        
        // Update task status to processing before calling executor
        task.Status = ProcessingStatus.Processing;
        task.UpdatedAt = DateTime.UtcNow;
        
        unitOfWork.Tasks.Update(task);
        await unitOfWork.SaveChangesAsync();

        try
        {
            // Call the task executor service
            var initialPrediction = await taskExecutorService.StartTaskExecutionAsync(task.Id);

            // Store the initial prediction result in the database
            await SubmitTaskPredictionAsync(task.Id, initialPrediction);

            logger.LogInformation("Started writer identification task {TaskId} using {ModelType} with {WriterCount} selected writers", 
                task.Id, 
                task.UseDefaultModel ? "default model" : $"custom model (ID: {task.ModelId})",
                task.SelectedWriters.Count);

            // Return the initial prediction result
            return initialPrediction;
        }
        catch (Exception ex)
        {
            // If execution fails, update task status to failed
            logger.LogError(ex, "Failed to start writer identification task execution for task {TaskId}", task.Id);
            
            task.Status = ProcessingStatus.Failed;
            task.UpdatedAt = DateTime.UtcNow;
            
            unitOfWork.Tasks.Update(task);
            await unitOfWork.SaveChangesAsync();
            
            throw; // Re-throw the exception to let the caller handle it if needed
        }
    }

    /// <summary>
    /// Gets task execution information including all container names for external processing.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <returns>The task execution information.</returns>
    public async Task<TaskExecutionInfoDto> GetTaskExecutionInfoAsync(Guid taskId)
    {
        var task = await GetRawTaskByIdAsync(taskId);
        
        // Get dataset information
        var dataset = await unitOfWork.Datasets.GetByIdAsync(task.DatasetId);
        if (dataset == null || !dataset.IsActive)
        {
            throw new KeyNotFoundException($"Dataset with ID {task.DatasetId} not found.");
        }

        // Get model information if using custom model
        string modelContainerName = null;
        if (!task.UseDefaultModel && task.ModelId.HasValue)
        {
            var model = await unitOfWork.Models.GetByIdAsync(task.ModelId.Value);
            if (model == null || !model.IsActive)
            {
                throw new KeyNotFoundException($"Model with ID {task.ModelId.Value} not found.");
            }
            modelContainerName = model.ContainerName;
        }

        var executionInfo = new TaskExecutionInfoDto
        {
            TaskId = task.Id,
            TaskContainerName = $"task-{task.Id}",
            DatasetContainerName = dataset.ContainerName,
            ModelContainerName = modelContainerName,
            UseDefaultModel = task.UseDefaultModel,
            SelectedWriters = task.SelectedWriters,
            QueryImageFileName = "query.png",
            Status = task.Status.ToString()
        };

        logger.LogInformation("Retrieved execution info for task {TaskId}: Dataset={DatasetContainer}, Model={ModelContainer}, UseDefault={UseDefault}",
            taskId, dataset.ContainerName, modelContainerName ?? "default", task.UseDefaultModel);

        return executionInfo;
    }

    /// <summary>
    /// Gets the prediction results for a completed task.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <returns>The prediction results if the task is completed, null otherwise.</returns>
    public async Task<TaskPredictionResultDto?> GetTaskPredictionResultsAsync(Guid taskId)
    {
        var task = await GetRawTaskByIdAsync(taskId);

        if (string.IsNullOrEmpty(task.ResultsJson) || task.Status != ProcessingStatus.Completed)
        {
            logger.LogWarning("Task {TaskId} does not have results or is not completed. Status: {Status}", taskId, task.Status);
            return null;
        }

        try
        {
            var predictionResult = JsonSerializer.Deserialize<TaskPredictionResultDto>(task.ResultsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            logger.LogInformation("Retrieved prediction results for task {TaskId}: Writer={WriterId}, Confidence={Confidence}", 
                taskId, predictionResult?.Prediction?.WriterId, predictionResult?.Prediction?.Confidence);

            return predictionResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to parse prediction results for task {TaskId}", taskId);
            return null;
        }
    }

    /// <summary>
    /// Submits prediction results for a task and marks it as completed.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="predictionResult">The prediction results.</param>
    public async Task SubmitTaskPredictionAsync(Guid taskId, TaskPredictionResultDto predictionResult)
    {
        var task = await GetRawTaskByIdAsync(taskId);

        // Ensure the task ID in the prediction result matches the task being updated
        predictionResult.TaskId = taskId;

        // Serialize the prediction result to JSON
        var resultsJson = JsonSerializer.Serialize(predictionResult, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        });

        // Update task with results and mark as completed
        task.ResultsJson = resultsJson;
        task.Status = ProcessingStatus.Completed;
        task.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Tasks.Update(task);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Submitted prediction results for task {TaskId}: Writer={WriterId}, Confidence={Confidence}", 
            taskId, predictionResult.Prediction?.WriterId, predictionResult.Prediction?.Confidence);
    }

    /// <summary>
    /// Gets the raw task by ID, ensuring it is active.
    /// </summary>
    /// <param name="id">The id</param>
    /// <returns>Returns the task details.</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    private async Task<WriterIdentificationTask> GetRawTaskByIdAsync(Guid id)
    {
        var task = await unitOfWork.Tasks.GetByIdAsync(id);
        if (task == null || !task.IsActive)
            throw new KeyNotFoundException($"Task with ID {id} not found.");
        return task;
    }
} 