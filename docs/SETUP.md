# Setup Guide - Authorizer.DotNet

This guide helps you set up and configure the Authorizer.DotNet SDK with your Authorizer instance.

## Quick Setup

### 1. Prerequisites

- .NET 6.0 or later
- An Authorizer.dev instance (cloud or self-hosted)
- Your Authorizer client credentials

### 2. Get Your Credentials

From your Authorizer dashboard, you'll need:

- **AuthorizerUrl**: Your Authorizer instance URL (e.g., `https://your-app.authorizer.dev`)
- **ClientId**: Your application's client ID
- **RedirectUrl**: Your application's callback URL

## Configuration Methods

### Option 1: appsettings.json (Recommended)

```json
{
  "Authorizer": {
    "AuthorizerUrl": "https://your-authorizer-instance.com",
    "ClientId": "your-client-id", 
    "RedirectUrl": "https://your-app.com/auth/callback",
    "HttpTimeout": "00:00:30"
  }
}
```

### Option 2: Environment Variables

```bash
export AUTHORIZER_URL="https://your-authorizer-instance.com"
export AUTHORIZER_CLIENT_ID="your-client-id"
export AUTHORIZER_REDIRECT_URL="https://your-app.com/auth/callback"
```

### Option 3: Direct Configuration

```csharp
services.AddAuthorizer(options =>
{
    options.AuthorizerUrl = "https://your-authorizer-instance.com";
    options.ClientId = "your-client-id";
    options.RedirectUrl = "https://your-app.com/auth/callback";
});
```

## Running Samples

### Console Sample

1. Update `samples/Authorizer.Sample.Console/appsettings.json`:
```json
{
  "Authorizer": {
    "AuthorizerUrl": "https://your-authorizer-instance.com",
    "ClientId": "your-client-id",
    "RedirectUrl": "http://localhost:3000/auth/callback"
  }
}
```

2. Run the sample:
```bash
cd samples/Authorizer.Sample.Console
dotnet run
```

### ASP.NET Core Sample

1. Update `samples/Authorizer.Sample.AspNetCore/appsettings.json` with your credentials
2. Run the sample:
```bash
cd samples/Authorizer.Sample.AspNetCore
dotnet run
```

3. Open https://localhost:5001/swagger to test the API

### Blazor Server Sample

1. Update `samples/Authorizer.Sample.BlazorServer/appsettings.json` with your credentials
2. Run the sample:
```bash
cd samples/Authorizer.Sample.BlazorServer
dotnet run
```

3. Open https://localhost:5001 to test the UI

## Running Integration Tests

The integration tests validate the SDK against a real Authorizer instance:

### Method 1: Environment Variables

```bash
export AUTHORIZER_URL="https://your-authorizer-instance.com"
export AUTHORIZER_CLIENT_ID="your-client-id"  
export AUTHORIZER_REDIRECT_URL="https://your-app.com/auth/callback"

cd integration-test
dotnet run
```

### Method 2: Update Source Code

Edit `integration-test/ComprehensiveIntegrationTest.cs` and replace the fallback values:

```csharp
var configData = new Dictionary<string, string?>
{
    ["Authorizer:AuthorizerUrl"] = Environment.GetEnvironmentVariable("AUTHORIZER_URL") ?? "https://YOUR-INSTANCE.com",
    ["Authorizer:ClientId"] = Environment.GetEnvironmentVariable("AUTHORIZER_CLIENT_ID") ?? "YOUR-CLIENT-ID", 
    ["Authorizer:RedirectUrl"] = Environment.GetEnvironmentVariable("AUTHORIZER_REDIRECT_URL") ?? "https://YOUR-APP.com/auth/callback"
};
```

### Integration Test Results

The integration tests will validate:
- ✅ Connection to your Authorizer instance
- ✅ User signup and login flows
- ✅ Session management and JWT validation  
- ✅ Profile retrieval and user management
- ✅ Error handling and edge cases
- ✅ OAuth flows and rate limiting

Expected results: **10-13 tests passing** (some may fail due to instance configuration differences)

## Development vs Production

### Development Configuration

```json
{
  "Authorizer": {
    "AuthorizerUrl": "http://localhost:8080", 
    "ClientId": "dev-client-id",
    "RedirectUrl": "http://localhost:3000/auth/callback",
    "UseSecureCookies": false,
    "HttpTimeout": "00:01:00"
  },
  "Logging": {
    "LogLevel": {
      "Authorizer.DotNet": "Debug"
    }
  }
}
```

