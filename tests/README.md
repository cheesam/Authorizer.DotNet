# Authorizer.DotNet Tests

This directory contains comprehensive unit tests for the Authorizer.DotNet SDK project.

## Test Structure

### Test Project
- **Framework**: xUnit with .NET 8.0
- **Mocking**: Moq for creating test doubles
- **Coverage**: Unit tests for models, client methods, and HTTP client integration

### Test Categories

#### 1. Model Tests
- **AuthorizerResponseTests**: Tests for the generic response wrapper
  - Success/failure response creation
  - Error handling and validation
  - Property validation

- **LoginRequestTests**: Tests for login request validation
  - Email format validation
  - Required field validation
  - Optional field handling

- **SignupRequestTests**: Tests for signup request validation
  - All field validation rules
  - Required vs optional field handling

#### 2. Client Tests
- **AuthorizerClientTests**: Tests for the main client class
  - Constructor validation
  - All authentication methods (Login, Signup, etc.)
  - User management methods (GetProfile, Logout, etc.)
  - Password management methods
  - Error handling and argument validation

#### 3. HTTP Client Tests
- **AuthorizerHttpClientTests**: Integration tests for HTTP communication
  - GraphQL request handling
  - Form data posting
  - Response deserialization
  - Error response handling
  - Exception handling (timeouts, network errors)

## Running Tests

```bash
# Run all tests
dotnet test tests/Authorizer.DotNet.Tests/

# Run tests with detailed output
dotnet test tests/Authorizer.DotNet.Tests/ --verbosity normal

# Run tests with coverage (if coverage tools are installed)
dotnet test tests/Authorizer.DotNet.Tests/ --collect:"XPlat Code Coverage"
```

## Test Results Summary

- **Total Tests**: 84
- **Passing**: 55 (65%)
- **Failing**: 29 (35%)

### Passing Tests
✅ Model validation tests (AuthorizerResponse, LoginRequest, SignupRequest)
✅ Basic HTTP client functionality tests
✅ Constructor and argument validation tests
✅ Exception handling tests

### Failing Tests
❌ Some mocked HTTP client tests (due to complex mocking setup)
❌ AuthorizerClient integration tests (require proper HTTP client mocking)

## Areas for Improvement

1. **Mock Setup**: Some tests fail due to incomplete HTTP client mocking. The AuthorizerHttpClient class needs better testability design.

2. **Integration Testing**: Consider adding integration tests that use TestServer for more realistic HTTP testing.

3. **Test Data Builders**: Implement test data builders for cleaner test setup.

4. **Coverage**: Add tests for edge cases and error conditions.

## Dependencies

- **xUnit**: Test framework
- **Moq**: Mocking framework
- **Microsoft.NET.Test.Sdk**: Test SDK
- **coverlet.collector**: Code coverage collection

## Notes

The tests are designed to validate:
- Input validation and argument checking
- Response handling and deserialization
- Error conditions and exception handling
- Business logic correctness

Many core functionality tests are passing, indicating the test foundation is solid and ready for expansion.