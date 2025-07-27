using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Authorizer.DotNet.Models.Common;

namespace Authorizer.DotNet.Exceptions;

/// <summary>
/// Exception thrown when an Authorizer.dev API operation fails.
/// </summary>
public class AuthorizerException : Exception
{
    /// <summary>
    /// HTTP status code from the API response.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// List of errors returned by the API.
    /// </summary>
    public IReadOnlyList<AuthorizerError>? Errors { get; }

    /// <summary>
    /// Raw response content from the API.
    /// </summary>
    public string? ResponseContent { get; }

    /// <summary>
    /// Initializes a new instance of the AuthorizerException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public AuthorizerException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AuthorizerException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AuthorizerException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AuthorizerException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">HTTP status code from the API response.</param>
    /// <param name="responseContent">Raw response content from the API.</param>
    public AuthorizerException(string message, HttpStatusCode statusCode, string? responseContent = null) 
        : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }

    /// <summary>
    /// Initializes a new instance of the AuthorizerException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">List of errors returned by the API.</param>
    /// <param name="statusCode">HTTP status code from the API response.</param>
    /// <param name="responseContent">Raw response content from the API.</param>
    public AuthorizerException(
        string message, 
        IReadOnlyList<AuthorizerError> errors, 
        HttpStatusCode? statusCode = null, 
        string? responseContent = null) 
        : base(message)
    {
        Errors = errors;
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }

    /// <summary>
    /// Creates an AuthorizerException from a list of API errors.
    /// </summary>
    /// <param name="errors">List of errors returned by the API.</param>
    /// <param name="statusCode">HTTP status code from the API response.</param>
    /// <param name="responseContent">Raw response content from the API.</param>
    /// <returns>A new AuthorizerException instance.</returns>
    public static AuthorizerException FromErrors(
        IReadOnlyList<AuthorizerError> errors, 
        HttpStatusCode? statusCode = null, 
        string? responseContent = null)
    {
        var message = errors?.FirstOrDefault()?.Message ?? "Unknown error occurred";
        return new AuthorizerException(message, errors ?? Array.Empty<AuthorizerError>(), statusCode, responseContent);
    }

    /// <summary>
    /// Creates an AuthorizerException from an HTTP response.
    /// </summary>
    /// <param name="statusCode">HTTP status code.</param>
    /// <param name="responseContent">Raw response content.</param>
    /// <returns>A new AuthorizerException instance.</returns>
    public static AuthorizerException FromHttpResponse(HttpStatusCode statusCode, string? responseContent = null)
    {
        var message = $"HTTP {(int)statusCode} {statusCode}";
        return new AuthorizerException(message, statusCode, responseContent);
    }
}