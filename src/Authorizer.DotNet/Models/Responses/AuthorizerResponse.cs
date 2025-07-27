using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Authorizer.DotNet.Models.Common;

namespace Authorizer.DotNet.Models.Responses;

/// <summary>
/// Generic wrapper for all Authorizer.dev API responses.
/// </summary>
/// <typeparam name="T">The type of data contained in the response.</typeparam>
public class AuthorizerResponse<T>
{
    /// <summary>
    /// The response data if the request was successful.
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    /// <summary>
    /// List of errors if the request failed.
    /// </summary>
    [JsonPropertyName("errors")]
    public IReadOnlyList<AuthorizerError>? Errors { get; set; }

    /// <summary>
    /// Indicates whether the request was successful (no errors).
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => Errors == null || !Errors.Any();

    /// <summary>
    /// Indicates whether the request has errors.
    /// </summary>
    [JsonIgnore]
    public bool HasErrors => !IsSuccess;

    /// <summary>
    /// Gets the first error message if any errors exist.
    /// </summary>
    [JsonIgnore]
    public string? FirstErrorMessage => Errors?.FirstOrDefault()?.Message;

    /// <summary>
    /// Creates a successful response with data.
    /// </summary>
    /// <param name="data">The successful response data.</param>
    /// <returns>A successful AuthorizerResponse.</returns>
    public static AuthorizerResponse<T> Success(T data)
    {
        return new AuthorizerResponse<T> { Data = data };
    }

    /// <summary>
    /// Creates a failed response with errors.
    /// </summary>
    /// <param name="errors">The errors that occurred.</param>
    /// <returns>A failed AuthorizerResponse.</returns>
    public static AuthorizerResponse<T> Failure(IReadOnlyList<AuthorizerError> errors)
    {
        return new AuthorizerResponse<T> { Errors = errors };
    }

    /// <summary>
    /// Creates a failed response with a single error.
    /// </summary>
    /// <param name="error">The error that occurred.</param>
    /// <returns>A failed AuthorizerResponse.</returns>
    public static AuthorizerResponse<T> Failure(AuthorizerError error)
    {
        return new AuthorizerResponse<T> { Errors = new[] { error } };
    }

    /// <summary>
    /// Creates a failed response with an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="code">Optional error code.</param>
    /// <returns>A failed AuthorizerResponse.</returns>
    public static AuthorizerResponse<T> Failure(string message, string? code = null)
    {
        var error = new AuthorizerError { Message = message, Code = code };
        return new AuthorizerResponse<T> { Errors = new[] { error } };
    }
}