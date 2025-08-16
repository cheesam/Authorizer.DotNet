# Integration Testing Guide

This guide covers how to set up and run integration tests for Authorizer.DotNet.

## Overview

The integration tests verify that the Authorizer.DotNet client works correctly with real Authorizer instances. These tests are located in the `tests/Authorizer.DotNet.IntegrationTests/` project.

## Configuration

### Personal Configuration

Create an `appsettings.personal.json` file in the integration tests directory (this file is gitignored):

```json
{
  "Authorizer": {
    "AuthorizerUrl": "https://your-authorizer-instance.com",
    "ClientId": "your-client-id",
    "RedirectUrl": "http://localhost:8080/auth/callback"
  }
}
```

### Environment Variables

You can override settings using environment variables:
- `Authorizer__AuthorizerUrl`
- `Authorizer__ClientId`  
- `Authorizer__RedirectUrl`

### Authorizer Instance Setup

**Important**: Configure your Authorizer instance to allow `http://localhost:8080/auth/callback` as a valid redirect URI in your OAuth settings.

## Running Tests

```bash
# Run all integration tests
dotnet test tests/Authorizer.DotNet.IntegrationTests/

# Run with verbose output
dotnet test tests/Authorizer.DotNet.IntegrationTests/ --logger "console;verbosity=detailed"

# Run specific test
dotnet test tests/Authorizer.DotNet.IntegrationTests/ --filter "GetMetaAsync"
```

## Test Coverage

The integration tests cover:

### Core Authentication
- User signup (`SignupAsync`)
- User login (`LoginAsync`)
- User logout (`LogoutAsync`)
- Session management (`GetSessionAsync`)

### User Management
- Profile retrieval (`GetProfileAsync`)
- User deletion (`DeleteUserAsync`)

### Token Management
- JWT validation (`ValidateJwtAsync`)
- Token exchange (`GetTokenAsync`)

### Password Management
- Password reset (`ForgotPasswordAsync`)

### OAuth Flow
- Authorization flow (`AuthorizeAsync`)
- Local callback server handling

### Error Scenarios
- Invalid credentials
- Invalid tokens
- Network errors

## OAuth Testing

The tests include a `LocalCallbackServer` helper for testing OAuth flows:

```csharp
using var callbackServer = new LocalCallbackServer(8080);

var authorizeRequest = new AuthorizeRequest
{
    RedirectUri = callbackServer.CallbackUrl,
    // ... other properties
};

// For manual testing, you can wait for the callback
var callbackResult = await callbackServer.StartAndWaitForCallbackAsync(TimeSpan.FromMinutes(5));
```

## Test Data Cleanup

Tests automatically clean up after themselves:

1. **Unique Test Data**: Each test uses unique email addresses with GUIDs
2. **Automatic Cleanup**: Uses `DeleteUserAsync()` to remove test users
3. **Graceful Failure**: Cleanup ignores failures to avoid test failures
4. **Isolation**: Each test is independent

## Common Issues

### "unauthorized" Error for Delete Operations
User deletion may require admin privileges in some Authorizer instances. Tests handle this gracefully.

### GraphQL Field Name Differences
Different Authorizer versions may use different GraphQL field names. The client handles common variations.

### Token Structure Variations
Different instances may return slightly different token structures. Tests are designed to be flexible.

## Adding New Tests

When adding integration tests:

1. Use unique test data (GUIDs in email addresses)
2. Add cleanup for any created resources
3. Handle instance-specific variations gracefully
4. Test both success and error scenarios
5. Make tests independent of execution order

For more details, see the [complete integration test source](../tests/Authorizer.DotNet.IntegrationTests/AuthorizerClientIntegrationTests.cs).