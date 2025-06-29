using System.ComponentModel.DataAnnotations;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Model.DTOs.External;

/// <summary>
/// Data transfer object for updating model status from external services.
/// </summary>
public class UpdateModelStatusDto
{
    /// <summary>
    /// Gets or sets the model identifier.
    /// </summary>
    [Required]
    public Guid ModelId { get; set; }

    /// <summary>
    /// Gets or sets the new status.
    /// </summary>
    [Required]
    public ProcessingStatus Status { get; set; }

    /// <summary>
    /// Gets or sets optional message or details about the status update.
    /// </summary>
    public string Message { get; set; }
} 