using Microsoft.AspNetCore.Mvc;
using WriterID.Dev.Portal.Model.Entities;
using WriterID.Dev.Portal.Model.DTOs.Model;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Controllers;

/// <summary>
/// The ModelsController class.
/// </summary>
public class ModelsController : BaseApiController
{
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
    [ProducesResponseType(typeof(WriterIdentificationModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateModelDto dto)
    {
        var model = await modelService.CreateModelAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    /// <summary>
    /// Retrieves a model by its identifier.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>The model if found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WriterIdentificationModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await modelService.GetModelByIdAsync(id);
        return Ok(model);
    }

    /// <summary>
    /// Retrieves all models for the current user.
    /// </summary>
    /// <returns>A list of models.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<WriterIdentificationModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var models = await modelService.GetUserModelsAsync(CurrentUserId);
        return Ok(models);
    }

    /// <summary>
    /// Updates an existing model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated model.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(WriterIdentificationModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateModelDto dto)
    {
        var model = await modelService.UpdateModelAsync(id, dto);
        return Ok(model);
    }

    /// <summary>
    /// Deletes a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>A no content response if successful.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
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
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartTraining(int id)
    {
        await modelService.StartTrainingAsync(id);
        return Accepted();
    }
}