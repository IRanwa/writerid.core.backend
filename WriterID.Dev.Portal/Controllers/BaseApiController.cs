using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WriterID.Dev.Portal.Controllers;

/// <summary>
/// The BaseApiController class.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Gets the ID of the currently authenticated user.
    /// </summary>
    /// <returns>The user ID from the claims principal.</returns>
    protected int CurrentUserId => int.Parse(User.Identity.Name ?? "0");
} 