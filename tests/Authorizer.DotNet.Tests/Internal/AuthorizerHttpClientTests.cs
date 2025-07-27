using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;
using Authorizer.DotNet.Exceptions;
using Authorizer.DotNet.Internal;
using Authorizer.DotNet.Models.Common;
using Authorizer.DotNet.Models.Responses;
using Authorizer.DotNet.Options;

namespace Authorizer.DotNet.Tests.Internal;

/// <summary>
/// Unit tests for the AuthorizerHttpClient class.
/// </summary>
public class AuthorizerHttpClientTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<AuthorizerHttpClient>> _mockLogger;
    private readonly AuthorizerOptions _options;
    private readonly AuthorizerHttpClient _authorizerHttpClient;

    /// <summary>
    /// Initializes a new instance of the AuthorizerHttpClientTests class.
    /// </summary>
    public AuthorizerHttpClientTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _mockLogger = new Mock<ILogger<AuthorizerHttpClient>>();
        
        _options = new AuthorizerOptions
        {
            AuthorizerUrl = "https://test.authorizer.dev",
            RedirectUrl = "https://test.app/callback",
            ApiKey = "test_api_key",
            HttpTimeout = TimeSpan.FromSeconds(30)
        };

        var optionsMock = new Mock<IOptions<AuthorizerOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);

        _authorizerHttpClient = new AuthorizerHttpClient(_httpClient, optionsMock.Object, _mockLogger.Object);
    }

    #region Constructor Tests

    /// <summary>
    /// Tests that constructor throws ArgumentNullException when httpClient is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullHttpClient_ShouldThrowArgumentNullException()
    {
        var optionsMock = new Mock<IOptions<AuthorizerOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);

        Assert.Throws<ArgumentNullException>(() => 
            new AuthorizerHttpClient(null!, optionsMock.Object, _mockLogger.Object));
    }

    /// <summary>
    /// Tests that constructor throws ArgumentNullException when options is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new AuthorizerHttpClient(_httpClient, null!, _mockLogger.Object));
    }

    /// <summary>
    /// Tests that constructor throws ArgumentNullException when logger is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        var optionsMock = new Mock<IOptions<AuthorizerOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);

        Assert.Throws<ArgumentNullException>(() => 
            new AuthorizerHttpClient(_httpClient, optionsMock.Object, null!));
    }

    #endregion

    #region PostGraphQLAsync Tests

    /// <summary>
    /// Tests that PostGraphQLAsync returns data when HTTP response is successful.
    /// </summary>
    [Fact]
    public async Task PostGraphQLAsync_WithSuccessfulResponse_ShouldReturnData()
    {
        var responseContent = """{"data": {"test": "value"}}""";
        SetupHttpResponse(HttpStatusCode.OK, responseContent);

        var result = await _authorizerHttpClient.PostGraphQLAsync<Dictionary<string, string>>(
            "query { test }", 
            new { variable = "value" });

        Assert.True(result.IsSuccess);
        Assert.Equal("value", result.Data!["test"]);
    }

    /// <summary>
    /// Tests that PostGraphQLAsync returns errors when GraphQL response contains errors.
    /// </summary>
    [Fact]
    public async Task PostGraphQLAsync_WithErrorResponse_ShouldReturnErrors()
    {
        var responseContent = """{"errors": [{"message": "GraphQL error", "code": "GQL001"}]}""";
        SetupHttpResponse(HttpStatusCode.BadRequest, responseContent);

        var result = await _authorizerHttpClient.PostGraphQLAsync<Dictionary<string, string>>(
            "query { test }", 
            new { variable = "value" });

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors!);
        Assert.Equal("GraphQL error", result.Errors![0].Message);
        Assert.Equal("GQL001", result.Errors![0].Code);
    }

    /// <summary>
    /// Tests that PostGraphQLAsync returns error response when HTTP request fails.
    /// </summary>
    [Fact]
    public async Task PostGraphQLAsync_WithHttpError_ShouldReturnErrorResponse()
    {
        var responseContent = """{"error": "Unauthorized"}""";
        SetupHttpResponse(HttpStatusCode.Unauthorized, responseContent);

        var result = await _authorizerHttpClient.PostGraphQLAsync<Dictionary<string, string>>(
            "query { test }", 
            new { variable = "value" });

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors!);
        Assert.Equal("Unauthorized", result.Errors![0].Message);
        Assert.Equal("Unauthorized", result.Errors![0].Code);
    }

    #endregion

    #region PostFormAsync Tests

    /// <summary>
    /// Tests that PostFormAsync returns data when form submission is successful.
    /// </summary>
    [Fact]
    public async Task PostFormAsync_WithSuccessfulResponse_ShouldReturnData()
    {
        var responseContent = """{"access_token": "token123", "token_type": "Bearer"}""";
        SetupHttpResponse(HttpStatusCode.OK, responseContent);

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = "auth_code"
        };

        var result = await _authorizerHttpClient.PostFormAsync<Dictionary<string, string>>(
            "oauth/token", formData);

        Assert.True(result.IsSuccess);
        Assert.Equal("token123", result.Data!["access_token"]);
        Assert.Equal("Bearer", result.Data["token_type"]);
    }

    /// <summary>
    /// Tests that PostFormAsync returns error when form submission fails.
    /// </summary>
    [Fact]
    public async Task PostFormAsync_WithErrorResponse_ShouldReturnError()
    {
        var responseContent = """{"error": "invalid_grant"}""";
        SetupHttpResponse(HttpStatusCode.BadRequest, responseContent);

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = "invalid_code"
        };

        var result = await _authorizerHttpClient.PostFormAsync<Dictionary<string, string>>(
            "oauth/token", formData);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors!);
        Assert.Equal("invalid_grant", result.Errors![0].Message);
    }

    #endregion

    #region GetAsync Tests

    /// <summary>
    /// Tests that GetAsync returns data when HTTP GET request is successful.
    /// </summary>
    [Fact]
    public async Task GetAsync_WithSuccessfulResponse_ShouldReturnData()
    {
        var responseContent = """{"version": "1.0.0", "status": "active"}""";
        SetupHttpResponse(HttpStatusCode.OK, responseContent);

        var result = await _authorizerHttpClient.GetAsync<Dictionary<string, string>>("meta");

        Assert.True(result.IsSuccess);
        Assert.Equal("1.0.0", result.Data!["version"]);
        Assert.Equal("active", result.Data["status"]);
    }

    #endregion

    #region PostAsync Tests

    /// <summary>
    /// Tests that PostAsync returns data when HTTP POST request is successful.
    /// </summary>
    [Fact]
    public async Task PostAsync_WithSuccessfulResponse_ShouldReturnData()
    {
        var responseContent = """{"message": "Success"}""";
        SetupHttpResponse(HttpStatusCode.OK, responseContent);

        var requestData = new { email = "test@example.com" };

        var result = await _authorizerHttpClient.PostAsync<Dictionary<string, string>>(
            "verify-email", requestData);

        Assert.True(result.IsSuccess);
        Assert.Equal("Success", result.Data!["message"]);
    }

    #endregion

    #region Exception Handling Tests

    /// <summary>
    /// Tests that PostGraphQLAsync throws AuthorizerException when HttpRequestException occurs.
    /// </summary>
    [Fact]
    public async Task PostGraphQLAsync_WithHttpRequestException_ShouldThrowAuthorizerException()
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        await Assert.ThrowsAsync<AuthorizerException>(() =>
            _authorizerHttpClient.PostGraphQLAsync<string>("query { test }", null));
    }

    /// <summary>
    /// Tests that PostGraphQLAsync throws AuthorizerException when request times out.
    /// </summary>
    [Fact]
    public async Task PostGraphQLAsync_WithTimeout_ShouldThrowAuthorizerException()
    {
        var timeoutException = new TaskCanceledException("Request timeout", new TimeoutException());
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(timeoutException);

        await Assert.ThrowsAsync<AuthorizerException>(() =>
            _authorizerHttpClient.PostGraphQLAsync<string>("query { test }", null));
    }

    #endregion

    #region Response Handling Tests

    /// <summary>
    /// Tests that PostGraphQLAsync returns default value when successful response is empty.
    /// </summary>
    [Fact]
    public async Task PostGraphQLAsync_WithEmptySuccessResponse_ShouldReturnDefaultValue()
    {
        SetupHttpResponse(HttpStatusCode.OK, "");

        var result = await _authorizerHttpClient.PostGraphQLAsync<string>("query { test }", null);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Data);
    }

    /// <summary>
    /// Tests that PostGraphQLAsync returns HTTP status error when error response is empty.
    /// </summary>
    [Fact]
    public async Task PostGraphQLAsync_WithEmptyErrorResponse_ShouldReturnHttpStatusError()
    {
        SetupHttpResponse(HttpStatusCode.InternalServerError, "");

        var result = await _authorizerHttpClient.PostGraphQLAsync<string>("query { test }", null);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors!);
        Assert.Equal("HTTP 500 InternalServerError", result.Errors![0].Message);
        Assert.Equal("InternalServerError", result.Errors![0].Code);
    }

    /// <summary>
    /// Tests that PostGraphQLAsync returns data when response contains direct data without GraphQL wrapper.
    /// </summary>
    [Fact]
    public async Task PostGraphQLAsync_WithDirectDataResponse_ShouldReturnData()
    {
        var responseContent = """{"name": "John", "email": "john@example.com"}""";
        SetupHttpResponse(HttpStatusCode.OK, responseContent);

        var result = await _authorizerHttpClient.PostGraphQLAsync<Dictionary<string, string>>(
            "query { user }", null);

        Assert.True(result.IsSuccess);
        Assert.Equal("John", result.Data!["name"]);
        Assert.Equal("john@example.com", result.Data["email"]);
    }

    #endregion

    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    /// <summary>
    /// Disposes the HTTP client used in tests.
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}