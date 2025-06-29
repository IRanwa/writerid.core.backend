using Microsoft.AspNetCore.Mvc;
using WriterID.Dev.Portal.Model.DTOs.Model;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Controllers;

/// <summary>
/// The ModelsController class.
/// </summary>
public class ModelsController : BaseApiController
{
    /// <summary>
    /// The model service
    /// </summary>
    private readonly IModelService modelService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelsController"/> class.
    /// </summary>
    /// <param name="modelService">The service for managing model operations.</param>
    public ModelsController(IModelService modelService)
    {
        this.modelService = modelService;
    }

    /// <summary>
    /// Creates a new model.
    /// </summary>
    /// <param name="dto">The model creation data.</param>
    /// <returns>A 201 Created response containing the created model.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateModelDto dto)
    {
        try
        {
            var modelDto = await modelService.CreateModelAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = modelDto.Id }, new { model = modelDto, message = "Model created successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the model.", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a model by its identifier.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>The model if found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var modelDto = await modelService.GetModelByIdAsync(id);
            return Ok(new { model = modelDto, message = "Model retrieved successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the model.", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all models for the current user.
    /// </summary>
    /// <returns>A list of models.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var modelDtos = await modelService.GetUserModelsAsync(CurrentUserId);
            return Ok(new { models = modelDtos, message = "Models retrieved successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving models.", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets the training results for a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>The training results if available.</returns>
    [HttpGet("{id}/training-results")]
    public async Task<IActionResult> GetTrainingResults(Guid id)
    {
        try
        {
            var trainingResults = await modelService.GetModelTrainingResultsAsync(id);
            return Ok(new { trainingResults = trainingResults, message = "Training results retrieved successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving training results.", error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>A no content response if successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await modelService.DeleteModelAsync(id);
            return Ok(new { message = "Model deleted successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the model.", error = ex.Message });
        }
    }

    /// <summary>
    /// Starts the training of a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>An accepted response if training started successfully.</returns>
    [HttpPost("{id}/train")]
    public async Task<IActionResult> StartTraining(Guid id)
    {
        try
        {
            await modelService.StartTrainingAsync(id);
            return Ok(new { message = "Model training has been queued." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while starting model training.", error = ex.Message });
        }
    }
}