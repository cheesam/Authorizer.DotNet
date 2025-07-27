# Contributing to Authorizer.DotNet

Thank you for your interest in contributing to the Authorizer.DotNet SDK! We welcome contributions from the community and are grateful for any help you can provide.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Making Changes](#making-changes)
- [Testing](#testing)
- [Submitting Changes](#submitting-changes)
- [Style Guidelines](#style-guidelines)
- [Reporting Issues](#reporting-issues)

## Code of Conduct

This project adheres to a code of conduct that we expect all contributors to follow. Please be respectful and constructive in all interactions.

## Getting Started

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/your-username/authorizer-dotnet.git
   cd authorizer-dotnet
   ```
3. **Add the upstream remote**:
   ```bash
   git remote add upstream https://github.com/authorizerdev/authorizer-dotnet.git
   ```

## Development Setup

### Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022, VS Code, or JetBrains Rider
- Git

### Building the Project

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

### Project Structure

```
├── src/
│   └── Authorizer.DotNet/          # Main SDK library
├── tests/
│   └── Authorizer.DotNet.Tests/    # Unit and integration tests
├── samples/                        # Example applications
│   ├── Authorizer.Sample.AspNetCore/
│   ├── Authorizer.Sample.BlazorServer/
│   └── Authorizer.Sample.Console/
└── docs/                          # Documentation
```

## Making Changes

### Branch Naming

Use descriptive branch names:
- `feature/add-oauth-support`
- `fix/login-timeout-issue`
- `docs/update-readme`
- `test/improve-coverage`

### Commit Messages

Follow conventional commit format:
- `feat: add OAuth 2.0 PKCE support`
- `fix: resolve timeout issues in login flow`
- `docs: update API documentation`
- `test: add unit tests for SignupRequest`
- `refactor: improve error handling`

### Development Workflow

1. **Create a feature branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes** following our coding standards

3. **Add tests** for new functionality

4. **Run tests** to ensure everything works:
   ```bash
   dotnet test
   ```

5. **Update documentation** if needed

6. **Commit your changes**:
   ```bash
   git add .
   git commit -m "feat: add your feature description"
   ```

7. **Push to your fork**:
   ```bash
   git push origin feature/your-feature-name
   ```

8. **Create a Pull Request** on GitHub

## Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Guidelines

- **Write tests for all new features**
- **Maintain existing test coverage**
- **Use descriptive test names** that explain the scenario
- **Follow AAA pattern**: Arrange, Act, Assert
- **Mock external dependencies** appropriately
- **Test both success and failure scenarios**

### Test Categories

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test component interactions
- **Validation Tests**: Test input validation and data annotations

## Submitting Changes

### Pull Request Process

1. **Update documentation** for any new features
2. **Add or update tests** as needed
3. **Ensure all tests pass**
4. **Update CHANGELOG.md** with your changes
5. **Create a Pull Request** with:
   - Clear title and description
   - Reference any related issues
   - List of changes made
   - Screenshots if applicable

### Pull Request Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] Tests added/updated
- [ ] CHANGELOG.md updated
```

## Style Guidelines

### C# Coding Standards

- **Use PascalCase** for public members, classes, and methods
- **Use camelCase** for private fields and local variables
- **Use meaningful names** for variables and methods
- **Add XML documentation** for public APIs
- **Follow .NET naming conventions**
- **Use `var`** when type is obvious
- **Prefer explicit types** when clarity is important

### Code Example

```csharp
/// <summary>
/// Authenticates a user with email and password.
/// </summary>
/// <param name="request">The login request containing credentials.</param>
/// <param name="cancellationToken">Cancellation token for the operation.</param>
/// <returns>Authentication response containing user data and tokens.</returns>
public async Task<AuthorizerResponse<LoginResponse>> LoginAsync(
    LoginRequest request, 
    CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(request);
    
    var query = BuildLoginQuery(request);
    return await _httpClient.PostGraphQLAsync<LoginResponse>(query, request, cancellationToken);
}
```

### Documentation Standards

- **Use XML documentation** for all public APIs
- **Include parameter descriptions**
- **Document return values**
- **Add code examples** for complex scenarios
- **Keep README.md updated**

## Reporting Issues

### Bug Reports

When reporting bugs, please include:

- **Clear description** of the issue
- **Steps to reproduce** the problem
- **Expected behavior**
- **Actual behavior**
- **Environment details** (.NET version, OS, etc.)
- **Code samples** if applicable
- **Error messages** and stack traces

### Feature Requests

For feature requests, please provide:

- **Clear description** of the proposed feature
- **Use case** and motivation
- **Proposed API** if applicable
- **Alternative solutions** considered

### Issue Templates

Use the provided issue templates on GitHub for consistency.

## Release Process

### Versioning

We follow [Semantic Versioning](https://semver.org/):
- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

### Release Checklist

- [ ] Update version numbers
- [ ] Update CHANGELOG.md
- [ ] Run full test suite
- [ ] Update documentation
- [ ] Create GitHub release
- [ ] Publish NuGet package

## Getting Help

If you need help or have questions:

- **Check existing issues** on GitHub
- **Join our Discord** community
- **Read the documentation**
- **Ask questions** in discussions

## Recognition

Contributors will be recognized in:
- CHANGELOG.md
- GitHub contributors list
- Release notes

Thank you for contributing to Authorizer.DotNet!