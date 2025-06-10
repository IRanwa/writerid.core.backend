using AutoMapper;
using Microsoft.Extensions.Logging;
using WriterID.Dev.Portal.Data.Interfaces;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Model;
using WriterID.Dev.Portal.Model.Queue;
using WriterID.Dev.Portal.Core.Enums;
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
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="queueService">The Azure queue service.</param>
    /// <param name="storageService">The Azure storage service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The mapper instance.</param>
    public ModelService(
        IUnitOfWork unitOfWork,
        IAzureQueueService queueService,
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
        var model = mapper.Map<WriterIdentificationModel>(dto);
        model.ContainerName = $"model-{Guid.NewGuid()}";
        model.Status = ProcessingStatus.Created;
        model.UserId = userId;

        await unitOfWork.Models.AddAsync(model);
        await unitOfWork.SaveChangesAsync();

        await storageService.CreateContainerAsync(model.ContainerName);

        return mapper.Map<ModelDto>(model);
    }

    /// <summary>
    /// Retrieves a model by its identifier.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>The model if found.</returns>
    public async Task<ModelDto> GetModelByIdAsync(int id)
    {
        var model = await GetRawModelByIdAsync(id);
        return mapper.Map<ModelDto>(model);
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
        return mapper.Map<List<ModelDto>>(orderedModels);
    }

    /// <summary>
    /// Updates an existing model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated model.</returns>
    public async Task<ModelDto> UpdateModelAsync(int id, UpdateModelDto dto)
    {
        var model = await GetRawModelByIdAsync(id);

        mapper.Map(dto, model);
        model.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Models.Update(model);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<ModelDto>(model);
    }

    /// <summary>
    /// Deletes a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    public async Task DeleteModelAsync(int id)
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
    public async Task StartTrainingAsync(int id)
    {
        var model = await GetRawModelByIdAsync(id);

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

        model.Status = ProcessingStatus.Processing;
        model.UpdatedAt = DateTime.UtcNow;
        
        unitOfWork.Models.Update(model);
        await unitOfWork.SaveChangesAsync();
    }

    private async Task<WriterIdentificationModel> GetRawModelByIdAsync(int id)
    {
        var model = await unitOfWork.Models.GetByIdAsync(id);
        if (model == null || !model.IsActive)
        {
            throw new KeyNotFoundException($"Model with ID {id} not found.");
        }
        return model;
    }
} 