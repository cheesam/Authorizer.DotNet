# Migration Guide: v1.0.3 → v1.0.4

This guide helps you upgrade from Authorizer.DotNet v1.0.3 to v1.0.4, which includes breaking changes to ensure compatibility with current Authorizer API versions (v1.4.4+).

## 🔄 Breaking Changes Overview

Version 1.0.4 includes two major improvements:

1. **GraphQL Schema Compatibility**: Removes deprecated GraphQL fields that are no longer supported by current Authorizer instances, ensuring compatibility and eliminating 422 "Cannot query field" errors.

2. **Simplified Architecture**: Removes opinionated automatic fallback mechanisms in favor of explicit developer control, providing clearer error messages and more transparent authentication flows.

## 📋 Required Code Changes

### Part 1: GraphQL Schema Updates

### 1. SessionInfo Model Changes

#### Removed Properties:
```csharp
// ❌ These properties no longer exist:
sessionInfo.IsValid        // → Use response.IsSuccess instead
sessionInfo.CreatedAt       // → No replacement (deprecated)
sessionInfo.SessionToken   // → Use AccessToken, RefreshToken, or IdToken
sessionInfo.ExpiresAt       // → Use ExpiresIn instead
```

#### Updated Properties:
```csharp
// ✅ Use these instead:
sessionInfo.ExpiresIn       // Now returns seconds until expiration
```

### 2. Response Model Changes

#### LoginResponse & SignupResponse:
```csharp
// ❌ Removed properties:
loginResponse.Data.CreatedAt      // No longer available
loginResponse.Data.SessionToken  // No longer available

// ✅ Use existing properties:
loginResponse.Data.AccessToken   // Still available
loginResponse.Data.RefreshToken  // Still available  
loginResponse.Data.IdToken       // Still available
loginResponse.Data.ExpiresIn     // Updated field name
```

#### TokenResponse:
```csharp
// ❌ Removed property:
tokenResponse.Data.CreatedAt     // No longer available

// ✅ Use existing properties:
tokenResponse.Data.ExpiresIn     // Available
```

### Part 2: Architecture Simplification

The SDK has been simplified to remove opinionated automatic behaviors:

#### Removed Configuration Options:
```csharp
// ❌ These options no longer exist:
options.EnableTokenFallback = true;   // → Use explicit ValidateSessionWithTokenAsync
options.SetOriginHeader = true;       // → No longer needed
```

#### New Explicit Methods:
```csharp
// ✅ New explicit token-based session validation:
var response = await client.ValidateSessionWithTokenAsync(accessToken);
```

#### Enhanced Error Messages:
The SDK now provides clear, actionable error messages instead of hiding failures behind automatic retries.

## 🔧 Code Migration Examples

### GraphQL Schema Migration

### Example 1: Session Validation
```csharp
// ❌ Before (v1.0.3):
var session = await client.GetSessionAsync();
if (session.IsSuccess && session.Data?.IsValid == true)
{
    // Handle valid session
    var expiresAt = session.Data.ExpiresAt;
}

// ✅ After (v1.0.4):
var session = await client.GetSessionAsync();
if (session.IsSuccess && session.Data != null)
{
    // Handle valid session (IsSuccess indicates validity)
    var expiresIn = session.Data.ExpiresIn; // Seconds until expiration
}
```

### Example 2: Login Response Handling
```csharp
// ❌ Before (v1.0.3):
var login = await client.LoginAsync(request);
if (login.IsSuccess)
{
    var sessionToken = login.Data?.SessionToken;
    var createdAt = login.Data?.CreatedAt;
    var expiresAt = login.Data?.ExpiresAt;
}

// ✅ After (v1.0.4):
var login = await client.LoginAsync(request);
if (login.IsSuccess)
{
    var accessToken = login.Data?.AccessToken;   // Use for API calls
    var refreshToken = login.Data?.RefreshToken; // Use for token refresh
    var expiresIn = login.Data?.ExpiresIn;       // Seconds until expiration
}
```

### Simplified Architecture Migration

