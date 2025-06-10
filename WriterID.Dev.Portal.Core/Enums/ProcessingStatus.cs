namespace WriterID.Dev.Portal.Core.Enums;

/// <summary>
/// Represents the current status of a processing operation in the system.
/// </summary>
public enum ProcessingStatus
{
    /// <summary>
    /// The operation has been created but not yet started.
    /// </summary>
    Created = 0,

    /// <summary>
    /// The operation is currently being processed.
    /// </summary>
    Processing = 1,

    /// <summary>
    /// The operation has been successfully completed.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// The operation has failed during processing.
    /// </summary>
    Failed = 3
} 