namespace WriterID.Dev.Portal.Model.DTOs.Dataset;

/// <summary>
/// Data transfer object for dataset SAS URLs.
/// </summary>
public class DatasetSasUrlDto
{
    /// <summary>
    /// Gets or sets the upload URL.
    /// </summary>
    public string UploadUrl { get; set; }

    /// <summary>
    /// Gets or sets the download URL.
    /// </summary>
    public string DownloadUrl { get; set; }

    /// <summary>
    /// Gets or sets the expiration time of the URLs.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
} 