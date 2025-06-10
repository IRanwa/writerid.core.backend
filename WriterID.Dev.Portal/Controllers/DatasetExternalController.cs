using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WriterID.Dev.Portal.Model.DTOs.External;
using WriterID.Dev.Portal.Service.Interfaces;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Controllers;

/// <summary>
/// External API controller for dataset operations by external services.
/// </summary>
[ApiController]
[Route("api/external/datasets")]
[Authorize(AuthenticationSchemes = "ApiKey")]
public class DatasetExternalController : ControllerBase
{
    private readonly IDatasetService datasetService;
    private readonly ILogger<DatasetExternalController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetExternalController"/> class.
    /// </summary>
    /// <param name="datasetService">The dataset service.</param>
    /// <param name="logger">The logger.</param>
    public DatasetExternalController(IDatasetService datasetService, ILogger<DatasetExternalController> logger)
    {
        this.datasetService = datasetService;
        this.logger = logger;
    }

    /// <summary>
    /// Updates the status of a dataset.
    /// </summary>
    /// <param name="request">The status update request.</param>
    /// <returns>A response indicating success or failure.</returns>
    [HttpPut("status")]
    public async Task<IActionResult> UpdateDatasetStatus([FromBody] UpdateDatasetStatusDto request)
    {
        try
        {
            logger.LogInformation("External service updating dataset {DatasetId} status to {Status}", 
                request.DatasetId, request.Status);

            // Get the dataset first to verify it exists
            var dataset = await datasetService.GetDatasetByIdAsync(request.DatasetId);
            if (dataset == null)
            {
                logger.LogWarning("Dataset {DatasetId} not found for status update", request.DatasetId);
                return NotFound(new { message = "Dataset not found" });
            }

            // Update the dataset status
            dataset.Status = request.Status;
            await datasetService.UpdateDatasetAsync(request.DatasetId, dataset);

            logger.LogInformation("Successfully updated dataset {DatasetId} status to {Status}", 
                request.DatasetId, request.Status);

            return Ok(new 
            { 
                message = "Dataset status updated successfully",
                datasetId = request.DatasetId,
                status = request.Status.ToString(),
                updatedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating dataset {DatasetId} status", request.DatasetId);
            return StatusCode(500, new { message = "Internal server error occurred while updating dataset status" });
        }
    }

    /// <summary>
    /// Gets the status of a dataset.
    /// </summary>
    /// <param name="id">The dataset identifier.</param>
    /// <returns>The dataset status information.</returns>
    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetDatasetStatus(Guid id)
    {
        try
        {
            var dataset = await datasetService.GetDatasetByIdAsync(id);
            if (dataset == null)
            {
                logger.LogWarning("Dataset {DatasetId} not found for status check", id);
                return NotFound(new { message = "Dataset not found" });
            }

            return Ok(new 
            { 
                datasetId = dataset.Id,
                status = dataset.Status.ToString(),
                containerName = dataset.ContainerName,
                createdAt = dataset.CreatedAt,
                updatedAt = dataset.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting dataset {DatasetId} status", id);
            return StatusCode(500, new { message = "Internal server error occurred while getting dataset status" });
        }
    }
} 