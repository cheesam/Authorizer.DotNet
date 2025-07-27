using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Responses;

/// <summary>
/// Response from the OAuth authorize endpoint.
/// </summary>
public class AuthorizeResponse
{
    /// <summary>
    /// Authorization code for token exchange.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// OAuth state parameter.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// Redirect URL where the user should be sent.
    /// </summary>
    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; set; }

    /// <summary>
    /// Error code if authorization failed.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// Error description if authorization failed.
    /// </summary>
    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; set; }

    /// <summary>
    /// Error URI with more information about the error.
    /// </summary>
    [JsonPropertyName("error_uri")]
    public string? ErrorUri { get; set; }
}