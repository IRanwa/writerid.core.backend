namespace WriterID.Dev.Portal.Model.Enums;

/// <summary>
/// Represents a dataset status in the writer identification system.
/// </summary>
public enum DatasetStatus
{
    /// <summary>
    /// The dataset has been created but not yet uploaded.
    /// </summary>
    Created,

    /// <summary>
    /// The dataset files are being uploaded.
    /// </summary>
    Uploading,

    /// <summary>
    /// The dataset files have been uploaded successfully.
    /// </summary>
    Uploaded,

    /// <summary>
    /// The dataset is being processed.
    /// </summary>
    Processing,

    /// <summary>
    /// The dataset has been processed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The dataset processing has failed.
    /// </summary>
    Failed
} 