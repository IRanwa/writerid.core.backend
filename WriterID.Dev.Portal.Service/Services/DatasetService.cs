using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WriterID.Dev.Portal.Data;
using WriterID.Dev.Portal.Data.Interfaces;
using WriterID.Dev.Portal.Core.Enums;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Dataset;
using WriterID.Dev.Portal.Model.Queue;
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
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="storageService">The Azure storage service.</param>
    /// <param name="queueService">The Azure queue service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="mapper">The mapper instance.</param>
    public DatasetService(
        IUnitOfWork unitOfWork,
        IAzureStorageService storageService,
        IAzureQueueService queueService,
        ILogger<DatasetService> logger,
        IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.storageService = storageService;
        this.queueService = queueService;
        this.logger = logger;
        this.mapper = mapper;
    }

    /// <summary>
    /// Creates a new dataset.
    /// </summary>
    /// <param name="dto">The dataset creation data.</param>
    /// <param name="file">The uploaded file.</param>
    /// <param name="userId">The ID of the user creating the dataset.</param>
    /// <returns>The created dataset.</returns>
    public async Task<DatasetDto> CreateDatasetAsync(CreateDatasetDto dto, IFormFile file, int userId)
    {
        var dataset = mapper.Map<Dataset>(dto);
        dataset.ContainerName = $"dataset-{Guid.NewGuid()}";
        dataset.FileName = $"{Guid.NewGuid()}-{file.FileName}";
        dataset.FileSize = file.Length;
        dataset.Status = DatasetStatus.Uploading;
        dataset.ProcessingStatus = ProcessingStatus.Created;
        dataset.UserId = userId;

        await unitOfWork.Datasets.AddAsync(dataset);
        await unitOfWork.SaveChangesAsync();

        await storageService.CreateContainerAsync(dataset.ContainerName);
        await storageService.UploadFileAsync(dataset.ContainerName, dataset.FileName, file.OpenReadStream());

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

        return mapper.Map<DatasetDto>(dataset);
    }

    /// <summary>
    /// Retrieves a dataset by its identifier.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The dataset if found.</returns>
    public async Task<DatasetDto> GetDatasetByIdAsync(int id)
    {
        var dataset = await GetRawDatasetByIdAsync(id);
        return mapper.Map<DatasetDto>(dataset);
    }

    /// <summary>
    /// Retrieves all datasets for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A list of datasets for the user.</returns>
    public async Task<List<DatasetDto>> GetUserDatasetsAsync(int userId)
    {
        var datasets = await unitOfWork.Datasets.FindAsync(d => d.UserId == userId && d.IsActive);
        var orderedDatasets = datasets.OrderByDescending(d => d.CreatedAt).ToList();
        return mapper.Map<List<DatasetDto>>(orderedDatasets);
    }

    /// <summary>
    /// Updates an existing dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated dataset.</returns>
    public async Task<DatasetDto> UpdateDatasetAsync(int id, UpdateDatasetDto dto)
    {
        var dataset = await GetRawDatasetByIdAsync(id);

        mapper.Map(dto, dataset);
        dataset.UpdatedAt = DateTime.UtcNow;

        unitOfWork.Datasets.Update(dataset);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<DatasetDto>(dataset);
    }

    /// <summary>
    /// Deletes a dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    public async Task DeleteDatasetAsync(int id)
    {
        var dataset = await GetRawDatasetByIdAsync(id);

        await storageService.DeleteContainerAsync(dataset.ContainerName);

        dataset.IsActive = false;
        unitOfWork.Datasets.Update(dataset);
        await unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Downloads a dataset file.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The file stream and file name.</returns>
    public async Task<(Stream fileStream, string fileName)> DownloadDatasetAsync(int id)
    {
        var dataset = await GetRawDatasetByIdAsync(id);

        var fileStream = await storageService.DownloadFileAsync(dataset.ContainerName, dataset.FileName);
        return (fileStream, dataset.FileName);
    }

    private async Task<Dataset> GetRawDatasetByIdAsync(int id)
    {
        var dataset = await unitOfWork.Datasets.GetByIdAsync(id);
        if (dataset == null || !dataset.IsActive)
        {
            throw new KeyNotFoundException($"Dataset with ID {id} not found.");
        }
        return dataset;
    }
} 