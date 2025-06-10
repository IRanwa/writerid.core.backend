using System.ComponentModel.DataAnnotations;

namespace WriterID.Dev.Portal.Model.DTOs.User;

/// <summary>
/// Data transfer object for user registration.
/// </summary>
public class UserForRegistrationDto
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the password confirmation.
    /// </summary>
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
} 