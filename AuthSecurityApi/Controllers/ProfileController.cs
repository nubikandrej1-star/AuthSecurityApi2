using AuthSecurityApi.Common.Extentions;
using AuthSecurityApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSecurityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserService _userService;

    public ProfileController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var profile = _userService.GetUserProfileAsync(userId.Value);
        return profile is { } ? Ok(profile) : NotFound();
    }

    [HttpGet("claim")]
    public IActionResult GetClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value, c.Issuer });

        return Ok(new { UserId = User.GetUserId(), Email = User.GetUserEmail(), Roles = User.GetUserRoles(), Claims = claims, IsVip = User.IsVip() });
    }
}