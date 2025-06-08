using System.ComponentModel.DataAnnotations;

namespace WriterID.Dev.Portal.Model.DTOs.User;

/// <summary>
/// Data transfer object for user login.
/// </summary>
public class UserForLoginDto
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [Required]
    public string Password { get; set; }
} 