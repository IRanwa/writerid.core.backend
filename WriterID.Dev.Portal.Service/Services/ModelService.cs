using Microsoft.Extensions.Logging;
using WriterID.Dev.Portal.Data.Interfaces;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Model;
using WriterID.Dev.Portal.Model.Queue;
using WriterID.Dev.Portal.Model.Enums;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The ModelService class.
/// </summary>
public class ModelService : IModelService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IAzureQueueService queueService;
    private readonly IAzureStorageService storageService;
    private readonly ILogger<ModelService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="queueService">The Azure queue service.</param>
    /// <param name="storageService">The Azure storage service.</param>
    /// <param name="logger">The logger instance.</param>
    public ModelService(
        IUnitOfWork unitOfWork,
        IAzureQueueService queueService,
        IAzureStorageService storageService,
        ILogger<ModelService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.queueService = queueService;
        this.storageService = storageService;
        this.logger = logger;
    }

    /// <summary>
    /// Creates a new writer identification model.
    /// </summary>
    /// <param name="dto">The model creation data.</param>
    /// <param name="userId">The ID of the user creating the model.</param>
    /// <returns>The created model.</returns>
    public async Task<WriterIdentificationModel> CreateModelAsync(CreateModelDto dto, int userId)
    {
        var containerName = $"model-{Guid.NewGuid()}";
        
        var model = new WriterIdentificationModel
        {
            Name = dto.Name,
            Description = dto.Description,
            ContainerName = containerName,
            Status = ModelStatus.Created,
            TrainingDatasetId = dto.TrainingDatasetId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await unitOfWork.Models.AddAsync(model);
        await unitOfWork.SaveChangesAsync();

        await storageService.CreateContainerAsync(containerName);

        return model;
    }

    /// <summary>
    /// Retrieves a model by its identifier.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>The model if found.</returns>
    public async Task<WriterIdentificationModel> GetModelByIdAsync(int id)
    {
        var model = await unitOfWork.Models.GetByIdAsync(id);

        if (model == null)
        {
            throw new KeyNotFoundException($"Model with ID {id} not found.");
        }

        return model;
    }

    /// <summary>
    /// Retrieves all models for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A list of models for the user.</returns>
    public async Task<List<WriterIdentificationModel>> GetUserModelsAsync(int userId)
    {
        var models = await unitOfWork.Models.FindAsync(m => m.UserId == userId);
        return models.OrderByDescending(m => m.CreatedAt).ToList();
    }

    /// <summary>
    /// Updates an existing model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated model.</returns>
    public async Task<WriterIdentificationModel> UpdateModelAsync(int id, UpdateModelDto dto)
    {
        var model = await GetModelByIdAsync(id);

        model.Name = dto.Name;
        model.Description = dto.Description;
        model.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Models.Update(model);
        await unitOfWork.SaveChangesAsync();

        return model;
    }

    /// <summary>
    /// Deletes a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    public async Task DeleteModelAsync(int id)
    {
        var model = await GetModelByIdAsync(id);

        await storageService.DeleteContainerAsync(model.ContainerName);

        unitOfWork.Models.Remove(model);
        await unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Starts the training of a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    public async Task StartTrainingAsync(int id)
    {
        var model = await GetModelByIdAsync(id);

        var message = new ModelTrainingMessage
        {
            ModelId = model.Id,
            Parameters = new ModelTrainingParameters
            {
                Epochs = 100,
                BatchSize = 32,
                LearningRate = 0.001
            }
        };

        await queueService.SendModelTrainingMessageAsync(message);

        model.Status = ModelStatus.Training;
        model.UpdatedAt = DateTime.UtcNow;
        
        unitOfWork.Models.Update(model);
        await unitOfWork.SaveChangesAsync();
    }
} 