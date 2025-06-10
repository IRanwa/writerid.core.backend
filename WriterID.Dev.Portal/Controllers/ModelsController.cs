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
        var modelDto = await modelService.CreateModelAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = modelDto.Id }, modelDto);
    }

    /// <summary>
    /// Retrieves a model by its identifier.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>The model if found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var modelDto = await modelService.GetModelByIdAsync(id);
        return Ok(modelDto);
    }

    /// <summary>
    /// Retrieves all models for the current user.
    /// </summary>
    /// <returns>A list of models.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var modelDtos = await modelService.GetUserModelsAsync(CurrentUserId);
        return Ok(modelDtos);
    }

    /// <summary>
    /// Updates an existing model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated model.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateModelDto dto)
    {
        var modelDto = await modelService.UpdateModelAsync(id, dto);
        return Ok(modelDto);
    }

    /// <summary>
    /// Deletes a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>A no content response if successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await modelService.DeleteModelAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Starts the training of a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>An accepted response if training started successfully.</returns>
    [HttpPost("{id}/train")]
    public async Task<IActionResult> StartTraining(Guid id)
    {
        await modelService.StartTrainingAsync(id);
        return Accepted();
    }
}