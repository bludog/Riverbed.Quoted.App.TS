using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Riverbed.Pricing.Paint.Shared.Data;
using System.ComponentModel.DataAnnotations;

namespace Riverbed.Pricing.Paint.Controllers;

[ApiController]
[Route("api/[controller]")]
[IgnoreAntiforgeryToken]
public sealed class AuthenticationController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthenticationController(SignInManager<ApplicationUser> signInManager)
    {
        ArgumentNullException.ThrowIfNull(signInManager);
        _signInManager = signInManager;
    }

    /// <summary>
    /// Signs in a user with ASP.NET Core Identity and issues the application cookie.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new LoginResponse(false, false, false));
        }

        var signInResult = await _signInManager.PasswordSignInAsync(
            request.Email,
            request.Password,
            request.RememberMe,
            lockoutOnFailure: false);

        if (signInResult.Succeeded)
        {
            return Ok(new LoginResponse(true, false, false));
        }

        if (signInResult.RequiresTwoFactor)
        {
            return Unauthorized(new LoginResponse(false, true, false));
        }

        if (signInResult.IsLockedOut)
        {
            return Unauthorized(new LoginResponse(false, false, true));
        }

        return Unauthorized(new LoginResponse(false, false, false));
    }

    /// <summary>
    /// Signs out the current user and clears the ASP.NET Core Identity cookie.
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }
}

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public sealed record LoginResponse(
    bool Succeeded,
    bool RequiresTwoFactor,
    bool IsLockedOut);
