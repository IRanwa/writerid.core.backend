using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WriterID.Dev.Portal.Model.DTOs.External;
using WriterID.Dev.Portal.Service.Interfaces;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Controllers;

/// <summary>
/// External API controller for model operations by external services.
/// </summary>
[ApiController]
[Route("api/external/models")]
[Authorize(AuthenticationSchemes = "ApiKey")]
public class ModelExternalController : ControllerBase
{
    private readonly IModelService modelService;
    private readonly ILogger<ModelExternalController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelExternalController"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    /// <param name="logger">The logger.</param>
    public ModelExternalController(IModelService modelService, ILogger<ModelExternalController> logger)
    {
        this.modelService = modelService;
        this.logger = logger;
    }

    /// <summary>
    /// Updates the status of a model.
    /// </summary>
    /// <param name="request">The status update request.</param>
    /// <returns>A response indicating success or failure.</returns>
    [HttpPut("status")]
    public async Task<IActionResult> UpdateModelStatus([FromBody] Model.DTOs.External.UpdateModelStatusDto request)
    {
        try
        {
            logger.LogInformation("External service updating model {ModelId} status to {Status}", 
                request.ModelId, request.Status);

            // Convert external DTO to regular DTO for service call
            var statusDto = new Model.DTOs.Model.UpdateModelStatusDto
            {
                Status = request.Status
            };

            // Update the model status
            var updatedModel = await modelService.UpdateModelStatusAsync(request.ModelId, statusDto);

            logger.LogInformation("Successfully updated model {ModelId} status to {Status}", 
                request.ModelId, request.Status);

            return Ok(new 
            { 
                message = "Model status updated successfully",
                modelId = request.ModelId,
                status = request.Status.ToString(),
                updatedAt = DateTime.UtcNow
            });
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning("Model {ModelId} not found for status update: {Message}", request.ModelId, ex.Message);
            return NotFound(new { message = "Model not found" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating model {ModelId} status", request.ModelId);
            return StatusCode(500, new { message = "Internal server error occurred while updating model status" });
        }
    }

    /// <summary>
    /// Gets the status of a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>The model status information.</returns>
    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetModelStatus(Guid id)
    {
        try
        {
            var model = await modelService.GetModelByIdAsync(id);
            if (model == null)
            {
                logger.LogWarning("Model {ModelId} not found for status check", id);
                return NotFound(new { message = "Model not found" });
            }

            return Ok(new 
            { 
                modelId = model.Id,
                status = model.Status.ToString(),
                name = model.Name,
                trainingDatasetId = model.TrainingDatasetId,
                trainingDatasetName = model.TrainingDatasetName,
                createdAt = model.CreatedAt,
                updatedAt = model.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting model {ModelId} status", id);
            return StatusCode(500, new { message = "Internal server error occurred while getting model status" });
        }
    }
} 