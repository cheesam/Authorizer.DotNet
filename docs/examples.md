# Code Examples

This document provides practical code examples for common use cases with Authorizer.DotNet.

## Basic Setup

### ASP.NET Core Application

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add Authorizer services
builder.Services.AddAuthorizer(builder.Configuration, "Authorizer");

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.MapControllers();

app.Run();
```

### Console Application

```csharp
// Program.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Authorizer.DotNet;
using Authorizer.DotNet.Extensions;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());
services.AddAuthorizer(configuration, "Authorizer");

var serviceProvider = services.BuildServiceProvider();
var authorizerClient = serviceProvider.GetRequiredService<IAuthorizerClient>();

// Use the client
var metaResponse = await authorizerClient.GetMetaAsync();
if (metaResponse.IsSuccess)
{
    Console.WriteLine($"Authorizer Version: {metaResponse.Data?.Version}");
}
```

## Authentication Flows

### Email/Password Login

```csharp
public class AuthController : ControllerBase
{
    private readonly IAuthorizerClient _authorizerClient;

    public AuthController(IAuthorizerClient authorizerClient)
    {
        _authorizerClient = authorizerClient;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var loginRequest = new LoginRequest
        {
            Email = loginDto.Email,
            Password = loginDto.Password
        };

        var response = await _authorizerClient.LoginAsync(loginRequest);

        if (response.IsSuccess && response.Data != null)
        {
            // Store tokens securely (e.g., in HTTP-only cookies)
            Response.Cookies.Append("access_token", response.Data.AccessToken!,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddHours(1)
                });

            return Ok(new { 
                success = true, 
                user = response.Data.User 
            });
        }

        return BadRequest(new { 
            success = false, 
            errors = response.Errors?.Select(e => e.Message) 
        });
    }
}
```

### User Registration

```csharp
[HttpPost("signup")]
public async Task<IActionResult> Signup([FromBody] SignupDto signupDto)
{
    var signupRequest = new SignupRequest
    {
        Email = signupDto.Email,
        Password = signupDto.Password,
        ConfirmPassword = signupDto.ConfirmPassword,
        GivenName = signupDto.FirstName,
        FamilyName = signupDto.LastName
    };

    var response = await _authorizerClient.SignupAsync(signupRequest);

    if (response.IsSuccess)
    {
        return Ok(new { 
            success = true, 
            message = "Registration successful. Please check your email for verification." 
        });
    }

    return BadRequest(new { 
        success = false, 
        errors = response.Errors?.Select(e => e.Message) 
    });
}
```

### OAuth Authorization Flow

```csharp
[HttpGet("oauth/authorize")]
public async Task<IActionResult> Authorize([FromQuery] string redirectUri)
{
    var authorizeRequest = new AuthorizeRequest
    {
        ResponseType = "code",
        ClientId = "your-client-id",
        RedirectUri = redirectUri,
        Scope = "openid email profile",
        State = Guid.NewGuid().ToString() // Store this to validate later
    };

    var response = await _authorizerClient.AuthorizeAsync(authorizeRequest);

    if (response.IsSuccess)
    {
        // Redirect user to authorization URL
        return Redirect(response.Data?.AuthorizationUrl ?? "/error");
    }

    return BadRequest("Authorization failed");
}

[HttpPost("oauth/callback")]
public async Task<IActionResult> Callback([FromForm] string code, [FromForm] string state)
{
    // Validate state parameter
    // Exchange code for tokens
    var tokenRequest = new GetTokenRequest
    {
        GrantType = "authorization_code",
        Code = code,
        ClientId = "your-client-id",
        ClientSecret = "your-client-secret",
        RedirectUri = "your-redirect-uri"
    };

    var response = await _authorizerClient.GetTokenAsync(tokenRequest);

    if (response.IsSuccess && response.Data != null)
    {
        // Store tokens and redirect user
        return Redirect("/dashboard");
    }

    return BadRequest("Token exchange failed");
}
```

## User Management

### Get User Profile

```csharp
[HttpGet("profile")]
[Authorize] // Assuming you have authentication middleware
public async Task<IActionResult> GetProfile()
{
    var accessToken = Request.Cookies["access_token"] ?? 
                     Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

    if (string.IsNullOrEmpty(accessToken))
    {
        return Unauthorized("Access token required");
    }

    var response = await _authorizerClient.GetProfileAsync(accessToken);

    if (response.IsSuccess)
    {
        return Ok(response.Data);
    }

    return BadRequest(response.Errors?.Select(e => e.Message));
}
```

### Update User Profile

```csharp
// Note: Profile updates typically require custom implementation
// as Authorizer.dev handles this through their admin API or direct database access
[HttpPut("profile")]
public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profile)
{
    // This would typically be implemented as a custom endpoint
    // that updates the user data in your Authorizer instance
    // Implementation depends on your specific Authorizer setup
    
    return Ok(new { message = "Profile updated successfully" });
}
```

## Token Management

### JWT Validation Middleware

```csharp
public class AuthorizerAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAuthorizerClient _authorizerClient;

    public AuthorizerAuthenticationMiddleware(
        RequestDelegate next, 
        IAuthorizerClient authorizerClient)
    {
        _next = next;
        _authorizerClient = authorizerClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = ExtractTokenFromRequest(context);

        if (!string.IsNullOrEmpty(token))
        {
            var response = await _authorizerClient.ValidateJwtAsync(token);
            
            if (response.IsSuccess && response.Data != null)
            {
                // Add user information to context
                context.Items["User"] = response.Data;
            }
        }

        await _next(context);
    }

    private string? ExtractTokenFromRequest(HttpContext context)
    {
        // Try Authorization header first
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length);
        }

        // Try cookie
        return context.Request.Cookies["access_token"];
    }
}

