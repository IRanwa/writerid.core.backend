using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The AzureStorageService class.
/// </summary>
public class AzureStorageService : IAzureStorageService
{
    private readonly BlobServiceClient blobServiceClient;
    private readonly ILogger<AzureStorageService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureStorageService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration containing the Azure Storage connection string</param>
    /// <param name="logger">The logger instance for recording operational data.</param>
    public AzureStorageService(IConfiguration configuration, ILogger<AzureStorageService> logger)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage");
        this.blobServiceClient = new BlobServiceClient(connectionString);
        this.logger = logger;
    }

    /// <summary>
    /// Creates a new container in Azure Blob Storage asynchronously.
    /// </summary>
    /// <param name="containerName">The name of the container to create.</param>
    public async Task CreateContainerAsync(string containerName)
    {
        try
        {
            await blobServiceClient.CreateBlobContainerAsync(containerName);
            logger.LogInformation("Created container {ContainerName}", containerName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating container {ContainerName}", containerName);
            throw;
        }
    }

    /// <summary>
    /// Uploads a file to Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <param name="fileStream">The file stream to upload.</param>
    public async Task UploadFileAsync(string containerName, string blobName, Stream fileStream)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            
            await blobClient.UploadAsync(fileStream, overwrite: true);
            logger.LogInformation("Uploaded file {ContainerName}/{BlobName}", containerName, blobName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading file {ContainerName}/{BlobName}", containerName, blobName);
            throw;
        }
    }

    /// <summary>
    /// Downloads a file from Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <returns>The file stream.</returns>
    public async Task<Stream> DownloadFileAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            
            var response = await blobClient.DownloadStreamingAsync();
            logger.LogInformation("Downloaded file {ContainerName}/{BlobName}", containerName, blobName);
            return response.Value.Content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error downloading file {ContainerName}/{BlobName}", containerName, blobName);
            throw;
        }
    }

    /// <summary>
    /// Generates a SAS URL for uploading files to a container.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <param name="expiryMinutes">The number of minutes until the SAS token expires.</param>
    /// <returns>The SAS URL for uploading files.</returns>
    public async Task<string> GetUploadSasUrlAsync(string containerName, string blobName, int expiryMinutes = 60)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            if (containerClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

                var sasUrl = containerClient.GetBlobClient(blobName).GenerateSasUri(sasBuilder).ToString();
                logger.LogInformation("Generated upload SAS URL for {ContainerName}/{BlobName}", containerName, blobName);
                return sasUrl;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating upload SAS URL for {ContainerName}/{BlobName}", containerName, blobName);
            throw;
        }
        
        return string.Empty;
    }

    /// <summary>
    /// Generates a SAS URL for downloading files from a container.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="blobName">The name of the blob.</param>
    /// <param name="expiryMinutes">The number of minutes until the SAS token expires.</param>
    /// <returns>The SAS URL for downloading files.</returns>
    public async Task<string> GetDownloadSasUrlAsync(string containerName, string blobName, int expiryMinutes = 60)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            if (containerClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasUrl = containerClient.GetBlobClient(blobName).GenerateSasUri(sasBuilder).ToString();
                logger.LogInformation("Generated download SAS URL for {ContainerName}/{BlobName}", containerName, blobName);
                return sasUrl;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating download SAS URL for {ContainerName}/{BlobName}", containerName, blobName);
            throw;
        }
        
        return string.Empty;
    }

    /// <summary>
    /// Lists all blobs in a container asynchronously.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <returns>A list of blob items in the container.</returns>
    public async Task<List<BlobItem>> ListBlobsAsync(string containerName)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = new List<BlobItem>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                blobs.Add(blobItem);
            }

            logger.LogInformation("Listed {Count} blobs in container {ContainerName}", blobs.Count, containerName);
            return blobs;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing blobs in container {ContainerName}", containerName);
            throw;
        }
    }

    /// <summary>
    /// Deletes a blob from a container asynchronously.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="blobName">The name of the blob to delete.</param>
    public async Task DeleteBlobAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.GetBlobClient(blobName).DeleteIfExistsAsync();
            
            logger.LogInformation("Deleted blob {ContainerName}/{BlobName}", containerName, blobName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting blob {ContainerName}/{BlobName}", containerName, blobName);
            throw;
        }
    }

    /// <summary>
    /// Deletes a container and all its contents asynchronously.
    /// </summary>
    /// <param name="containerName">The name of the container to delete.</param>
    public async Task DeleteContainerAsync(string containerName)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.DeleteIfExistsAsync();
            logger.LogInformation("Deleted container {ContainerName}", containerName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting container {ContainerName}", containerName);
            throw;
        }
    }
} 