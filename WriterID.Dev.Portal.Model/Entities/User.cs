using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WriterID.Dev.Portal.Model.Entities;

/// <summary>
/// Represents a user in the writer identification system.
/// </summary>
public class User : IdentityUser<int>
{
    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }
    
    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets a value indicating whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
} 