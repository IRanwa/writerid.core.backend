using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WriterID.Dev.Portal.Model.DTOs.User;
using WriterID.Dev.Portal.Model.Entities;

namespace WriterID.Dev.Portal.Controllers;

/// <summary>
/// The AuthController class.
/// </summary>
[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    /// <summary>
    /// The user manager
    /// </summary>
    private readonly UserManager<User> userManager;

    /// <summary>
    /// The configuration
    /// </summary>
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="configuration">The configuration.</param>
    public AuthController(UserManager<User> userManager, IConfiguration configuration)
    {
        this.userManager = userManager;
        this.configuration = configuration;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="userForRegistration">The user registration data.</param>
    /// <returns>A 201 Created response if registration is successful.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserForRegistrationDto userForRegistration)
    {
        var user = new User
        {
            UserName = userForRegistration.Username,
            Email = userForRegistration.Email,
            FirstName = userForRegistration.FirstName,
            LastName = userForRegistration.LastName,
            CreatedAt = DateTime.UtcNow
        };
        var result = await userManager.CreateAsync(user, userForRegistration.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(201);
    }

    /// <summary>
    /// Logs in a user and returns a JWT token.
    /// </summary>
    /// <param name="userForLogin">The user login credentials.</param>
    /// <returns>A JWT token and expiration time if login is successful.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLogin)
    {
        var user = await userManager.FindByNameAsync(userForLogin.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, userForLogin.Password))
        {
            return Unauthorized();
        }

        var authClaims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? ""));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }
}