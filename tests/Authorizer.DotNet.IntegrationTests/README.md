# Authorizer.DotNet Integration Tests

This project contains integration tests that verify the Authorizer.DotNet client works correctly with real Authorizer instances.

## Quick Start

1. **Configure your instance**: Create `appsettings.personal.json` (gitignored):
   ```json
   {
     "Authorizer": {
       "AuthorizerUrl": "https://your-authorizer-instance.com", 
       "ClientId": "your-client-id",
       "RedirectUrl": "http://localhost:8080/auth/callback"
     }
   }
   ```

2. **Configure OAuth redirect**: In your Authorizer instance, allow `http://localhost:8080/auth/callback` as a valid redirect URI

3. **Run tests**: `dotnet test`

## Complete Documentation

For comprehensive setup instructions, configuration options, and troubleshooting, see:

**📖 [Integration Testing Guide](../../docs/integration-testing.md)**

The centralized guide covers:
- Detailed configuration options and environment variables
- OAuth callback server setup and usage
- Complete test coverage overview  
- Test data cleanup mechanisms
- Troubleshooting common issues
- Adding new integration tests
- Instance-specific variations and error handling

## Test Results

All tests should pass against a properly configured Authorizer instance. The tests are designed to:
- Create unique test data for each run
- Handle instance-specific variations gracefully  
- Clean up test data automatically
- Validate both success and error scenarios