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

    /// <summary>
    /// Creates a task container for storing task-related files.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <returns>The container name that was created.</returns>
    Task<string> CreateTaskContainerAsync(Guid taskId);

    /// <summary>
    /// Uploads a base64 encoded image to a container as PNG file.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    /// <param name="fileName">Name of the file (e.g., "query.png").</param>
    /// <param name="base64Image">The base64 encoded image string.</param>
    /// <returns>The blob path of the uploaded image.</returns>
    Task<string> UploadBase64ImageAsync(string containerName, string fileName, string base64Image);
}