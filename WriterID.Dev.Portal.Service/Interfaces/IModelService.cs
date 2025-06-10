using WriterID.Dev.Portal.Model.DTOs.Model;

namespace WriterID.Dev.Portal.Service.Interfaces;

/// <summary>
/// The IModelService interface.
/// </summary>
public interface IModelService
{
    /// <summary>
    /// Creates a new writer identification model.
    /// </summary>
    /// <param name="dto">The model creation data.</param>
    /// <param name="userId">The ID of the user creating the model.</param>
    /// <returns>The created model.</returns>
    Task<ModelDto> CreateModelAsync(CreateModelDto dto, int userId);

    /// <summary>
    /// Retrieves a model by its identifier.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <returns>The model if found.</returns>
    Task<ModelDto> GetModelByIdAsync(int id);

    /// <summary>
    /// Retrieves all models for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A list of models for the user.</returns>
    Task<List<ModelDto>> GetUserModelsAsync(int userId);

    /// <summary>
    /// Updates an existing model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    /// <param name="dto">The update data.</param>
    /// <returns>The updated model.</returns>
    Task<ModelDto> UpdateModelAsync(int id, UpdateModelDto dto);

    /// <summary>
    /// Deletes a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    Task DeleteModelAsync(int id);

    /// <summary>
    /// Starts the training of a model.
    /// </summary>
    /// <param name="id">The model identifier.</param>
    Task StartTrainingAsync(int id);
} 