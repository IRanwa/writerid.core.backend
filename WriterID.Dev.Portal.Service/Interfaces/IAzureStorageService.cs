using Azure.Storage.Blobs.Models;

namespace WriterID.Dev.Portal.Service.Interfaces;

/// <summary>
/// The IAzureStorageService interface.
/// </summary>
public interface IAzureStorageService
{
    /// <summary>
    /// Creates a new container in Azure Blob Storage asynchronously.
    /// </summary>
    /// <param name="containerName">The name of the container to create.</param>
    Task CreateContainerAsync(string containerName);

    /// <summary>
    /// Uploads a file to Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <param name="fileStream">The file stream to upload.</param>
    Task UploadFileAsync(string containerName, string blobName, Stream fileStream);

    /// <summary>
    /// Downloads a file from Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <returns>The file stream.</returns>
    Task<Stream> DownloadFileAsync(string containerName, string blobName);

    /// <summary>
    /// Generates a SAS URL for uploading files to a container.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <param name="expiryMinutes">The number of minutes until the SAS token expires.</param>
    /// <returns>The SAS URL for uploading files.</returns>
    Task<string> GetUploadSasUrlAsync(string containerName, string blobName, int expiryMinutes = 60);

    /// <summary>
    /// Generates a SAS URL for downloading files from a container.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <param name="expiryMinutes">The number of minutes until the SAS token expires.</param>
    /// <returns>The SAS URL for downloading files.</returns>
    Task<string> GetDownloadSasUrlAsync(string containerName, string blobName, int expiryMinutes = 60);

    /// <summary>
    /// Lists all blobs in a container asynchronously.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <returns>A list of blob items in the container.</returns>
    Task<List<BlobItem>> ListBlobsAsync(string containerName);

    /// <summary>
    /// Deletes a blob from a container asynchronously.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="blobName">The name of the blob to delete.</param>
    Task DeleteBlobAsync(string containerName, string blobName);

    /// <summary>
    /// Deletes a container and all its contents asynchronously.
    /// </summary>
    /// <param name="containerName">The name of the container to delete.</param>
    Task DeleteContainerAsync(string containerName);
} 