### Production Configuration

```json
{
  "Authorizer": {
    "AuthorizerUrl": "https://your-production-instance.com",
    "ClientId": "prod-client-id", 
    "RedirectUrl": "https://your-app.com/auth/callback",
    "UseSecureCookies": true,
    "HttpTimeout": "00:00:30"
  },
  "Logging": {
    "LogLevel": {
      "Authorizer.DotNet": "Information"
    }
  }
}
```

## Common Configuration Options

| Option | Description | Default | Example |
|--------|-------------|---------|---------|
| `AuthorizerUrl` | Your Authorizer instance URL | *Required* | `https://app.authorizer.dev` |
| `ClientId` | Your application client ID | *Required* | `abc123-def456-ghi789` |
| `RedirectUrl` | OAuth callback URL | *Required* | `https://app.com/callback` |
| `ApiKey` | Server-to-server API key | `null` | `sk_123456789` |
| `HttpTimeout` | HTTP request timeout | `00:00:30` | `00:01:00` |
| `UseSecureCookies` | Secure cookie flag | `true` | `false` (dev only) |
| `CookieDomain` | Cookie domain scope | `null` | `.yourdomain.com` |
| `ExtraHeaders` | Additional HTTP headers | `{}` | `{"X-App": "MyApp"}` |

## Security Considerations

### 🔒 Production Checklist

- [ ] Use HTTPS for `AuthorizerUrl` 
- [ ] Set `UseSecureCookies` to `true`
- [ ] Store sensitive config in secure storage (Azure Key Vault, etc.)
- [ ] Use restrictive `CookieDomain` settings
- [ ] Enable HTTPS redirect in your application
- [ ] Set appropriate CORS policies
- [ ] Implement proper token storage and rotation

### 🚫 Never Do This

```csharp
// ❌ Don't disable SSL validation in production
services.ConfigureHttpClientDefaults(http => 
{
    http.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
    });
});
```

### ✅ Production Best Practices

```csharp
// ✅ Use secure configuration
services.AddAuthorizer(options =>
{
    options.AuthorizerUrl = configuration["Authorizer:AuthorizerUrl"]; // From secure config
    options.ClientId = configuration["Authorizer:ClientId"];
    options.RedirectUrl = configuration["Authorizer:RedirectUrl"];
    options.UseSecureCookies = true;
    options.HttpTimeout = TimeSpan.FromSeconds(30);
});

// ✅ Add security headers
app.UseHsts();
app.UseHttpsRedirection();
```

## Troubleshooting

### Connection Issues

**Problem**: `Cannot connect to Authorizer instance`
- ✅ Verify `AuthorizerUrl` is correct and accessible
- ✅ Check network connectivity and firewalls
- ✅ Ensure your Authorizer instance is running

**Problem**: `SSL/TLS errors`
- ✅ Use HTTPS URLs in production
- ✅ Verify SSL certificate validity
- ✅ For development, consider using HTTP (with secure cookies disabled)

### Authentication Issues

**Problem**: `401 Unauthorized`
- ✅ Verify `ClientId` matches your Authorizer configuration
- ✅ Check if API key is required and properly configured
- ✅ Ensure tokens are being passed correctly

**Problem**: `OAuth redirect mismatch`
- ✅ Verify `RedirectUrl` matches exactly in Authorizer dashboard
- ✅ Include protocol (http/https) and port if needed
- ✅ Check for trailing slashes or case sensitivity

### Performance Issues

**Problem**: `Request timeouts`
- ✅ Increase `HttpTimeout` setting
- ✅ Check network latency to your Authorizer instance
- ✅ Monitor Authorizer instance performance

**Problem**: `High memory usage`
- ✅ Ensure you're using dependency injection properly
- ✅ Don't create new `IAuthorizerClient` instances manually
- ✅ Let the framework manage HttpClient lifecycle

### Debug Logging

Enable detailed logging to troubleshoot issues:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Authorizer.DotNet": "Debug",
      "System.Net.Http": "Debug"
    }
  }
}
```

This will log all HTTP requests/responses for debugging.

## Getting Help

- 📖 [Full Documentation](README.md)
- 🐛 [Report Issues](https://github.com/authorizerdev/authorizer-dotnet/issues)
- 💬 [Discord Community](https://discord.gg/Zv2D5h6kkK)
- 📧 [Email Support](mailto:support@authorizer.dev)

---

**Ready to start building? Check out the [samples](samples/) and [README](README.md) for detailed usage examples!**