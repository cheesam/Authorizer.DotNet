# Changelog

All notable changes to the Authorizer.DotNet SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.2] - 2025-01-16

### Fixed
- Fixed failing unit test for `GetProfileAsync` method with correct HTTP client mocking
- Updated documentation with accurate test results (84/84 tests passing)
- Corrected file paths in testing documentation

### Improved
- Enhanced documentation accuracy and consistency
- Updated test quality documentation

## [1.0.1] - 2025-01-XX

### Added
- Initial project documentation
- Contributing guidelines
- MIT License

## [1.0.0] - 2025-01-XX

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