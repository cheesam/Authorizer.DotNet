using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Authorizer.DotNet.Internal;
using Authorizer.DotNet.Models.Common;
using Authorizer.DotNet.Models.Requests;
using Authorizer.DotNet.Models.Responses;
using Authorizer.DotNet.Options;

namespace Authorizer.DotNet.Tests;

/// <summary>
/// Unit tests for the AuthorizerClient class.
/// </summary>
public class AuthorizerClientTests
{
    private readonly Mock<AuthorizerHttpClient> _mockHttpClient;
    private readonly Mock<ILogger<AuthorizerClient>> _mockLogger;
    private readonly Mock<ITokenStorage> _mockTokenStorage;
    private readonly AuthorizerOptions _options;
    private readonly AuthorizerClient _client;

    /// <summary>
    /// Initializes a new instance of the AuthorizerClientTests class.
    /// </summary>
    public AuthorizerClientTests()
    {
        _options = new AuthorizerOptions
        {
            AuthorizerUrl = "https://demo.authorizer.dev",
            RedirectUrl = "https://test.app/callback"
        };

        // Create a real HttpClient for the AuthorizerHttpClient constructor
        var httpClient = new System.Net.Http.HttpClient();
        var httpClientOptionsWithValue = new Mock<IOptions<AuthorizerOptions>>();
        httpClientOptionsWithValue.Setup(o => o.Value).Returns(_options);
        var httpClientLogger = new Mock<ILogger<AuthorizerHttpClient>>();

        _mockHttpClient = new Mock<AuthorizerHttpClient>(
            httpClient,
            httpClientOptionsWithValue.Object,
            httpClientLogger.Object)
        {
            CallBase = false // Important: don't call base methods, only use our setups
        };
        
        _mockLogger = new Mock<ILogger<AuthorizerClient>>();
        _mockTokenStorage = new Mock<ITokenStorage>();

        var optionsMock = new Mock<IOptions<AuthorizerOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);

