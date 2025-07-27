using Microsoft.AspNetCore.Mvc;
using Authorizer.DotNet;
using Authorizer.DotNet.Models.Requests;

namespace Authorizer.Sample.AspNetCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthorizerClient _authorizerClient;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthorizerClient authorizerClient, ILogger<AuthController> logger)
    {
        _authorizerClient = authorizerClient;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authorizerClient.LoginAsync(request);
            
            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(new { errors = response.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user {Email}", request.Email);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest request)
    {
        try
        {
            var response = await _authorizerClient.SignupAsync(request);
            
            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(new { errors = response.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Signup failed for user {Email}", request.Email);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            var response = await _authorizerClient.ForgotPasswordAsync(request.Email);
            
            if (response.IsSuccess)
            {
                return Ok(new { message = "Password reset email sent successfully" });
            }

            return BadRequest(new { errors = response.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Forgot password failed for email {Email}", request.Email);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var response = await _authorizerClient.ResetPasswordAsync(request);
            
            if (response.IsSuccess)
            {
                return Ok(new { message = "Password reset successfully" });
            }

            return BadRequest(new { errors = response.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset failed");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        try
        {
            var response = await _authorizerClient.VerifyEmailAsync(request);
            
            if (response.IsSuccess)
            {
                return Ok(new { message = "Email verified successfully" });
            }

            return BadRequest(new { errors = response.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email verification failed for {Email}", request.Email);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("meta")]
    public async Task<IActionResult> GetMeta()
    {
        try
        {
            var response = await _authorizerClient.GetMetaAsync();
            
            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(new { errors = response.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get meta failed");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}