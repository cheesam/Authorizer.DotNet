using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Responses;

/// <summary>
/// Response from the OAuth token endpoint.
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// JWT access token for API authentication.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// Refresh token for obtaining new access tokens.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// ID token containing user claims.
    /// </summary>
    [JsonPropertyName("id_token")]
    public string? IdToken { get; set; }

    /// <summary>
    /// Token type (typically "Bearer").
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Access token expiration time in seconds.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public long? ExpiresIn { get; set; }

    /// <summary>
    /// OAuth scope granted to the access token.
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>
    /// OAuth state parameter.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

}