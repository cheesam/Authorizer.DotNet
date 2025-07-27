using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Authorizer.DotNet;
using Authorizer.DotNet.Models.Requests;
using Authorizer.DotNet.Models.Responses;

namespace Authorizer.Sample.BlazorServer.Services;

public class AuthService
{
    private readonly IAuthorizerClient _authorizerClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthorizerClient authorizerClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger)
    {
        _authorizerClient = authorizerClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

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

    public bool IsAuthenticated()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.User?.Identity?.IsAuthenticated == true;
    }

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