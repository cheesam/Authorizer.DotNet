# Changelog

All notable changes to the Authorizer.DotNet SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.4] - 2025-08-19

### Added
- **New Explicit Authentication Methods**
  - `ValidateSessionWithTokenAsync()` method for explicit token-based session validation
  - Clear alternative to automatic fallback mechanisms for cross-domain scenarios
  - Developer control over authentication flow selection

### Changed
- **Simplified Architecture** (BREAKING CHANGES)
  - Removed opinionated automatic fallback mechanisms from `GetSessionAsync()` 
  - Removed automatic cookie-to-token fallback behavior for transparency
  - Removed configuration options: `EnableTokenFallback`, `SetOriginHeader`
  - Enhanced error messages with clear, actionable guidance instead of technical GraphQL errors
  - Focused on simplicity and developer control rather than hidden automatic behaviors

### Fixed
- **GraphQL Schema Compatibility**
  - Updated GraphQL queries to use current Authorizer API v1.4.4+ field names
  - Changed `expires_at` → `expires_in` in session and authentication responses
  - Removed deprecated `created_at` field from TokenResponse, LoginResponse, and SignupResponse
  - Removed deprecated `session_token` field from LoginResponse and SignupResponse  
  - Removed deprecated `is_valid` field from SessionInfo model
  - Fixed session query to remove invalid `session_token` parameter

- **Authentication Reliability**
  - Resolved HTTP 422 "Cannot query field" errors when using current Authorizer instances
  - Fixed "Unknown argument 'session_token'" errors in session queries
  - Eliminated GraphQL validation failures due to outdated field references

### Enhanced
- **API Compatibility**
  - Full compatibility with Authorizer v1.4.4 and later versions
  - Updated SessionInfo model to match current GraphQL schema structure
  - Improved response model accuracy for authentication operations

- **Developer Experience**
  - Clear, actionable error messages replace technical GraphQL failures
  - Explicit methods give developers full control over authentication flow
  - Transparent error handling without hidden automatic retries
  - Enhanced error messages for common issues (401, 422, 403) with specific guidance

- **Cross-Domain Authentication**
  - Developers can explicitly choose between cookie-based and token-based validation
  - Clear error messages explain cross-domain restrictions and solutions
  - `ValidateSessionWithTokenAsync()` provides explicit alternative for cross-domain scenarios

### Security
- **Credential Protection**
  - Replaced all real credentials in sample code with safe placeholders
  - Updated configuration examples to use `https://sample-app.authorizer.dev`
  - Added gitignore rules to prevent accidental credential commits
  - Secured integration test configurations

### Documentation
- **Comprehensive Updates**
  - Updated README.md to reflect simplified architecture and new methods
  - Enhanced examples with explicit token validation patterns
  - Updated migration guide with architectural simplification changes
  - Added "Simplified v1.0.4 Features" section demonstrating new approach
  - Updated troubleshooting guide with simplified solutions

- **API Documentation**
  - Documented new `ValidateSessionWithTokenAsync()` method
  - Updated cross-domain authentication examples
  - Added enhanced error message examples
  - Updated configuration examples to remove deprecated options

### Compatibility
- **Breaking Changes**
  - Removed deprecated model properties (`IsValid`, `CreatedAt`, `SessionToken`)
  - Removed configuration options (`EnableTokenFallback`, `SetOriginHeader`)
  - `GetSessionAsync()` no longer automatically falls back to token-based authentication
  - Updated sample applications to use correct property references and explicit patterns
  - Integration tests now validate against current Authorizer API schema

### Migration
- **Simplified Migration Path**
  - Added comprehensive migration guide (`docs/MIGRATION_GUIDE_v1.0.4.md`)
  - Clear examples showing before/after patterns for both GraphQL and architectural changes
  - Step-by-step guidance for updating from automatic to explicit authentication patterns

## [1.0.3] - 2025-08-18

### Added
- **Cross-Domain Authentication Support**
  - Automatic cookie-based and token-based authentication fallback for cross-subdomain scenarios
  - New configuration options: `UseCookies`, `UseCredentials`, `SetOriginHeader`, `EnableTokenFallback`
  - Cross-domain cookie sharing with `CookieDomain` configuration (e.g., `.example.com`)
  - Enhanced error handling for 422 responses with helpful troubleshooting messages
  - Token storage interface (`ITokenStorage`) with in-memory implementation (`InMemoryTokenStorage`)
  - Automatic Origin header setting for cross-domain requests
  - Seamless fallback when `GetSessionAsync()` fails due to cross-domain cookie restrictions

### Enhanced
- **HTTP Client Configuration**
  - Improved cookie container setup for cross-domain scenarios with proper subdomain support
  - CORS credentials support with `Access-Control-Allow-Credentials` header
  - Enhanced error messages for common cross-domain authentication issues (422, 401, 403, etc.)
  - Better cookie domain handling with automatic dot prefix for subdomain compatibility

