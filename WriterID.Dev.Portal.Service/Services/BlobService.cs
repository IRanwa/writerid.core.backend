using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using WriterID.Dev.Portal.Service.Interfaces;

namespace WriterID.Dev.Portal.Service.Services;

/// <summary>
/// The blob service.
/// </summary>
/// <seealso cref="IBlobService" />
public class BlobService : IBlobService
{
    /// <summary>
    /// The BLOB service client
    /// </summary>
    private readonly BlobServiceClient blobServiceClient;

    /// <summary>
    /// The configuration
    /// </summary>
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public BlobService(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureStorage"));
    }

    /// <summary>
    /// Creates the container and get sas URI asynchronous.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    /// <returns>Returns SAS URI.</returns>
    public async Task<Uri> CreateContainerAndGetSasUriAsync(string containerName)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        if (!int.TryParse(configuration["SasUrlExpiryDays"], out var expiryDays))
        {
            expiryDays = 3;
        }
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            Resource = "c",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(expiryDays),
        };
        sasBuilder.SetPermissions(BlobContainerSasPermissions.Write | BlobContainerSasPermissions.Read | BlobContainerSasPermissions.List);
        return containerClient.GenerateSasUri(sasBuilder);
    }

    /// <summary>
    /// Downloads the file as string asynchronous.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Returns blob file content.</returns>
    public async Task<string> DownloadFileAsStringAsync(string containerName, string fileName)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        if (!await blobClient.ExistsAsync())
            return null;
        var response = await blobClient.DownloadContentAsync();
        return response.Value.Content.ToString();
    }

    /// <summary>
    /// Deletes the container asynchronous.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    public async Task DeleteContainerAsync(string containerName)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.DeleteIfExistsAsync();
    }

    /// <summary>
    /// Creates a task container for storing task-related files.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <returns>The container name that was created.</returns>
    public async Task<string> CreateTaskContainerAsync(Guid taskId)
    {
        var containerName = $"task-{taskId}";
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        return containerName;
    }

    /// <summary>
    /// Uploads a base64 encoded image to a container as PNG file.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    /// <param name="fileName">Name of the file (e.g., "query.png").</param>
    /// <param name="base64Image">The base64 encoded image string.</param>
    /// <returns>The blob path of the uploaded image.</returns>
    public async Task<string> UploadBase64ImageAsync(string containerName, string fileName, string base64Image)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        // Remove data URL prefix if present (e.g., "data:image/png;base64,")
        var base64Data = base64Image;
        if (base64Image.Contains(","))
        {
            base64Data = base64Image.Split(',')[1];
        }

        // Convert base64 to byte array
        var imageBytes = Convert.FromBase64String(base64Data);

        // Upload the image
        using var stream = new MemoryStream(imageBytes);
        await blobClient.UploadAsync(stream, overwrite: true);

        return $"{containerName}/{fileName}";
    }

    /// <summary>
    /// Downloads an image file from blob storage as base64 string.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>The image as base64 string, or null if not found.</returns>
    public async Task<string?> DownloadImageAsBase64Async(string containerName, string fileName)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            
            if (!await blobClient.ExistsAsync())
                return null;

            var response = await blobClient.DownloadContentAsync();
            var imageBytes = response.Value.Content.ToArray();
            
            return Convert.ToBase64String(imageBytes);
        }
        catch
        {
            return null;
        }
    }
}