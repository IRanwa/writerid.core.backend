using Microsoft.AspNetCore.Mvc;
using WriterID.Dev.Portal.Model.DTOs.Dataset;
using WriterID.Dev.Portal.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
    /// <param name="createDto">The dataset creation data.</param>
    /// <returns>A 200 OK response containing the generated SAS URL.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDatasetRequestDto createDto)
    {
        var sasUri = await datasetService.CreateDatasetAsync(createDto, CurrentUserId);
        return Ok(new { sasUrl = sasUri.ToString() });
    }

    /// <summary>
    /// Retrieves a dataset by its identifier.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The dataset if found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var dataset = await datasetService.GetDatasetByIdAsync(id);
        return Ok(dataset);
    }

    /// <summary>
    /// Retrieves all datasets for the current user.
    /// </summary>
    /// <returns>A list of datasets.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var datasets = await datasetService.GetAllDatasetsAsync(CurrentUserId);
        return Ok(datasets);
    }

    /// <summary>
    /// Updates an existing dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <param name="datasetDto">The update data.</param>
    /// <returns>A no content response if successful.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DatasetDto datasetDto)
    {
        await datasetService.UpdateDatasetAsync(id, datasetDto);
        return NoContent();
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
    /// Analyzes the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Returns dataset analysis response.</returns>
    [HttpPost("{id}/analyze")]
    public async Task<IActionResult> Analyze(int id)
    {
        try
        {
            await datasetService.AnalyzeDatasetAsync(id);
            return Ok(new { message = "Dataset analysis has been queued." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Gets the analysis results.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Returns analysis result response.</returns>
    [HttpGet("{id}/analysis-results")]
    public async Task<IActionResult> GetAnalysisResults(int id)
    {
        try
        {
            var results = await datasetService.GetAnalysisResultsAsync(id);
            if (results == null)
            {
                return NotFound("Analysis results not found or not yet available.");
            }
            return Content(results, "application/json");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    
}