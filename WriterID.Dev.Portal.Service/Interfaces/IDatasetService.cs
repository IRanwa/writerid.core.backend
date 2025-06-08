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
    /// Creates a new dataset.
    /// </summary>
    /// <param name="dto">The dataset creation data.</param>
    /// <param name="file">The uploaded file.</param>
    /// <param name="userId">The ID of the user creating the dataset.</param>
    /// <returns>The created dataset.</returns>
    Task<Dataset> CreateDatasetAsync(CreateDatasetDto dto, IFormFile file, int userId);

    /// <summary>
    /// Retrieves a dataset by its identifier.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The dataset if found.</returns>
    Task<Dataset> GetDatasetByIdAsync(int id);

    /// <summary>
    /// Retrieves all datasets for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A list of datasets for the user.</returns>
    Task<List<Dataset>> GetUserDatasetsAsync(int userId);

    /// <summary>
    /// Updates an existing dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated dataset.</returns>
    Task<Dataset> UpdateDatasetAsync(int id, UpdateDatasetDto dto);

    /// <summary>
    /// Deletes a dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    Task DeleteDatasetAsync(int id);

    /// <summary>
    /// Downloads a dataset file.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The file stream and file name.</returns>
    Task<(Stream fileStream, string fileName)> DownloadDatasetAsync(int id);
} 