using AutoMapper;
using System.Text.Json;
using WriterID.Dev.Portal.Core.Enums;
using WriterID.Dev.Portal.Data.Interfaces;
using WriterID.Dev.Portal.Model.DTOs.Dataset;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The DatasetService class.
/// </summary>
public class DatasetService : IDatasetService
{
    /// <summary>
    /// The unit of work
    /// </summary>
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// The mapper
    /// </summary>
    private readonly IMapper mapper;

    /// <summary>
    /// The BLOB service
    /// </summary>
    private readonly IBlobService blobService;

    /// <summary>
    /// The queue service
    /// </summary>
    private readonly IQueueService queueService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="mapper">The mapper instance.</param>
    /// <param name="blobService">The blob service.</param>
    /// <param name="queueService">The queue service.</param>
    public DatasetService(IUnitOfWork unitOfWork, IMapper mapper, IBlobService blobService, IQueueService queueService)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
        this.blobService = blobService;
        this.queueService = queueService;
    }

    /// <summary>
    /// Creates a new dataset.
    /// </summary>
    /// <param name="dto">The dataset creation data.</param>
    /// <param name="userId">The ID of the user creating the dataset.</param>
    /// <returns>The created dataset.</returns>
    public async Task<Uri> CreateDatasetAsync(CreateDatasetRequestDto createDatasetDto, int userId)
    {
        var dataset = mapper.Map<Dataset>(createDatasetDto);
        dataset.UserId = userId;
        dataset.CreatedAt = DateTime.UtcNow;
        dataset.IsActive = true;
        dataset.Status = ProcessingStatus.Created;
        dataset.ContainerName = string.Empty;
        await unitOfWork.Datasets.AddAsync(dataset);
        await unitOfWork.SaveChangesAsync();
        dataset.ContainerName = $"dataset-{dataset.Id}";
        unitOfWork.Repository<Dataset>().Update(dataset);
        await unitOfWork.SaveChangesAsync();
        var sasUri = await blobService.CreateContainerAndGetSasUriAsync(dataset.ContainerName);
        return sasUri;
    }

    /// <summary>
    /// Retrieves all datasets for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A list of datasets for the user.</returns>
    public async Task<IEnumerable<DatasetDto>> GetAllDatasetsAsync(int userId)
    {
        var datasets = await unitOfWork.Datasets.FindAsync(d => d.UserId == userId);
        return mapper.Map<IEnumerable<DatasetDto>>(datasets);
    }

    /// <summary>
    /// Retrieves a dataset by its identifier.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The dataset if found.</returns>
    public async Task<DatasetDto> GetDatasetByIdAsync(int id)
    {
        var dataset = await unitOfWork.Datasets.GetByIdAsync(id);
        return mapper.Map<DatasetDto>(dataset);
    }

    /// <summary>
    /// Updates an existing dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <param name="dto">The update data.</param>
    public async Task UpdateDatasetAsync(int id, DatasetDto datasetDto)
    {
        var dataset = await unitOfWork.Datasets.GetByIdAsync(id);
        if (dataset == null)
        {
            return;
        }
        dataset.Status = datasetDto.Status;
        dataset.UpdatedAt = DateTime.UtcNow;
        unitOfWork.Repository<Dataset>().Update(dataset);
        await unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    public async Task DeleteDatasetAsync(int id)
    {
        var dataset = await unitOfWork.Datasets.GetByIdAsync(id);
        if (dataset != null)
        {
            dataset.IsActive = false;
            unitOfWork.Repository<Dataset>().Update(dataset);
            await unitOfWork.SaveChangesAsync();
            await blobService.DeleteContainerAsync(dataset.ContainerName);
        }
    }

    /// <summary>
    /// Analyzes a dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    public async Task AnalyzeDatasetAsync(int id)
    {
        var dataset = await unitOfWork.Datasets.GetByIdAsync(id);
        if (dataset == null)
            throw new KeyNotFoundException("Dataset not found.");
        var message = new
        {
            task = "analyze_dataset",
            @params = new { container_name = dataset.ContainerName }
        };
        var messageString = JsonSerializer.Serialize(message);
        await queueService.SendMessageAsync("writerid-task-queue", messageString);
    }

    /// <summary>
    /// Retrieves the analysis results for a dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The analysis results.</returns>
    public async Task<string> GetAnalysisResultsAsync(int id)
    {
        var dataset = await unitOfWork.Datasets.GetByIdAsync(id);
        if (dataset == null)
            throw new KeyNotFoundException("Dataset not found.");
        return await blobService.DownloadFileAsStringAsync(dataset.ContainerName, "analysis-results.json");
    }
}