### Fixed
- **Cross-Domain Session Validation**
  - Fixed `GetSessionAsync()` failures in cross-subdomain scenarios (e.g., auth.example.com ↔ app.example.com)
  - Resolved HTTP 422 errors when cookies are not accessible across different subdomains
  - Improved session persistence between OAuth callback and application domain

### Documentation
- Added comprehensive cross-domain authentication guide (`docs/CROSS_DOMAIN_AUTHENTICATION.md`)
- Updated README.md with cross-domain configuration examples and troubleshooting
- Enhanced configuration documentation with new cross-domain options
- Updated examples with cross-domain scenarios and session management
- Improved troubleshooting section with cross-domain solutions and debugging tips

### Compatibility
- Maintains full backward compatibility with existing single-domain deployments
- All new cross-domain features are enabled by default but gracefully fall back if not needed
- No breaking changes to existing API or configuration

## [1.0.2] - 2025-08-16

### Fixed
- Fixed failing unit test for `GetProfileAsync` method with correct HTTP client mocking
- Updated documentation with accurate test results (84/84 tests passing)
- Corrected file paths in testing documentation

### Improved
- Enhanced documentation accuracy and consistency
- Updated test quality documentation

## [1.0.1] - 2025-07-31

### Added
- Initial project documentation
- Contributing guidelines
- MIT License

## [1.0.0] - 2025-07-31

### Added
- **Core Authentication Features**
  - User login with email/password authentication
  - User signup with comprehensive registration data
  - OAuth 2.0 authorization code flow with PKCE support
  - JWT token validation and refresh capabilities
  - Secure logout functionality

- **User Management**
  - User profile retrieval and management
  - Session information and validation
  - Multi-factor authentication support

- **Password Management**
  - Forgot password flow initiation
  - Password reset with secure token validation
  - Password change for authenticated users

- **Email Verification**
  - Email verification with secure tokens
  - Resend verification email capability

- **SDK Features**
  - .NET 6.0, 7.0, and 8.0 support
  - Native dependency injection integration
  - Comprehensive error handling with structured responses
  - Built-in HTTP client with connection pooling
  - Configurable timeouts and retry policies
  - Structured logging with configurable levels
  - Full async/await support with cancellation tokens

- **Framework Integration**
  - ASP.NET Core integration with JWT authentication
  - Blazor Server support with cookie authentication
  - Console application examples
  - Worker service compatibility

- **Configuration Options**
  - Flexible configuration via `appsettings.json`
  - Programmatic configuration with fluent API
  - Custom HTTP client configuration support
  - Environment-specific settings

- **Validation & Security**
  - Input validation with data annotations
  - Secure token storage recommendations
  - HTTPS enforcement options
  - Custom header support for advanced scenarios

- **Testing Infrastructure**
  - Comprehensive unit test suite (84 tests)
  - Integration testing examples
  - Mocking framework integration
  - Code coverage reporting

- **Documentation & Samples**
  - Complete API documentation
  - ASP.NET Core API sample with Swagger
  - Blazor Server sample with authentication
  - Console application sample
  - Performance and security best practices
  - Migration guide from JavaScript SDK

### Security
- Implemented secure token handling
- Added HTTPS enforcement options
- Included security best practices documentation
- Added input validation and sanitization

### Performance
- Optimized HTTP client with connection pooling
- Efficient JSON serialization with System.Text.Json
- Minimal memory allocations
- Proper async/await patterns with ConfigureAwait(false)

---

## Release Notes Format

### Types of Changes
- **Added** for new features
- **Changed** for changes in existing functionality
- **Deprecated** for soon-to-be removed features
- **Removed** for now removed features
- **Fixed** for any bug fixes
- **Security** for vulnerability fixes

### Version Guidelines
- **MAJOR**: Breaking changes that require code modifications
- **MINOR**: New features that are backward compatible
- **PATCH**: Bug fixes and minor improvements

---

## Contributing

When adding entries to the changelog:

1. **Add new entries under [Unreleased]**
2. **Use the format above** for consistency
3. **Include issue/PR references** when applicable
4. **Group related changes** under appropriate categories
5. **Move entries to versioned section** on release

Example entry:
```markdown
### Added
- New OAuth provider support for Google (#123)
- Enhanced error messages with detailed validation (#124)

### Fixed
- Fixed timeout issue in login flow (#125)
- Resolved memory leak in HTTP client (#126)
```

## Support

For questions about specific releases or changes:
- Check the [documentation](https://docs.authorizer.dev)
- Open an [issue](https://github.com/authorizerdev/authorizer-dotnet/issues)
- Join our [Discord community](https://discord.gg/Zv2D5h6kkK)
