namespace WriterID.Dev.Portal.Core.Enums;

/// <summary>
/// Represents the status of a writer identification model.
/// </summary>
public enum ModelStatus
{
    /// <summary>
    /// The model has been created but not yet trained.
    /// </summary>
    Created = 0,

    /// <summary>
    /// The model is currently being trained.
    /// </summary>
    Training = 1,

    /// <summary>
    /// The model training has completed successfully.
    /// </summary>
    Trained = 2,

    /// <summary>
    /// The model training has failed.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// The model has been archived and is no longer active.
    /// </summary>
    Archived = 4
} 