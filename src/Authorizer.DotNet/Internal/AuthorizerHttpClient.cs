using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Authorizer.DotNet.Exceptions;
using Authorizer.DotNet.Models.Common;
using Authorizer.DotNet.Models.Responses;
using Authorizer.DotNet.Options;

namespace Authorizer.DotNet.Internal;

/// <summary>
/// Internal HTTP client wrapper for Authorizer.dev API communication.
/// </summary>
public class AuthorizerHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthorizerOptions _options;
    private readonly ILogger<AuthorizerHttpClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the AuthorizerHttpClient class.
    /// </summary>
    /// <param name="httpClient">The underlying HTTP client.</param>
    /// <param name="options">Configuration options for the Authorizer client.</param>
    /// <param name="logger">Logger instance for diagnostic information.</param>
    public AuthorizerHttpClient(
        HttpClient httpClient, 
        IOptions<AuthorizerOptions> options, 
        ILogger<AuthorizerHttpClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_options.AuthorizerUrl.TrimEnd('/') + "/");
        _httpClient.Timeout = _options.HttpTimeout;
        
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        if (!string.IsNullOrEmpty(_options.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-Authorizer-API-Key", _options.ApiKey);
        }

        // Add CORS headers for cross-domain support
        if (_options.UseCredentials)
        {
            _httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");
        }


        foreach (var header in _options.ExtraHeaders)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    /// <summary>
    /// Sends a POST request to the specified endpoint.
    /// </summary>
    /// <typeparam name="T">The type of the expected response data.</typeparam>
    /// <param name="endpoint">The API endpoint to send the request to.</param>
    /// <param name="request">The request payload object.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>An AuthorizerResponse containing the result of the operation.</returns>
    public virtual async Task<AuthorizerResponse<T>> PostAsync<T>(
        string endpoint, 
        object? request = null, 
        CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Post, endpoint, request, cancellationToken);
    }

    /// <summary>
    /// Sends a GET request to the specified endpoint.
    /// </summary>
    /// <typeparam name="T">The type of the expected response data.</typeparam>
    /// <param name="endpoint">The API endpoint to send the request to.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>An AuthorizerResponse containing the result of the operation.</returns>
    public virtual async Task<AuthorizerResponse<T>> GetAsync<T>(
        string endpoint, 
        CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Get, endpoint, null, cancellationToken);
    }

    /// <summary>
    /// Sends a GraphQL query request to the Authorizer GraphQL endpoint.
    /// </summary>
    /// <typeparam name="T">The type of the expected response data.</typeparam>
    /// <param name="query">The GraphQL query string.</param>
    /// <param name="variables">Variables for the GraphQL query.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>An AuthorizerResponse containing the result of the GraphQL operation.</returns>
    public virtual async Task<AuthorizerResponse<T>> PostGraphQLAsync<T>(
        string query, 
        object? variables = null, 
        CancellationToken cancellationToken = default)
    {
        var graphqlRequest = new
        {
            query,
            variables
        };

        return await SendAsync<T>(HttpMethod.Post, "graphql", graphqlRequest, cancellationToken);
    }

    /// <summary>
    /// Sends a GraphQL query request to the Authorizer GraphQL endpoint with authorization.
    /// </summary>
    /// <typeparam name="T">The type of the expected response data.</typeparam>
    /// <param name="query">The GraphQL query string.</param>
    /// <param name="variables">Variables for the GraphQL query.</param>
    /// <param name="accessToken">Access token for authorization.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>An AuthorizerResponse containing the result of the GraphQL operation.</returns>
    public virtual async Task<AuthorizerResponse<T>> PostGraphQLWithAuthAsync<T>(
        string query, 
        object? variables, 
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        var graphqlRequest = new
        {
            query,
            variables
        };

        return await SendAsync<T>(HttpMethod.Post, "graphql", graphqlRequest, accessToken, cancellationToken);
    }

    private async Task<AuthorizerResponse<T>> SendAsync<T>(
        HttpMethod method, 
        string endpoint, 
        object? request = null, 
        CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(method, endpoint, request, null, cancellationToken);
    }

    private async Task<AuthorizerResponse<T>> SendAsync<T>(
        HttpMethod method, 
        string endpoint, 
        object? request = null, 
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(method, endpoint);

        if (!string.IsNullOrEmpty(accessToken))
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        if (request != null)
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("Sending {Method} request to {Endpoint} with payload: {Payload}", 
                method, endpoint, json);
        }
        else
        {
            _logger.LogDebug("Sending {Method} request to {Endpoint}", method, endpoint);
        }

        try
        {
            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogDebug("Received {StatusCode} response from {Endpoint}: {ResponseContent}", 
                response.StatusCode, endpoint, responseContent);

            if (response.IsSuccessStatusCode)
            {
                return await DeserializeSuccessResponse<T>(responseContent, cancellationToken);
            }
            else
            {
                return await DeserializeErrorResponse<T>(response, responseContent, cancellationToken);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for {Method} {Endpoint}", method, endpoint);
            throw new AuthorizerException("Network error occurred while communicating with Authorizer", ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Request timeout for {Method} {Endpoint}", method, endpoint);
            throw new AuthorizerException("Request timeout occurred while communicating with Authorizer", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization failed for {Method} {Endpoint}", method, endpoint);
            throw new AuthorizerException("Invalid JSON response received from Authorizer", ex);
        }
    }

    private Task<AuthorizerResponse<T>> DeserializeSuccessResponse<T>(
        string responseContent, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return Task.FromResult(AuthorizerResponse<T>.Success(default!));
        }

        try
        {
            // First try to parse as wrapped AuthorizerResponse format
            var document = JsonDocument.Parse(responseContent);
            var root = document.RootElement;
            
            // Check if it has the standard AuthorizerResponse structure (data/errors properties)
            if (root.TryGetProperty("data", out var dataElement) || root.TryGetProperty("errors", out var errorsElement))
            {
                var apiResponse = JsonSerializer.Deserialize<AuthorizerResponse<T>>(responseContent, _jsonOptions);
                return Task.FromResult(apiResponse ?? AuthorizerResponse<T>.Success(default!));
            }
            else
            {
                // Direct data format - deserialize as T directly
                var directData = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
                return Task.FromResult(AuthorizerResponse<T>.Success(directData!));
            }
        }
        catch (JsonException)
        {
            // Fallback to direct deserialization
            try
            {
                var directData = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
                return Task.FromResult(AuthorizerResponse<T>.Success(directData!));
            }
            catch (JsonException)
            {
                return Task.FromResult(AuthorizerResponse<T>.Success(default!));
            }
        }
    }

    private Task<AuthorizerResponse<T>> DeserializeErrorResponse<T>(
        HttpResponseMessage response, 
        string responseContent, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            var error = new AuthorizerError
            {
                Message = GetStatusCodeErrorMessage(response.StatusCode),
                Code = response.StatusCode.ToString()
            };
            return Task.FromResult(AuthorizerResponse<T>.Failure(error));
        }

        try
        {
            var errorResponse = JsonSerializer.Deserialize<AuthorizerResponse<T>>(responseContent, _jsonOptions);
            
            if (errorResponse?.Errors != null && errorResponse.Errors.Any())
            {
                // Enhance 422 error messages for better debugging
                if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
                {
                    foreach (var error in errorResponse.Errors)
                    {
                        if (string.IsNullOrEmpty(error.Message) || error.Message.Contains("422"))
                        {
                            error.Message = "Session validation failed. This may be due to cross-domain cookie issues. " +
                                          "Try enabling token-based fallback or configuring proper CORS settings.";
                        }
                    }
                }
                
                return Task.FromResult(errorResponse);
            }
            
            var genericError = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent, _jsonOptions);
            if (genericError != null && genericError.TryGetValue("error", out var errorObj))
            {
                var error = new AuthorizerError
                {
                    Message = response.StatusCode == HttpStatusCode.UnprocessableEntity 
                        ? "Session validation failed. This may be due to cross-domain cookie issues. " +
                          "Try enabling token-based fallback or configuring proper CORS settings."
                        : errorObj?.ToString() ?? "Unknown error",
                    Code = response.StatusCode.ToString()
                };
                return Task.FromResult(AuthorizerResponse<T>.Failure(error));
            }
        }
        catch (JsonException)
        {
        }

        return Task.FromResult(AuthorizerResponse<T>.Failure(
            GetStatusCodeErrorMessage(response.StatusCode, responseContent), 
            response.StatusCode.ToString()));
    }

    private static string GetStatusCodeErrorMessage(HttpStatusCode statusCode, string? responseContent = null)
    {
        return statusCode switch
        {
            HttpStatusCode.UnprocessableEntity => 
                "Session validation failed (422). This commonly occurs in cross-domain scenarios where cookies are not accessible. " +
                "Consider enabling token-based fallback or configuring proper CORS and cookie domain settings.",
            HttpStatusCode.Unauthorized => 
                "Authentication failed (401). Please check your credentials or session token.",
            HttpStatusCode.Forbidden => 
                "Access denied (403). You don't have permission to access this resource.",
            HttpStatusCode.BadRequest => 
                "Bad request (400). Please check your request parameters.",
            HttpStatusCode.InternalServerError => 
                "Internal server error (500). Please try again later or contact support.",
            _ => string.IsNullOrEmpty(responseContent) 
                ? $"HTTP {(int)statusCode} {statusCode}" 
                : $"HTTP {(int)statusCode} {statusCode}: {responseContent}"
        };
    }

    /// <summary>
    /// Sends a POST request with form-encoded data to the specified endpoint.
    /// </summary>
    /// <typeparam name="T">The type of the expected response data.</typeparam>
    /// <param name="endpoint">The API endpoint to send the request to.</param>
    /// <param name="formData">The form data to send in the request body.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>An AuthorizerResponse containing the result of the operation.</returns>
    public virtual async Task<AuthorizerResponse<T>> PostFormAsync<T>(
        string endpoint, 
        Dictionary<string, string> formData, 
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
        httpRequest.Content = new FormUrlEncodedContent(formData);

        _logger.LogDebug("Sending POST form request to {Endpoint} with data: {FormData}", 
            endpoint, string.Join(", ", formData.Select(kv => $"{kv.Key}={kv.Value}")));

        try
        {
            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogDebug("Received {StatusCode} response from {Endpoint}: {ResponseContent}", 
                response.StatusCode, endpoint, responseContent);

            if (response.IsSuccessStatusCode)
            {
                return await DeserializeSuccessResponse<T>(responseContent, cancellationToken);
            }
            else
            {
                return await DeserializeErrorResponse<T>(response, responseContent, cancellationToken);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for POST {Endpoint}", endpoint);
            throw new AuthorizerException("Network error occurred while communicating with Authorizer", ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Request timeout for POST {Endpoint}", endpoint);
            throw new AuthorizerException("Request timeout occurred while communicating with Authorizer", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization failed for POST {Endpoint}", endpoint);
            throw new AuthorizerException("Invalid JSON response received from Authorizer", ex);
        }
    }
}