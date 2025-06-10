using Microsoft.AspNetCore.Mvc;
using WriterID.Dev.Portal.Model.DTOs.Dataset;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Controllers;

/// <summary>
/// The DatasetsController class.
/// </summary>
public class DatasetsController : BaseApiController
{
    /// <summary>
    /// The dataset service
    /// </summary>
    private readonly IDatasetService datasetService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetsController"/> class.
    /// </summary>
    /// <param name="datasetService">The service for managing dataset operations.</param>
    public DatasetsController(IDatasetService datasetService)
    {
        this.datasetService = datasetService;
    }

    /// <summary>
    /// Creates a new dataset with file upload.
    /// </summary>
    /// <param name="dto">The dataset creation data.</param>
    /// <param name="file">The dataset file to upload.</param>
    /// <returns>A 201 Created response containing the created dataset.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateDatasetDto dto, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required");
        }

        var datasetDto = await datasetService.CreateDatasetAsync(dto, file, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = datasetDto.Id }, datasetDto);
    }

    /// <summary>
    /// Retrieves a dataset by its identifier.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The dataset if found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var datasetDto = await datasetService.GetDatasetByIdAsync(id);
        return Ok(datasetDto);
    }

    /// <summary>
    /// Retrieves all datasets for the current user.
    /// </summary>
    /// <returns>A list of datasets.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var datasetDtos = await datasetService.GetUserDatasetsAsync(CurrentUserId);
        return Ok(datasetDtos);
    }

    /// <summary>
    /// Updates an existing dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated dataset.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDatasetDto dto)
    {
        var datasetDto = await datasetService.UpdateDatasetAsync(id, dto);
        return Ok(datasetDto);
    }

    /// <summary>
    /// Deletes a dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>A no content response if successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await datasetService.DeleteDatasetAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Downloads a dataset file.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The dataset file for download.</returns>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var (fileStream, fileName) = await datasetService.DownloadDatasetAsync(id);
        return File(fileStream, "application/octet-stream", fileName);
    }
}