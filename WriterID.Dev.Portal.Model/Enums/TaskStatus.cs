namespace WriterID.Dev.Portal.Model.Enums;

/// <summary>
/// Represents the status of a writer identification task.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// The task has been created but not yet started.
    /// </summary>
    Created = 0,

    /// <summary>
    /// The task is currently being processed.
    /// </summary>
    Processing = 1,

    /// <summary>
    /// The task has completed successfully.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// The task has failed.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// The task has been archived and is no longer active.
    /// </summary>
    Archived = 4
} 