# API Reference

This document provides a complete API reference for the Authorizer.DotNet SDK.

## IAuthorizerClient Interface

The main interface for interacting with Authorizer.dev.

### Authentication Methods

#### LoginAsync
```csharp
Task<AuthorizerResponse<LoginResponse>> LoginAsync(
    LoginRequest request, 
    CancellationToken cancellationToken = default)
```

Authenticates a user with email and password.

**Parameters:**
- `request` - Login credentials and optional parameters
- `cancellationToken` - Cancellation token

**Returns:** `AuthorizerResponse<LoginResponse>` with access tokens and user information

#### SignupAsync
```csharp
Task<AuthorizerResponse<SignupResponse>> SignupAsync(
    SignupRequest request, 
    CancellationToken cancellationToken = default)
```

Creates a new user account.

#### AuthorizeAsync
```csharp
Task<AuthorizerResponse<AuthorizeResponse>> AuthorizeAsync(
    AuthorizeRequest request, 
    CancellationToken cancellationToken = default)
```

Initiates OAuth authorization flow.

#### GetTokenAsync
```csharp
Task<AuthorizerResponse<TokenResponse>> GetTokenAsync(
    GetTokenRequest request, 
    CancellationToken cancellationToken = default)
```

Exchanges authorization code for access tokens.

### User Management

#### GetProfileAsync
```csharp
Task<AuthorizerResponse<UserProfile>> GetProfileAsync(
    string accessToken, 
    CancellationToken cancellationToken = default)
```

Retrieves user profile information.

#### GetSessionAsync
```csharp
Task<AuthorizerResponse<SessionInfo>> GetSessionAsync(
    string? sessionToken = null, 
    CancellationToken cancellationToken = default)
```

Gets current session information.

#### LogoutAsync
```csharp
Task<AuthorizerResponse<bool>> LogoutAsync(
    string? sessionToken = null, 
    CancellationToken cancellationToken = default)
```

Logs out the current user.

#### DeleteUserAsync
```csharp
Task<AuthorizerResponse<bool>> DeleteUserAsync(
    DeleteUserRequest request, 
    CancellationToken cancellationToken = default)
```

Deletes a user account (may require admin privileges).

### Token Validation

#### ValidateJwtAsync
```csharp
Task<AuthorizerResponse<UserProfile>> ValidateJwtAsync(
    string token, 
    CancellationToken cancellationToken = default)
```

Validates a JWT token and returns user information.

#### VerifyEmailAsync
```csharp
Task<AuthorizerResponse<bool>> VerifyEmailAsync(
    VerifyEmailRequest request, 
    CancellationToken cancellationToken = default)
```

Verifies a user's email address.

### Password Management

#### ForgotPasswordAsync
```csharp
Task<AuthorizerResponse<bool>> ForgotPasswordAsync(
    string email, 
    CancellationToken cancellationToken = default)
```

Initiates password reset process.

#### ResetPasswordAsync
```csharp
Task<AuthorizerResponse<bool>> ResetPasswordAsync(
    ResetPasswordRequest request, 
    CancellationToken cancellationToken = default)
```

Resets password using reset token.

#### ChangePasswordAsync
```csharp
Task<AuthorizerResponse<bool>> ChangePasswordAsync(
    ChangePasswordRequest request, 
    CancellationToken cancellationToken = default)
```

Changes user password.

### Metadata

#### GetMetaAsync
```csharp
Task<AuthorizerResponse<MetaInfo>> GetMetaAsync(
    CancellationToken cancellationToken = default)
```

Retrieves Authorizer instance metadata.

## Request Models

### LoginRequest
```csharp
public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Scope { get; set; }
    public string? State { get; set; }
    public string[]? Roles { get; set; }
}
```

### SignupRequest
```csharp
public class SignupRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? MiddleName { get; set; }
    public string? Nickname { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Picture { get; set; }
    public string? Birthdate { get; set; }
    public string? Gender { get; set; }
    public string? Scope { get; set; }
    public string? State { get; set; }
    public string[]? Roles { get; set; }
    public string? RedirectUri { get; set; }
}
```

### AuthorizeRequest
```csharp
public class AuthorizeRequest
{
    public string ResponseType { get; set; }
    public string ClientId { get; set; }
    public string RedirectUri { get; set; }
    public string? Scope { get; set; }
    public string? State { get; set; }
    public string? CodeChallenge { get; set; }
    public string? CodeChallengeMethod { get; set; }
    public string? Nonce { get; set; }
    public string? ResponseMode { get; set; }
    public string? Prompt { get; set; }
    public int? MaxAge { get; set; }
    public string? UiLocales { get; set; }
    public string? LoginHint { get; set; }
}
```

## Response Models

### AuthorizerResponse<T>
```csharp
public class AuthorizerResponse<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public IReadOnlyList<AuthorizerError>? Errors { get; }
    public string? Message { get; }
}
```

### LoginResponse
```csharp
public class LoginResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? IdToken { get; set; }
    public string? TokenType { get; set; } = "Bearer";
    public long? ExpiresIn { get; set; }
    public User? User { get; set; }
    public bool? ShouldShowEmailOtpScreen { get; set; }
    public bool? ShouldShowMobileOtpScreen { get; set; }
}
```

### UserProfile
```csharp
public class UserProfile
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? MiddleName { get; set; }
    public string? Nickname { get; set; }
    public string? Picture { get; set; }
    public string? Gender { get; set; }
    public string? Birthdate { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? PhoneNumberVerified { get; set; }
    public long? CreatedAt { get; set; }
    public long? UpdatedAt { get; set; }
    public string[]? Roles { get; set; }
    public bool? IsMultiFactorAuthEnabled { get; set; }
    public long? RevokedTimestamp { get; set; }
    public string[]? SignupMethods { get; set; }
    public Dictionary<string, object>? AppData { get; set; }
}
```

## Error Handling

All methods return `AuthorizerResponse<T>` with error information:

```csharp
var response = await client.LoginAsync(request);
if (!response.IsSuccess)
{
    foreach (var error in response.Errors ?? [])
    {
        Console.WriteLine($"Error: {error.Message}");
    }
}
```

## Extension Methods

### Service Collection Extensions

```csharp
services.AddAuthorizer(configuration, "Authorizer");
```

Registers all necessary services for dependency injection.

For complete usage examples, see the [Examples Guide](examples.md).