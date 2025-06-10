namespace WriterID.Dev.Portal.Model.DTOs.User;

/// <summary>
/// Data transfer object for login response.
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// Gets or sets the JWT token.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Gets or sets the token expiration time.
    /// </summary>
    public DateTime Expiration { get; set; }

    /// <summary>
    /// Gets or sets the user information.
    /// </summary>
    public UserInfoDto User { get; set; }
}

/// <summary>
/// Data transfer object for user information.
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
} 