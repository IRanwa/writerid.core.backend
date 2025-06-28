using System.ComponentModel.DataAnnotations;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Model.DTOs.External;

/// <summary>
/// Data transfer object for updating task status from external services.
/// </summary>
public class UpdateTaskStatusDto
{
    /// <summary>
    /// Gets or sets the task identifier.
    /// </summary>
    [Required]
    public Guid TaskId { get; set; }

    /// <summary>
    /// Gets or sets the new status.
    /// </summary>
    [Required]
    public ProcessingStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the task results in JSON format (optional).
    /// Used when status is Completed to provide the processing results.
    /// </summary>
    public string ResultsJson { get; set; }

    /// <summary>
    /// Gets or sets optional message or details about the status update.
    /// </summary>
    public string Message { get; set; }
} 