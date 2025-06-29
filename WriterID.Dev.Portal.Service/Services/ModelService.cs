using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WriterID.Dev.Portal.Data.Interfaces;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Model;
using WriterID.Dev.Portal.Core.Enums;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The ModelService class.
/// </summary>
public class ModelService : IModelService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IQueueService queueService;
    private readonly IAzureStorageService storageService;
    private readonly ILogger<ModelService> logger;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="queueService">The queue service.</param>
    /// <param name="storageService">The Azure storage service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The mapper instance.</param>
    public ModelService(
        IUnitOfWork unitOfWork,
        IQueueService queueService,
        IAzureStorageService storageService,
        ILogger<ModelService> logger,
        IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.queueService = queueService;
        this.storageService = storageService;
        this.logger = logger;
        this.mapper = mapper;
    }

    /// <summary>
    /// Creates a new writer identification model.
    /// </summary>
    /// <param name="dto">The model creation data.</param>
    /// <param name="userId">The ID of the user creating the model.</param>
    /// <returns>The created model.</returns>
    public async Task<ModelDto> CreateModelAsync(CreateModelDto dto, int userId)
    {
        // Get the training dataset to access its container name
        var dataset = await unitOfWork.Datasets.GetByIdAsync(dto.TrainingDatasetId);
        if (dataset == null)
        {
            throw new KeyNotFoundException($"Training dataset with ID {dto.TrainingDatasetId} not found.");
        }

        var model = mapper.Map<WriterIdentificationModel>(dto);
        model.Status = ProcessingStatus.Created;
        model.UserId = userId;
        // Set container name using the model's primary key (GUID is generated when object is created)
        model.ContainerName = $"model-{model.Id}";

        await unitOfWork.Models.AddAsync(model);
        await unitOfWork.SaveChangesAsync();

        await storageService.CreateContainerAsync(model.ContainerName);

        // Send queue message for training
        var message = new
        {
            task = "train",
            dataset_container_name = dataset.ContainerName,
            model_container_name = model.ContainerName
        };

        var messageString = JsonSerializer.Serialize(message);
        await queueService.SendMessageAsync("writerid-task-queue", messageString);

        var modelDto = mapper.Map<ModelDto>(model);
        
        // Populate TrainingDatasetName
        modelDto.TrainingDatasetName = dataset.Name;

        return modelDto;
    }

    /// <summary>
    /// Retrieves a model by its identifier.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>The model if found.</returns>
    public async Task<ModelDto> GetModelByIdAsync(Guid id)
    {
        var model = await GetRawModelByIdAsync(id);
        var modelDto = mapper.Map<ModelDto>(model);
        
        // Populate TrainingDatasetName
        var dataset = await unitOfWork.Datasets.GetByIdAsync(model.TrainingDatasetId);
        modelDto.TrainingDatasetName = dataset?.Name ?? "Unknown Dataset";
        
        return modelDto;
    }

    /// <summary>
    /// Retrieves all models for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A list of models for the user.</returns>
    public async Task<List<ModelDto>> GetUserModelsAsync(int userId)
    {
        var models = await unitOfWork.Models.FindAsync(m => m.UserId == userId && m.IsActive);
        var orderedModels = models.OrderByDescending(m => m.CreatedAt).ToList();
        
        // Get unique dataset IDs from the models
        var datasetIds = orderedModels.Select(m => m.TrainingDatasetId).Distinct().ToList();
        
        // Fetch datasets by IDs to get their names
        var datasets = new List<Dataset>();
        foreach (var datasetId in datasetIds)
        {
            var dataset = await unitOfWork.Datasets.GetByIdAsync(datasetId);
            if (dataset != null)
            {
                datasets.Add(dataset);
            }
        }
        
        // Create a lookup dictionary for dataset names
        var datasetLookup = datasets.ToDictionary(d => d.Id, d => d.Name);
        
        // Map models to DTOs
        var modelDtos = mapper.Map<List<ModelDto>>(orderedModels);
        
        // Populate TrainingDatasetName for each model
        foreach (var modelDto in modelDtos)
        {
            modelDto.TrainingDatasetName = datasetLookup.GetValueOrDefault(modelDto.TrainingDatasetId, "Unknown Dataset");
        }
        
        return modelDtos;
    }



    /// <summary>
    /// Updates the status of an existing model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <param name="dto">The status update data.</param>
    /// <returns>The updated model.</returns>
    public async Task<ModelDto> UpdateModelStatusAsync(Guid id, UpdateModelStatusDto dto)
    {
        var model = await GetRawModelByIdAsync(id);

        model.Status = dto.Status;
        model.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Models.Update(model);
        await unitOfWork.SaveChangesAsync();

        var modelDto = mapper.Map<ModelDto>(model);
        
        // Populate TrainingDatasetName
        var dataset = await unitOfWork.Datasets.GetByIdAsync(model.TrainingDatasetId);
        modelDto.TrainingDatasetName = dataset?.Name ?? "Unknown Dataset";

        return modelDto;
    }

    /// <summary>
    /// Deletes a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    public async Task DeleteModelAsync(Guid id)
    {
        var model = await GetRawModelByIdAsync(id);

        await storageService.DeleteContainerAsync(model.ContainerName);

        model.IsActive = false;
        unitOfWork.Models.Update(model);
        await unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Starts the training of a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    public async Task StartTrainingAsync(Guid id)
    {
        var model = await GetRawModelByIdAsync(id);

        var message = new
        {
            task = "train_model",
            taskId = id.ToString(),
            container_name = model.ContainerName,
            parameters = new
            {
                epochs = 100,
                batch_size = 32,
                learning_rate = 0.001
            }
        };

        var messageString = JsonSerializer.Serialize(message);
        await queueService.SendMessageAsync("writerid-task-queue", messageString);

        model.Status = ProcessingStatus.Processing;
        model.UpdatedAt = DateTime.UtcNow;
        
        unitOfWork.Models.Update(model);
        await unitOfWork.SaveChangesAsync();
    }

    private async Task<WriterIdentificationModel> GetRawModelByIdAsync(Guid id)
    {
        var model = await unitOfWork.Models.GetByIdAsync(id);
        if (model == null || !model.IsActive)
        {
            throw new KeyNotFoundException($"Model with ID {id} not found.");
        }
        return model;
    }
} 