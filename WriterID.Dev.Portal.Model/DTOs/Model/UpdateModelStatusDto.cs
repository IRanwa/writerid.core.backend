using System.ComponentModel.DataAnnotations;
using WriterID.Dev.Portal.Core.Enums;

namespace WriterID.Dev.Portal.Model.DTOs.Model;

/// <summary>
/// Data transfer object for updating a model's status.
/// </summary>
public class UpdateModelStatusDto
{
    /// <summary>
    /// Gets or sets the new status for the model.
    /// </summary>
    [Required]
    public ProcessingStatus Status { get; set; }
} 