using System.ComponentModel.DataAnnotations;

namespace WriterID.Dev.Portal.Model.DTOs.Task;

/// <summary>
/// Data transfer object for creating a new writer identification task.
/// </summary>
public class CreateTaskDto : IValidatableObject
{
    /// <summary>
    /// Gets or sets the name of the task.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    [StringLength(1000)]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the ID of the dataset to analyze.
    /// </summary>
    [Required]
    public Guid DatasetId { get; set; }

    /// <summary>
    /// Gets or sets the list of selected writer IDs for comparison.
    /// These should be selected from the dataset analysis results.
    /// </summary>
    [Required]
    public List<string> SelectedWriters { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets a value indicating whether to use the default model.
    /// Defaults to true for now.
    /// </summary>
    public bool UseDefaultModel { get; set; } = true;

    /// <summary>
    /// Gets or sets the ID of the custom model to use for writer identification.
    /// This is only used when UseDefaultModel is false.
    /// </summary>
    public Guid? ModelId { get; set; }

    /// <summary>
    /// Gets or sets the query image as base64 encoded string.
    /// This is the image to be compared against the selected writers.
    /// </summary>
    [Required]
    public string QueryImageBase64 { get; set; }

    /// <summary>
    /// Validates the task creation requirements.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!UseDefaultModel && (!ModelId.HasValue || ModelId == Guid.Empty))
        {
            yield return new ValidationResult(
                "Either UseDefaultModel must be true or a valid ModelId must be provided.",
                new[] { nameof(UseDefaultModel), nameof(ModelId) });
        }

        if (SelectedWriters == null || !SelectedWriters.Any())
        {
            yield return new ValidationResult(
                "At least one writer must be selected for comparison.",
                new[] { nameof(SelectedWriters) });
        }
    }
} 