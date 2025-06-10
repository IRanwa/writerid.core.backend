using Microsoft.AspNetCore.Http;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Dataset;

namespace WriterID.Dev.Portal.Service.Interfaces;

/// <summary>
/// The IDatasetService interface.
/// </summary>
public interface IDatasetService
{
    /// <summary>
    /// Retrieves a dataset by its identifier.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The dataset if found.</returns>
    Task<DatasetDto> GetDatasetByIdAsync(int id);

    /// <summary>
    /// Retrieves all datasets for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A list of datasets for the user.</returns>
    Task<IEnumerable<DatasetDto>> GetAllDatasetsAsync(int userId);

    /// <summary>
    /// Deletes a dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    Task DeleteDatasetAsync(int id);

    /// <summary>
    /// Creates a new dataset.
    /// </summary>
    /// <param name="createDatasetDto">The dataset creation data.</param>
    /// <param name="userId">The ID of the user creating the dataset.</param>
    /// <returns>The URI for the SAS token.</returns>
    Task<Uri> CreateDatasetAsync(CreateDatasetRequestDto createDatasetDto, int userId);

    /// <summary>
    /// Updates an existing dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <param name="datasetDto">The updated dataset.</param>
    Task UpdateDatasetAsync(int id, DatasetDto datasetDto);

    /// <summary>
    /// Analyzes a dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    Task AnalyzeDatasetAsync(int id);

    /// <summary>
    /// Retrieves the analysis results for a dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The analysis results.</returns>
    Task<string> GetAnalysisResultsAsync(int id);
} 