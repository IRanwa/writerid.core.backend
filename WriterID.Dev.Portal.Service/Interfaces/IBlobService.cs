namespace WriterID.Dev.Portal.Service.Interfaces;

/// <summary>
/// The blob service.
/// </summary>
public interface IBlobService
{
    /// <summary>
    /// Creates the container and get sas URI asynchronous.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    /// <returns>Returns container SAS URL.</returns>
    Task<Uri> CreateContainerAndGetSasUriAsync(string containerName);

    /// <summary>
    /// Downloads the file as string asynchronous.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>Returns download file content.</returns>
    Task<string> DownloadFileAsStringAsync(string containerName, string fileName);

    /// <summary>
    /// Deletes the container asynchronous.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    Task DeleteContainerAsync(string containerName);
}