using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WriterID.Dev.Portal.Data.Interfaces;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Dataset;
using WriterID.Dev.Portal.Model.Queue;
using WriterID.Dev.Portal.Model.Enums;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The DatasetService class.
/// </summary>
public class DatasetService : IDatasetService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IAzureStorageService storageService;
    private readonly IAzureQueueService queueService;
    private readonly ILogger<DatasetService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="storageService">The Azure storage service.</param>
    /// <param name="queueService">The Azure queue service.</param>
    /// <param name="logger">The logger instance.</param>
    public DatasetService(
        IUnitOfWork unitOfWork,
        IAzureStorageService storageService,
        IAzureQueueService queueService,
        ILogger<DatasetService> logger)
    {
        this.unitOfWork = unitOfWork;
        this.storageService = storageService;
        this.queueService = queueService;
        this.logger = logger;
    }

    /// <summary>
    /// Creates a new dataset.
    /// </summary>
    /// <param name="dto">The dataset creation data.</param>
    /// <param name="file">The uploaded file.</param>
    /// <param name="userId">The ID of the user creating the dataset.</param>
    /// <returns>The created dataset.</returns>
    public async Task<Dataset> CreateDatasetAsync(CreateDatasetDto dto, IFormFile file, int userId)
    {
        var containerName = $"dataset-{Guid.NewGuid()}";
        var fileName = $"{Guid.NewGuid()}-{file.FileName}";

        var dataset = new Dataset
        {
            Name = dto.Name,
            Description = dto.Description,
            ContainerName = containerName,
            FileName = fileName,
            FileSize = file.Length,
            Status = DatasetStatus.Uploading,
            ProcessingStatus = ProcessingStatus.Created,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await unitOfWork.Datasets.AddAsync(dataset);
        await unitOfWork.SaveChangesAsync();

        await storageService.CreateContainerAsync(containerName);
        await storageService.UploadFileAsync(containerName, fileName, file.OpenReadStream());

        dataset.Status = DatasetStatus.Uploaded;
        unitOfWork.Datasets.Update(dataset);
        await unitOfWork.SaveChangesAsync();

        var message = new DatasetAnalysisMessage
        {
            DatasetId = dataset.Id,
            Parameters = new DatasetAnalysisParameters
            {
                ConfidenceThreshold = 0.8,
                Preprocess = true,
                ExtractFeatures = true
            }
        };

        await queueService.SendDatasetAnalysisMessageAsync(message);

        return dataset;
    }

    /// <summary>
    /// Retrieves a dataset by its identifier.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The dataset if found.</returns>
    public async Task<Dataset> GetDatasetByIdAsync(int id)
    {
        var dataset = await unitOfWork.Datasets.GetByIdAsync(id);

        if (dataset == null)
        {
            throw new KeyNotFoundException($"Dataset with ID {id} not found.");
        }

        return dataset;
    }

    /// <summary>
    /// Retrieves all datasets for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A list of datasets for the user.</returns>
    public async Task<List<Dataset>> GetUserDatasetsAsync(int userId)
    {
        var datasets = await unitOfWork.Datasets.FindAsync(d => d.UserId == userId);
        return datasets.OrderByDescending(d => d.CreatedAt).ToList();
    }

    /// <summary>
    /// Updates an existing dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated dataset.</returns>
    public async Task<Dataset> UpdateDatasetAsync(int id, UpdateDatasetDto dto)
    {
        var dataset = await GetDatasetByIdAsync(id);

        dataset.Name = dto.Name;
        dataset.Description = dto.Description;
        dataset.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Datasets.Update(dataset);
        await unitOfWork.SaveChangesAsync();

        return dataset;
    }

    /// <summary>
    /// Deletes a dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    public async Task DeleteDatasetAsync(int id)
    {
        var dataset = await GetDatasetByIdAsync(id);

        await storageService.DeleteContainerAsync(dataset.ContainerName);

        unitOfWork.Datasets.Remove(dataset);
        await unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Downloads a dataset file.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The file stream and file name.</returns>
    public async Task<(Stream fileStream, string fileName)> DownloadDatasetAsync(int id)
    {
        var dataset = await GetDatasetByIdAsync(id);

        var fileStream = await storageService.DownloadFileAsync(dataset.ContainerName, dataset.FileName);
        return (fileStream, dataset.FileName);
    }
} 