#### Example 4: Cross-Domain Authentication
```csharp
// ❌ Before (v1.0.3): Relied on automatic fallbacks
services.AddAuthorizer(options =>
{
    options.EnableTokenFallback = true;  // Automatic fallback
    options.SetOriginHeader = true;      // Automatic header setting
});

// Session handling relied on hidden automatic retries
var session = await client.GetSessionAsync(); // Could hide failures

// ✅ After (v1.0.4): Explicit control
services.AddAuthorizer(options =>
{
    // Removed opinionated options for developer control
});

// Explicit session handling with clear error messages
var session = await client.GetSessionAsync();
if (!session.IsSuccess)
{
    // Clear error messages guide next steps
    // For cross-domain, explicitly use token validation:
    var tokenSession = await client.ValidateSessionWithTokenAsync(accessToken);
}
```

### Example 5: API Controllers
```csharp
// ❌ Before (v1.0.3):
[HttpGet("session")]
public async Task<IActionResult> GetSession()
{
    var response = await _authorizerClient.GetSessionAsync();
    if (response.IsSuccess && response.Data != null)
    {
        return Ok(new
        {
            isValid = response.Data.IsValid,
            expiresAt = response.Data.ExpiresAt,
            sessionToken = response.Data.SessionToken
        });
    }
    return Unauthorized();
}

// ✅ After (v1.0.4):
[HttpGet("session")]
public async Task<IActionResult> GetSession()
{
    var response = await _authorizerClient.GetSessionAsync();
    if (response.IsSuccess && response.Data != null)
    {
        return Ok(new
        {
            hasValidSession = true,
            expiresIn = response.Data.ExpiresIn,
            accessToken = response.Data.AccessToken
        });
    }
    return Unauthorized();
}
```

## 🧪 Testing Your Migration

### 1. Check for Compilation Errors
```bash
dotnet build
```

Look for errors mentioning removed properties:
- `IsValid`
- `CreatedAt` 
- `SessionToken`
- `ExpiresAt`

### 2. Update Property References
Search your codebase for these patterns:
```bash
# Find potential issues:
grep -r "\.IsValid" .
grep -r "\.CreatedAt" .
grep -r "\.SessionToken" .
grep -r "\.ExpiresAt" .
```

### 3. Test Authentication Flows
Verify that authentication still works correctly:
```csharp
// Test basic login flow
var loginResponse = await client.LoginAsync(new LoginRequest 
{ 
    Email = "test@example.com", 
    Password = "password" 
});

Assert.True(loginResponse.IsSuccess);
Assert.NotNull(loginResponse.Data?.AccessToken);

// Test session retrieval
var sessionResponse = await client.GetSessionAsync();
Assert.True(sessionResponse.IsSuccess);
```

## 🔍 Troubleshooting

### Common Issues

#### "Property does not exist" errors:
```
Error CS1061: 'SessionInfo' does not contain a definition for 'IsValid'
```
**Solution**: Replace with appropriate logic using `response.IsSuccess`

#### "Cannot query field" GraphQL errors:
If you still see these errors, ensure you're using v1.0.4 and your Authorizer instance is v1.4.4+.

#### Token handling confusion:
**Old approach**: Relied on `SessionToken`
**New approach**: Use `AccessToken` for API calls, `RefreshToken` for renewals

## 🎯 Benefits After Migration

✅ **Compatibility**: Works with all current Authorizer versions (v1.4.4+)  
✅ **Reliability**: No more 422 GraphQL validation errors  
✅ **Future-proof**: Aligned with current Authorizer API schema  
✅ **Cleaner code**: Removes deprecated fields and confusion  
✅ **Transparency**: Clear error messages instead of hidden automatic retries  
✅ **Developer control**: Explicit methods for different authentication scenarios  
✅ **Simplicity**: Focused on core functionality without opinionated behaviors  

## 🆘 Need Help?

If you encounter issues during migration:

1. **Check the [CHANGELOG.md](../CHANGELOG.md)** for detailed changes
2. **Review the [examples](examples.md)** for updated patterns
3. **Run integration tests** against your Authorizer instance
4. **Open an issue** if you find compatibility problems

## 📚 Additional Resources

- [Current API Reference](api-reference.md)
- [Cross-Domain Authentication Guide](CROSS_DOMAIN_AUTHENTICATION.md)
- [Examples and Best Practices](examples.md)

---

**Migration Complete!** Your application should now work seamlessly with current Authorizer versions. 🎉