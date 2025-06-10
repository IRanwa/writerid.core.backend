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
}