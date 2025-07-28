using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Authorizer.DotNet;
using Authorizer.DotNet.Models.Requests;
using Authorizer.DotNet.Models.Responses;

namespace Authorizer.Sample.BlazorServer.Services;

/// <summary>
/// Service for handling authentication operations in Blazor Server applications.
/// </summary>
public class AuthService
{
    private readonly IAuthorizerClient _authorizerClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="authorizerClient">The authorizer client.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="logger">The logger.</param>
    public AuthService(
        IAuthorizerClient authorizerClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger)
    {
        _authorizerClient = authorizerClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="rememberMe">Whether to remember the user's login.</param>
    /// <returns>A tuple indicating success and an optional error message.</returns>
    public async Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password, bool rememberMe = false)
    {
        try
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password,
                IsRememberMe = rememberMe
            };

            var response = await _authorizerClient.LoginAsync(request);

            if (response.IsSuccess && response.Data != null)
            {
                await CreateAuthenticationCookieAsync(response.Data);
                return (true, null);
            }

            var errorMessage = response.FirstErrorMessage ?? "Login failed";
            return (false, errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user {Email}", email);
            return (false, "An error occurred during login");
        }
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The signup request containing user information.</param>
    /// <returns>A tuple indicating success and an optional error message.</returns>
    public async Task<(bool Success, string? ErrorMessage)> SignupAsync(SignupRequest request)
    {
        try
        {
            var response = await _authorizerClient.SignupAsync(request);

            if (response.IsSuccess && response.Data != null)
            {
                if (!string.IsNullOrEmpty(response.Data.AccessToken))
                {
                    await CreateAuthenticationCookieAsync(response.Data);
                }
                return (true, response.Data.Message);
            }

            var errorMessage = response.FirstErrorMessage ?? "Signup failed";
            return (false, errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Signup failed for user {Email}", request.Email);
            return (false, "An error occurred during signup");
        }
    }

    /// <summary>
    /// Logs out the current user and clears their authentication cookies.
    /// </summary>
    /// <returns>A task representing the asynchronous logout operation.</returns>
    public async Task LogoutAsync()
    {
        try
        {
            await _authorizerClient.LogoutAsync();
            
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout failed");
        }
    }

    /// <summary>
    /// Gets the current user's profile information.
    /// </summary>
    /// <returns>The user's profile or null if not authenticated or an error occurs.</returns>
    public async Task<UserProfile?> GetProfileAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var accessToken = httpContext.User.FindFirst("access_token")?.Value;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    var response = await _authorizerClient.GetProfileAsync(accessToken);
                    return response.IsSuccess ? response.Data : null;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get profile failed");
        }

        return null;
    }

    /// <summary>
    /// Determines whether the current user is authenticated.
    /// </summary>
    /// <returns>true if the user is authenticated; otherwise, false.</returns>
    public bool IsAuthenticated()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.User?.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Gets the current user's email address from the authentication context.
    /// </summary>
    /// <returns>The user's email address or null if not available.</returns>
    public string? GetCurrentUserEmail()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    private async Task CreateAuthenticationCookieAsync(LoginResponse loginData)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || loginData.User == null) return;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, loginData.User.Id),
            new(ClaimTypes.Email, loginData.User.Email ?? string.Empty),
            new(ClaimTypes.Name, loginData.User.Name ?? loginData.User.Email ?? string.Empty)
        };

        if (!string.IsNullOrEmpty(loginData.AccessToken))
        {
            claims.Add(new Claim("access_token", loginData.AccessToken));
        }

        if (!string.IsNullOrEmpty(loginData.RefreshToken))
        {
            claims.Add(new Claim("refresh_token", loginData.RefreshToken));
        }

        if (loginData.User.Roles != null)
        {
            claims.AddRange(loginData.User.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = loginData.ExpiresIn.HasValue 
                ? DateTimeOffset.UtcNow.AddSeconds(loginData.ExpiresIn.Value)
                : DateTimeOffset.UtcNow.AddHours(24)
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    private async Task CreateAuthenticationCookieAsync(SignupResponse signupData)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || signupData.User == null) return;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, signupData.User.Id),
            new(ClaimTypes.Email, signupData.User.Email ?? string.Empty),
            new(ClaimTypes.Name, signupData.User.Name ?? signupData.User.Email ?? string.Empty)
        };

        if (!string.IsNullOrEmpty(signupData.AccessToken))
        {
            claims.Add(new Claim("access_token", signupData.AccessToken));
        }

        if (!string.IsNullOrEmpty(signupData.RefreshToken))
        {
            claims.Add(new Claim("refresh_token", signupData.RefreshToken));
        }

        if (signupData.User.Roles != null)
        {
            claims.AddRange(signupData.User.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = signupData.ExpiresIn.HasValue 
                ? DateTimeOffset.UtcNow.AddSeconds(signupData.ExpiresIn.Value)
                : DateTimeOffset.UtcNow.AddHours(24)
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }
}