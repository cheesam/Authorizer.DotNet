# Cross-Domain Authentication Configuration

This document explains how to configure the Authorizer .NET SDK for cross-domain authentication scenarios where your Authorizer instance and client application are on different subdomains.

## Problem Statement

In cross-domain scenarios (e.g., `auth.example.com` and `app.example.com`), cookies set by the Authorizer instance may not be accessible to the client application, causing session validation to fail with HTTP 422 errors.

## Solution

The .NET SDK now provides several configuration options to handle cross-domain authentication:

### 1. Basic Cross-Domain Cookie Configuration

```csharp
services.AddAuthorizer(options =>
{
    options.AuthorizerUrl = "https://auth.example.com";
    options.RedirectUrl = "https://app.example.com/auth/callback";
    options.ClientId = "your-client-id";
    
    // Enable cross-domain authentication
    options.UseCookies = true;
    options.UseCredentials = true;
    options.SetOriginHeader = true;
    options.CookieDomain = ".example.com"; // Note the leading dot for subdomain support
});
```

### 2. Token-Based Fallback (Recommended)

Enable automatic token-based fallback when cookie authentication fails:

```csharp
services.AddAuthorizer(options =>
{
    options.AuthorizerUrl = "https://auth.example.com";
    options.RedirectUrl = "https://app.example.com/auth/callback";
    options.ClientId = "your-client-id";
    
    // Cross-domain configuration
    options.UseCookies = true;
    options.UseCredentials = true;
    options.SetOriginHeader = true;
    options.CookieDomain = ".example.com";
    
    // Enable token fallback for cross-domain scenarios
    options.EnableTokenFallback = true; // Default: true
});
```

### 3. Advanced HttpClient Configuration

For more control over the HTTP client behavior:

```csharp
services.AddAuthorizer(
    options =>
    {
        options.AuthorizerUrl = "https://auth.example.com";
        options.RedirectUrl = "https://app.example.com/auth/callback";
        options.ClientId = "your-client-id";
        options.CookieDomain = ".example.com";
        options.EnableTokenFallback = true;
    },
    httpClient =>
    {
        // Additional HttpClient configuration if needed
        httpClient.DefaultRequestHeaders.Add("X-Custom-Header", "value");
    });
```

## How It Works

1. **Cookie-First Approach**: The SDK first attempts to authenticate using cookies.

2. **Automatic Fallback**: If cookie authentication fails with a 422 error (common in cross-domain scenarios), the SDK automatically falls back to token-based authentication.

3. **Token Storage**: Successful logins store access and refresh tokens in memory for fallback use.

4. **Seamless Operation**: Your application code remains unchanged - the fallback mechanism is transparent.

## Usage Example

```csharp
public class AuthController : ControllerBase
{
    private readonly IAuthorizerClient _authorizerClient;

    public AuthController(IAuthorizerClient authorizerClient)
    {
        _authorizerClient = authorizerClient;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // This works the same regardless of domain configuration
        var response = await _authorizerClient.LoginAsync(request);
        
        if (response.IsSuccess)
        {
            // Tokens are automatically stored for fallback use
            return Ok(response.Data);
        }

        return BadRequest(response.Errors);
    }

    [HttpGet("session")]
    public async Task<IActionResult> GetSession()
    {
        // Automatically tries cookies first, then falls back to tokens
        var response = await _authorizerClient.GetSessionAsync();
        
        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }

        // Enhanced error messages for cross-domain issues
        return BadRequest(response.Errors);
    }
}
```

## Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `UseCookies` | `true` | Enable cookie handling in HTTP requests |
| `UseCredentials` | `true` | Include credentials in CORS requests |
| `SetOriginHeader` | `true` | Automatically set Origin header for cross-domain requests |
| `CookieDomain` | `null` | Domain for cross-domain cookie sharing (use `.domain.com` format) |
| `EnableTokenFallback` | `true` | Enable token-based fallback when cookies fail |

## Error Handling

The SDK provides enhanced error messages for common cross-domain issues:

- **HTTP 422 Errors**: Automatically suggests enabling token fallback or checking CORS configuration
- **Detailed Logging**: Debug-level logs show when fallback mechanisms are triggered
- **Graceful Degradation**: Application continues to work even when cookies are blocked

## Custom Token Storage

By default, tokens are stored in memory. For production scenarios, you can implement a custom token storage:

```csharp
public class CustomTokenStorage : ITokenStorage
{
    // Implement your preferred storage mechanism
    // (e.g., Redis, database, encrypted cookies)
}

// Register in DI
services.AddSingleton<ITokenStorage, CustomTokenStorage>();
```

## Troubleshooting

### Common Issues

1. **Still getting 422 errors**: Ensure `CookieDomain` is set correctly with a leading dot (e.g., `.example.com`)

2. **Token fallback not working**: Verify `EnableTokenFallback` is set to `true` (default)

3. **CORS issues**: Check that your Authorizer instance allows requests from your application domain

### Debug Logging

Enable debug logging to see detailed information about authentication attempts:

```csharp
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

Look for log messages like:
- "Cookie-based session failed, attempting token-based fallback"
- "Token-based fallback succeeded"
- "Could not parse RedirectUrl to set Origin header"