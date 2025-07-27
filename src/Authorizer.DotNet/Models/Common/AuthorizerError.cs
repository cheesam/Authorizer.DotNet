using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Common;

/// <summary>
/// Represents an error returned by the Authorizer.dev API.
/// </summary>
public class AuthorizerError
{
    /// <summary>
    /// Error message describing what went wrong.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error code identifying the type of error.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// Additional details about the error.
    /// </summary>
    [JsonPropertyName("details")]
    public Dictionary<string, object>? Details { get; set; }

    /// <summary>
    /// Path in the GraphQL query where the error occurred.
    /// </summary>
    [JsonPropertyName("path")]
    public string[]? Path { get; set; }

    /// <summary>
    /// Extensions containing additional error information.
    /// </summary>
    [JsonPropertyName("extensions")]
    public Dictionary<string, object>? Extensions { get; set; }

    /// <summary>
    /// Returns a string representation of the error.
    /// </summary>
    /// <returns>A formatted string containing the error message and code.</returns>
    public override string ToString()
    {
        var codeText = !string.IsNullOrEmpty(Code) ? $" ({Code})" : string.Empty;
        return $"{Message}{codeText}";
    }
}