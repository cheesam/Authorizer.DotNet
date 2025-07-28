using Microsoft.AspNetCore.Mvc;
using Authorizer.DotNet;
using Authorizer.DotNet.Models.Requests;

namespace Authorizer.Sample.AspNetCore.Controllers;

/// <summary>
/// Authentication controller for handling login, signup, and related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthorizerClient _authorizerClient;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authorizerClient">The authorizer client.</param>
    /// <param name="logger">The logger.</param>
    public AuthController(IAuthorizerClient authorizerClient, ILogger<AuthController> logger)
    {
        _authorizerClient = authorizerClient;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    /// <param name="request">The login request containing user credentials.</param>
    /// <returns>Login response with user data and tokens.</returns>
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

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The signup request containing user information.</param>
    /// <returns>Signup response with user data.</returns>
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

    /// <summary>
    /// Initiates a password reset process by sending a reset email.
    /// </summary>
    /// <param name="request">The forgot password request containing the user's email.</param>
    /// <returns>Success message if email was sent.</returns>
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

    /// <summary>
    /// Resets a user's password using a reset token.
    /// </summary>
    /// <param name="request">The reset password request containing token and new password.</param>
    /// <returns>Success message if password was reset.</returns>
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

    /// <summary>
    /// Verifies a user's email address using a verification token.
    /// </summary>
    /// <param name="request">The verify email request containing the verification token.</param>
    /// <returns>Success message if email was verified.</returns>
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

    /// <summary>
    /// Gets metadata information about the Authorizer configuration.
    /// </summary>
    /// <returns>Meta information including available auth methods and configuration.</returns>
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

/// <summary>
/// Request model for forgot password operation.
/// </summary>
public class ForgotPasswordRequest
{
    /// <summary>
    /// Gets or sets the email address for password reset.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}