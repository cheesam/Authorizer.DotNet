using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Authorizer.DotNet;
using Authorizer.DotNet.Models.Requests;

namespace Authorizer.Sample.AspNetCore.Controllers;

/// <summary>
/// Profile controller for managing user profile and session operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IAuthorizerClient _authorizerClient;
    private readonly ILogger<ProfileController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileController"/> class.
    /// </summary>
    /// <param name="authorizerClient">The authorizer client.</param>
    /// <param name="logger">The logger.</param>
    public ProfileController(IAuthorizerClient authorizerClient, ILogger<ProfileController> logger)
    {
        _authorizerClient = authorizerClient;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current user's profile information.
    /// </summary>
    /// <returns>User profile data.</returns>
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var accessToken = GetAccessTokenFromRequest();
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { message = "Access token not found" });
            }

            var response = await _authorizerClient.GetProfileAsync(accessToken);
            
            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(new { errors = response.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get profile failed");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Gets the current user's session information.
    /// </summary>
    /// <returns>Session data including tokens and expiration.</returns>
    [HttpGet("session")]
    public async Task<IActionResult> GetSession()
    {
        try
        {
            var response = await _authorizerClient.GetSessionAsync();
            
            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(new { errors = response.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get session failed");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Logs out the current user and invalidates their session.
    /// </summary>
    /// <returns>Success message if logout was successful.</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var response = await _authorizerClient.LogoutAsync();
            
            if (response.IsSuccess)
            {
                return Ok(new { message = "Logged out successfully" });
            }

            return BadRequest(new { errors = response.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout failed");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Changes the current user's password.
    /// </summary>
    /// <param name="request">The change password request containing old and new passwords.</param>
    /// <returns>Success message if password was changed.</returns>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var accessToken = GetAccessTokenFromRequest();
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { message = "Access token not found" });
            }

            request.Token = accessToken;
            var response = await _authorizerClient.ChangePasswordAsync(request);
            
            if (response.IsSuccess)
            {
                return Ok(new { message = "Password changed successfully" });
            }

            return BadRequest(new { errors = response.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Change password failed");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Validates the current user's JWT token.
    /// </summary>
    /// <returns>Validation result with user profile if valid.</returns>
    [HttpPost("validate-jwt")]
    public async Task<IActionResult> ValidateJwt()
    {
        try
        {
            var accessToken = GetAccessTokenFromRequest();
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { message = "Access token not found" });
            }

            var response = await _authorizerClient.ValidateJwtAsync(accessToken);
            
            if (response.IsSuccess)
            {
                return Ok(new { valid = true, profile = response.Data });
            }

            return Ok(new { valid = false, errors = response.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "JWT validation failed");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    private string? GetAccessTokenFromRequest()
    {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            return authHeader["Bearer ".Length..];
        }

        return HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}