// Register in Program.cs
app.UseMiddleware<AuthorizerAuthenticationMiddleware>();
```

### Token Refresh

```csharp
public async Task<string?> RefreshTokenAsync(string refreshToken)
{
    var tokenRequest = new GetTokenRequest
    {
        GrantType = "refresh_token",
        RefreshToken = refreshToken,
        ClientId = "your-client-id",
        ClientSecret = "your-client-secret"
    };

    var response = await _authorizerClient.GetTokenAsync(tokenRequest);

    if (response.IsSuccess && response.Data != null)
    {
        return response.Data.AccessToken;
    }

    return null;
}
```

## Password Management

### Forgot Password Flow

```csharp
[HttpPost("forgot-password")]
public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
{
    var response = await _authorizerClient.ForgotPasswordAsync(dto.Email);

    if (response.IsSuccess)
    {
        return Ok(new { 
            message = "Password reset email sent. Please check your inbox." 
        });
    }

    return BadRequest(new { 
        errors = response.Errors?.Select(e => e.Message) 
    });
}

[HttpPost("reset-password")]
public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
{
    var resetRequest = new ResetPasswordRequest
    {
        Token = dto.Token,
        Password = dto.Password,
        ConfirmPassword = dto.ConfirmPassword
    };

    var response = await _authorizerClient.ResetPasswordAsync(resetRequest);

    if (response.IsSuccess)
    {
        return Ok(new { message = "Password reset successful" });
    }

    return BadRequest(new { 
        errors = response.Errors?.Select(e => e.Message) 
    });
}
```

## Error Handling Patterns

### Global Error Handler

```csharp
public class AuthorizerErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthorizerErrorHandlingMiddleware> _logger;

    public AuthorizerErrorHandlingMiddleware(
        RequestDelegate next, 
        ILogger<AuthorizerErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AuthorizerException ex)
        {
            _logger.LogError(ex, "Authorizer API error");
            
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "Authentication failed",
                details = ex.Message
            }));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error communicating with Authorizer");
            
            context.Response.StatusCode = 503;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "Service temporarily unavailable",
                details = "Please try again later"
            }));
        }
    }
}
```

### Retry Pattern

```csharp
public class RetryableAuthorizerClient
{
    private readonly IAuthorizerClient _client;
    private readonly ILogger<RetryableAuthorizerClient> _logger;

    public RetryableAuthorizerClient(
        IAuthorizerClient client, 
        ILogger<RetryableAuthorizerClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<AuthorizerResponse<LoginResponse>> LoginWithRetryAsync(
        LoginRequest request, 
        int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var response = await _client.LoginAsync(request);
                
                if (response.IsSuccess)
                {
                    return response;
                }

                // Don't retry on authentication errors
                if (response.Errors?.Any(e => e.Message.Contains("invalid credentials")) == true)
                {
                    return response;
                }

                if (attempt == maxRetries)
                {
                    return response;
                }
                
                _logger.LogWarning("Login attempt {Attempt} failed, retrying...", attempt);
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Exponential backoff
            }
            catch (HttpRequestException) when (attempt < maxRetries)
            {
                _logger.LogWarning("Network error on attempt {Attempt}, retrying...", attempt);
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
            }
        }

        throw new AuthorizerException("Max retry attempts exceeded");
    }
}
```

## Testing

### Integration Test Example

```csharp
[Fact]
public async Task LoginFlow_ShouldReturnValidTokens()
{
    // Arrange
    var signupRequest = new SignupRequest
    {
        Email = $"test-{Guid.NewGuid()}@example.com",
        Password = "TestPassword123!",
        ConfirmPassword = "TestPassword123!"
    };

    // Act - Create user
    var signupResponse = await _client.SignupAsync(signupRequest);
    Assert.True(signupResponse.IsSuccess);

    // Act - Login
    var loginRequest = new LoginRequest
    {
        Email = signupRequest.Email,
        Password = signupRequest.Password
    };
    
    var loginResponse = await _client.LoginAsync(loginRequest);

    // Assert
    Assert.True(loginResponse.IsSuccess);
    Assert.NotNull(loginResponse.Data?.AccessToken);
    Assert.NotNull(loginResponse.Data?.User);

    // Cleanup
    if (loginResponse.Data?.AccessToken != null)
    {
        await _client.DeleteUserAsync(new DeleteUserRequest 
        { 
            Email = signupRequest.Email 
        });
    }
}
```

For more examples, check the sample projects in the `/samples` directory:
- [Console Sample](../samples/Authorizer.Sample.Console/)
- [ASP.NET Core Sample](../samples/Authorizer.Sample.AspNetCore/)
- [Blazor Server Sample](../samples/Authorizer.Sample.BlazorServer/)