        _client = new AuthorizerClient(_mockHttpClient.Object, optionsMock.Object, _mockLogger.Object, _mockTokenStorage.Object);
    }

    #region Constructor Tests

    /// <summary>
    /// Tests that the AuthorizerClient constructor throws ArgumentNullException when httpClient is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullHttpClient_ShouldThrowArgumentNullException()
    {
        var optionsMock = new Mock<IOptions<AuthorizerOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);

        Assert.Throws<ArgumentNullException>(() => 
            new AuthorizerClient(null!, optionsMock.Object, _mockLogger.Object, _mockTokenStorage.Object));
    }

    /// <summary>
    /// Tests that the AuthorizerClient constructor throws ArgumentNullException when options is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new AuthorizerClient(_mockHttpClient.Object, null!, _mockLogger.Object, _mockTokenStorage.Object));
    }

    /// <summary>
    /// Tests that the AuthorizerClient constructor throws ArgumentNullException when logger is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        var optionsMock = new Mock<IOptions<AuthorizerOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);

        Assert.Throws<ArgumentNullException>(() => 
            new AuthorizerClient(_mockHttpClient.Object, optionsMock.Object, null!, _mockTokenStorage.Object));
    }

    /// <summary>
    /// Tests that the AuthorizerClient constructor throws ArgumentNullException when tokenStorage is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullTokenStorage_ShouldThrowArgumentNullException()
    {
        var optionsMock = new Mock<IOptions<AuthorizerOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);

        Assert.Throws<ArgumentNullException>(() => 
            new AuthorizerClient(_mockHttpClient.Object, optionsMock.Object, _mockLogger.Object, null!));
    }

    #endregion

    #region LoginAsync Tests

    /// <summary>
    /// Tests that LoginAsync returns a successful response when given valid credentials.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithValidRequest_ShouldReturnSuccessResponse()
    {
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var expectedResponse = AuthorizerResponse<LoginResponse>.Success(
            new LoginResponse
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token",
                User = new User { Id = "user_id", Email = "test@example.com" }
            });

        _mockHttpClient
            .Setup(x => x.PostGraphQLAsync<LoginResponse>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.LoginAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("access_token", result.Data!.AccessToken);
        Assert.Equal("test@example.com", result.Data.User!.Email);
    }

    /// <summary>
    /// Tests that LoginAsync throws ArgumentNullException when request is null.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _client.LoginAsync(null!));
    }

    /// <summary>
    /// Tests that LoginAsync returns an error response when given invalid credentials.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WithFailedResponse_ShouldReturnErrorResponse()
    {
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrong_password"
        };

        var expectedResponse = AuthorizerResponse<LoginResponse>.Failure("Invalid credentials");

        _mockHttpClient
            .Setup(x => x.PostGraphQLAsync<LoginResponse>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.LoginAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid credentials", result.FirstErrorMessage);
    }

    #endregion

    #region SignupAsync Tests

    /// <summary>
    /// Tests that SignupAsync returns a successful response when given valid data.
    /// </summary>
    [Fact]
    public async Task SignupAsync_WithValidRequest_ShouldReturnSuccessResponse()
    {
        var request = new SignupRequest
        {
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        var expectedResponse = AuthorizerResponse<SignupResponse>.Success(
            new SignupResponse
            {
                AccessToken = "access_token",
                User = new User { Id = "user_id", Email = "test@example.com" }
            });

        _mockHttpClient
            .Setup(x => x.PostGraphQLAsync<SignupResponse>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.SignupAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("access_token", result.Data!.AccessToken);
        Assert.Equal("test@example.com", result.Data.User!.Email);
    }

    /// <summary>
    /// Tests that SignupAsync throws ArgumentNullException when request is null.
    /// </summary>
    [Fact]
    public async Task SignupAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _client.SignupAsync(null!));
    }

    #endregion

    #region GetProfileAsync Tests

    /// <summary>
    /// Tests that GetProfileAsync returns user profile when given a valid access token.
    /// </summary>
    [Fact]
    public async Task GetProfileAsync_WithValidToken_ShouldReturnUserProfile()
    {
        var accessToken = "valid_access_token";
        var expectedProfile = new UserProfile
        {
            Id = "user_id",
            Email = "test@example.com",
            GivenName = "Test",
            FamilyName = "User"
        };

        var expectedResponse = AuthorizerResponse<UserProfile>.Success(expectedProfile);

        _mockHttpClient
            .Setup(x => x.PostGraphQLWithAuthAsync<UserProfile>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.GetProfileAsync(accessToken);

        Assert.True(result.IsSuccess);
        Assert.Equal("user_id", result.Data!.Id);
        Assert.Equal("test@example.com", result.Data.Email);
    }

    /// <summary>
    /// Tests that GetProfileAsync throws ArgumentException when given null, empty, or whitespace token.
    /// </summary>
    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetProfileAsync_WithInvalidToken_ShouldThrowArgumentException(string? token)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _client.GetProfileAsync(token!));
    }

    #endregion

    #region LogoutAsync Tests

    /// <summary>
    /// Tests that LogoutAsync returns success when given a valid session token.
    /// </summary>
    [Fact]
    public async Task LogoutAsync_WithValidSession_ShouldReturnSuccess()
    {
        var sessionToken = "session_token";
        var expectedResponse = AuthorizerResponse<System.Collections.Generic.Dictionary<string, string>>.Success(
            new System.Collections.Generic.Dictionary<string, string> { { "message", "Logged out successfully" } });

        _mockHttpClient
            .Setup(x => x.PostGraphQLAsync<System.Collections.Generic.Dictionary<string, string>>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.LogoutAsync(sessionToken);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
    }

    /// <summary>
    /// Tests that LogoutAsync works when called with null session token.
    /// </summary>
    [Fact]
    public async Task LogoutAsync_WithNullSessionToken_ShouldStillWork()
    {
        var expectedResponse = AuthorizerResponse<System.Collections.Generic.Dictionary<string, string>>.Success(
            new System.Collections.Generic.Dictionary<string, string> { { "message", "Logged out successfully" } });

        _mockHttpClient
            .Setup(x => x.PostGraphQLAsync<System.Collections.Generic.Dictionary<string, string>>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.LogoutAsync();

        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
    }

    #endregion

    #region VerifyEmailAsync Tests

    /// <summary>
    /// Tests that VerifyEmailAsync returns success when given a valid verification request.
    /// </summary>
    [Fact]
    public async Task VerifyEmailAsync_WithValidRequest_ShouldReturnSuccess()
    {
        var request = new VerifyEmailRequest
        {
            Email = "test@example.com",
            Token = "verification_token"
        };

        var expectedResponse = AuthorizerResponse<System.Collections.Generic.Dictionary<string, string>>.Success(
            new System.Collections.Generic.Dictionary<string, string> { { "message", "Email verified successfully" } });

        _mockHttpClient
            .Setup(x => x.PostGraphQLAsync<System.Collections.Generic.Dictionary<string, string>>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.VerifyEmailAsync(request);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
    }

    /// <summary>
    /// Tests that VerifyEmailAsync throws ArgumentNullException when request is null.
    /// </summary>
    [Fact]
    public async Task VerifyEmailAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _client.VerifyEmailAsync(null!));
    }

    #endregion

    #region ValidateJwtAsync Tests

    /// <summary>
    /// Tests that ValidateJwtAsync returns user profile when given a valid JWT token.
    /// </summary>
    [Fact]
    public async Task ValidateJwtAsync_WithValidToken_ShouldReturnUserProfile()
    {
        var token = "valid_jwt_token";
        var expectedClaims = new UserProfile
        {
            Id = "user_id",
            Email = "test@example.com"
        };

        var responseDict = new System.Collections.Generic.Dictionary<string, object>
        {
            ["is_valid"] = true,
            ["claims"] = System.Text.Json.JsonSerializer.Serialize(expectedClaims)
        };

        var expectedResponse = AuthorizerResponse<System.Collections.Generic.Dictionary<string, object>>.Success(responseDict);

        _mockHttpClient
            .Setup(x => x.PostGraphQLAsync<System.Collections.Generic.Dictionary<string, object>>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.ValidateJwtAsync(token);

        Assert.True(result.IsSuccess);
        Assert.Equal("user_id", result.Data!.Id);
        Assert.Equal("test@example.com", result.Data.Email);
    }

    /// <summary>
    /// Tests that ValidateJwtAsync throws ArgumentException when given null, empty, or whitespace token.
    /// </summary>
    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ValidateJwtAsync_WithInvalidToken_ShouldThrowArgumentException(string? token)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _client.ValidateJwtAsync(token!));
    }

    #endregion

    #region ForgotPasswordAsync Tests

    /// <summary>
    /// Tests that ForgotPasswordAsync returns success when given a valid email address.
    /// </summary>
    [Fact]
    public async Task ForgotPasswordAsync_WithValidEmail_ShouldReturnSuccess()
    {
        var email = "test@example.com";
        var expectedResponse = AuthorizerResponse<System.Collections.Generic.Dictionary<string, string>>.Success(
            new System.Collections.Generic.Dictionary<string, string> { { "message", "Password reset email sent" } });

        _mockHttpClient
            .Setup(x => x.PostGraphQLAsync<System.Collections.Generic.Dictionary<string, string>>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.ForgotPasswordAsync(email);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
    }

    /// <summary>
    /// Tests that ForgotPasswordAsync throws ArgumentException when given null, empty, or whitespace email.
    /// </summary>
    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ForgotPasswordAsync_WithInvalidEmail_ShouldThrowArgumentException(string? email)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _client.ForgotPasswordAsync(email!));
    }

    #endregion

    #region GetMetaAsync Tests

    /// <summary>
    /// Tests that GetMetaAsync returns meta information about the Authorizer instance.
    /// </summary>
    [Fact]
    public async Task GetMetaAsync_ShouldReturnMetaInfo()
    {
        var expectedMeta = new MetaInfo
        {
            Version = "1.0.0",
            ClientId = "test_client_id",
            IsSignupEnabled = true
        };

        var expectedResponse = AuthorizerResponse<MetaInfo>.Success(expectedMeta);

        _mockHttpClient
            .Setup(x => x.PostGraphQLAsync<MetaInfo>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.GetMetaAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal("1.0.0", result.Data!.Version);
        Assert.Equal("test_client_id", result.Data.ClientId);
        Assert.True(result.Data.IsSignupEnabled);
    }

    #endregion

    #region ValidateSessionWithTokenAsync Tests

    /// <summary>
    /// Tests that ValidateSessionWithTokenAsync returns session info when given a valid access token.
    /// </summary>
    [Fact]
    public async Task ValidateSessionWithTokenAsync_WithValidToken_ShouldReturnSessionInfo()
    {
        var accessToken = "valid_access_token";
        var expectedUser = new UserProfile
        {
            Id = "user_id",
            Email = "test@example.com",
            GivenName = "Test",
            FamilyName = "User"
        };

        var expectedResponse = AuthorizerResponse<UserProfile>.Success(expectedUser);

        _mockHttpClient
            .Setup(x => x.PostGraphQLWithAuthAsync<UserProfile>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.Is<string>(token => token == accessToken),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.ValidateSessionWithTokenAsync(accessToken);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(accessToken, result.Data!.AccessToken);
        Assert.NotNull(result.Data.User);
        Assert.Equal("user_id", result.Data.User!.Id);
        Assert.Equal("test@example.com", result.Data.User.Email);
    }

    /// <summary>
    /// Tests that ValidateSessionWithTokenAsync returns failure when access token is invalid.
    /// </summary>
    [Fact]
    public async Task ValidateSessionWithTokenAsync_WithInvalidToken_ShouldReturnFailure()
    {
        var accessToken = "invalid_access_token";
        var expectedResponse = AuthorizerResponse<UserProfile>.Failure(
            new[] { new AuthorizerError { Message = "Invalid access token" } });

        _mockHttpClient
            .Setup(x => x.PostGraphQLWithAuthAsync<UserProfile>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.Is<string>(token => token == accessToken),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _client.ValidateSessionWithTokenAsync(accessToken);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors, e => e.Message.Contains("Invalid access token"));
    }

    /// <summary>
    /// Tests that ValidateSessionWithTokenAsync returns failure with appropriate error when given null or empty token.
    /// </summary>
    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ValidateSessionWithTokenAsync_WithInvalidToken_ShouldReturnFailureWithError(string? accessToken)
    {
        var result = await _client.ValidateSessionWithTokenAsync(accessToken!);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Contains(result.Errors, e => e.Message == "Access token is required for token-based session validation.");
    }

    /// <summary>
    /// Tests that ValidateSessionWithTokenAsync handles HTTP client exceptions gracefully.
    /// </summary>
    [Fact]
    public async Task ValidateSessionWithTokenAsync_WithHttpException_ShouldReturnFailure()
    {
        var accessToken = "valid_access_token";

        _mockHttpClient
            .Setup(x => x.PostGraphQLWithAuthAsync<UserProfile>(
                It.IsAny<string>(), 
                It.IsAny<object>(), 
                It.Is<string>(token => token == accessToken),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Net.Http.HttpRequestException("Network error"));

        // Should not throw, but return failure response
        await Assert.ThrowsAsync<System.Net.Http.HttpRequestException>(() => 
            _client.ValidateSessionWithTokenAsync(accessToken));
    }

    #endregion
}