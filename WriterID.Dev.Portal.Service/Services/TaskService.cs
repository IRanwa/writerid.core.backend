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
                Writers = new List<WriterInfo>()
            };
        }

        // Parse the JSON to extract writer names
        var jsonDocument = JsonDocument.Parse(analysisResultsJson);
        var root = jsonDocument.RootElement;
        
        var writers = new List<WriterInfo>();
        
        // Extract writer_names array
        if (root.TryGetProperty("writer_names", out var writerNamesArray) && writerNamesArray.ValueKind == JsonValueKind.Array)
        {
            foreach (var writerNameElement in writerNamesArray.EnumerateArray())
            {
                var writerName = writerNameElement.GetString();
                if (!string.IsNullOrEmpty(writerName))
                {
                    writers.Add(new WriterInfo
                    {
                        WriterId = writerName,
                        WriterName = writerName
                    });
                }
            }
        }
        
        logger.LogInformation("Retrieved {WriterCount} writers for dataset {DatasetId}", writers.Count, datasetId);
        
        return new DatasetAnalysisResultDto
        {
            DatasetId = dataset.Id,
            DatasetName = dataset.Name,
            Status = dataset.Status.ToString(),
            AnalyzedAt = dataset.UpdatedAt,
            Writers = writers
        };
    }

    /// <summary>
    /// Creates a new writer identification task with selected writers and query image, then executes the prediction.
    /// </summary>
    /// <param name="dto">The task creation data.</param>
    /// <param name="userId">The ID of the user creating the task.</param>
    /// <returns>True if task created and execution started successfully, false otherwise.</returns>
    public async Task<bool> CreateTaskAsync(CreateTaskDto dto, int userId)
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
            await taskExecutorService.StartTaskExecutionAsync(task.Id);

            logger.LogInformation("Started prediction execution for task {TaskId} using {ModelType}", 
                task.Id, 
                task.UseDefaultModel ? "default model" : $"custom model (ID: {task.ModelId})");

            // Return success
            return true;
        }
        catch (Exception ex)
        {
            // If execution fails, update task status to failed
            logger.LogError(ex, "Failed to start prediction execution for task {TaskId}", task.Id);
            
            task.Status = ProcessingStatus.Failed;
            task.UpdatedAt = DateTime.UtcNow;
            
            unitOfWork.Tasks.Update(task);
            await unitOfWork.SaveChangesAsync();
            
            // Return failure
            return false;
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
    public async Task StartTaskAsync(Guid id)
    {
        var task = await GetRawTaskByIdAsync(id);
        
        // Update task status to processing before calling executor
        task.Status = ProcessingStatus.Processing;
        task.UpdatedAt = DateTime.UtcNow;
        
        unitOfWork.Tasks.Update(task);
        await unitOfWork.SaveChangesAsync();

        // Call the task executor service
        await taskExecutorService.StartTaskExecutionAsync(task.Id);

        logger.LogInformation("Started writer identification task {TaskId} using {ModelType} with {WriterCount} selected writers", 
            task.Id, 
            task.UseDefaultModel ? "default model" : $"custom model (ID: {task.ModelId})",
            task.SelectedWriters.Count);
    }
    
    private async Task<WriterIdentificationTask> GetRawTaskByIdAsync(Guid id)
    {
        var task = await unitOfWork.Tasks.GetByIdAsync(id);
        if (task == null || !task.IsActive)
        {
            throw new KeyNotFoundException($"Task with ID {id} not found.");
        }
        return task;
    }
} 