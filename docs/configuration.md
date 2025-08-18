# Configuration Reference

This document provides a complete reference for configuring the Authorizer.DotNet SDK.

## Basic Configuration

### ASP.NET Core

```csharp
builder.Services.AddAuthorizer(builder.Configuration, "Authorizer");
```

### Console Applications

```csharp
var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

services.AddAuthorizer(configuration, "Authorizer");
```

## Configuration Options

### AuthorizerOptions

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `AuthorizerUrl` | `string` | Yes | The base URL of your Authorizer instance |
| `ClientId` | `string` | Yes | Your OAuth client ID |
| `RedirectUrl` | `string` | Yes | OAuth redirect URL |
| `ApiKey` | `string` | No | API key for server-to-server authentication |
| `HttpTimeout` | `TimeSpan` | No | HTTP request timeout (default: 30 seconds) |
| `ExtraHeaders` | `Dictionary<string, string>` | No | Additional headers for all requests |
| `UseSecureCookies` | `bool` | No | Use secure cookies (default: true) |
| `CookieDomain` | `string` | No | Domain for cross-domain cookie sharing (e.g., ".example.com") |
| `DisableBrowserHistory` | `bool` | No | Disable browser history for OAuth flows (default: false) |
| `UseCookies` | `bool` | No | Enable cookie handling in HTTP requests (default: true) |
| `UseCredentials` | `bool` | No | Include credentials in CORS requests (default: true) |
| `SetOriginHeader` | `bool` | No | Automatically set Origin header (default: true) |
| `EnableTokenFallback` | `bool` | No | Enable token-based fallback when cookies fail (default: true) |

### Configuration Sources

#### appsettings.json

```json
{
  "Authorizer": {
    "AuthorizerUrl": "https://your-authorizer-instance.com",
    "ClientId": "your-client-id",
    "RedirectUrl": "https://your-app.com/auth/callback",
    "ApiKey": "your-api-key",
    "HttpTimeout": "00:01:00",
    "UseSecureCookies": true,
    "CookieDomain": ".your-domain.com",
    "DisableBrowserHistory": false,
    "UseCookies": true,
    "UseCredentials": true,
    "SetOriginHeader": true,
    "EnableTokenFallback": true,
    "ExtraHeaders": {
      "X-Custom-Header": "value"
    }
  }
}
```

#### Environment Variables

Environment variables override appsettings.json:

```bash
export Authorizer__AuthorizerUrl=https://your-instance.com
export Authorizer__ClientId=your-client-id
export Authorizer__RedirectUrl=https://your-app.com/callback
export Authorizer__ApiKey=your-api-key
```

#### Direct Configuration

```csharp
services.Configure<AuthorizerOptions>(options =>
{
    options.AuthorizerUrl = "https://your-instance.com";
    options.ClientId = "your-client-id";
    options.RedirectUrl = "https://your-app.com/callback";
    options.HttpTimeout = TimeSpan.FromMinutes(2);
});
```

## Advanced Configuration

### Cross-Domain Authentication

For scenarios where your Authorizer instance and application are on different subdomains (e.g., `auth.example.com` and `app.example.com`):

```csharp
services.AddAuthorizer(options =>
{
    options.AuthorizerUrl = "https://auth.example.com";
    options.RedirectUrl = "https://app.example.com/auth/callback";
    options.ClientId = "your-client-id";
    
    // Cross-domain configuration
    options.UseCookies = true;                    // Enable cookie handling
    options.UseCredentials = true;                // Include credentials in CORS
    options.SetOriginHeader = true;               // Auto-set Origin header
    options.CookieDomain = ".example.com";        // Cross-domain cookie sharing
    options.EnableTokenFallback = true;           // Token fallback (default: true)
});
```

**Key Points:**
- `CookieDomain` should start with a dot (e.g., `.example.com`) for subdomain support
- Token fallback automatically handles cases where cookies fail across domains
- CORS must be properly configured on your Authorizer instance

### Custom HTTP Client

```csharp
services.AddHttpClient<AuthorizerHttpClient>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
});
```

### Dependency Injection

The SDK registers these services:

- `IAuthorizerClient` - Main client interface
- `AuthorizerClient` - Default implementation
- `AuthorizerHttpClient` - Internal HTTP client
- `AuthorizerOptions` - Configuration options

### Multiple Instances

For multiple Authorizer instances:

```csharp
// Register named configurations
services.Configure<AuthorizerOptions>("Instance1", config.GetSection("Authorizer:Instance1"));
services.Configure<AuthorizerOptions>("Instance2", config.GetSection("Authorizer:Instance2"));

// Register named clients
services.AddHttpClient<AuthorizerHttpClient>("Instance1");
services.AddHttpClient<AuthorizerHttpClient>("Instance2");

// Use named options in your services
public class MyService
{
    public MyService(
        IOptionsSnapshot<AuthorizerOptions> options,
        IHttpClientFactory httpClientFactory)
    {
        var options1 = options.Get("Instance1");
        var httpClient1 = httpClientFactory.CreateClient("Instance1");
        // Create AuthorizerClient instances as needed
    }
}
```

## Security Considerations

### API Keys

- Store API keys in secure configuration (Azure Key Vault, AWS Secrets Manager, etc.)
- Never commit API keys to source control
- Use environment variables for production deployments

### HTTPS Requirements

- Always use HTTPS for `AuthorizerUrl` in production
- Redirect URLs should use HTTPS in production
- Local development can use HTTP for `localhost`

### CORS Configuration

For browser-based applications, configure CORS on your Authorizer instance:

```json
{
  "allowed_origins": [
    "https://your-app.com",
    "http://localhost:3000"
  ]
}
```

## Validation

The SDK performs automatic validation:

- `AuthorizerUrl` must be a valid HTTP/HTTPS URL
- `ClientId` is required and cannot be empty
- `RedirectUrl` must be a valid URL
- `HttpTimeout` must be positive

## Troubleshooting

### Common Configuration Issues

1. **Invalid URL Format**
   ```
   Error: "Invalid AuthorizerUrl format"
   Solution: Ensure URL includes protocol (https://)
   ```

2. **Missing Required Fields**
   ```
   Error: "ClientId is required"
   Solution: Verify all required configuration values are set
   ```

3. **Connection Timeouts**
   ```
   Error: "Request timeout"
   Solution: Increase HttpTimeout or check network connectivity
   ```

4. **Unauthorized Errors**
   ```
   Error: "401 Unauthorized" 
   Solution: Verify ClientId and API key are correct
   ```

5. **Cross-Domain Session Issues**
   ```
   Error: "422 Unprocessable Entity" 
   Solution: Configure cross-domain options:
   - Set CookieDomain to ".yourdomain.com"
   - Enable UseCredentials and UseCookies
   - Verify CORS configuration on Authorizer instance
   ```

6. **Token Fallback Not Working**
   ```
   Error: Session validation continues to fail
   Solution: Check that EnableTokenFallback is true (default)
   and tokens are being stored during login
   ```

### Debug Configuration

Enable detailed logging to troubleshoot configuration issues:

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

This will log configuration values and HTTP requests/responses.