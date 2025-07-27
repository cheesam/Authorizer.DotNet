using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Requests;

/// <summary>
/// Request model for OAuth token exchange.
/// </summary>
public class GetTokenRequest
{
    /// <summary>
    /// OAuth grant type (authorization_code, refresh_token, client_credentials).
    /// </summary>
    [Required]
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = string.Empty;

    /// <summary>
    /// Authorization code from the authorize endpoint (required for authorization_code grant).
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// PKCE code verifier for enhanced security.
    /// </summary>
    [JsonPropertyName("code_verifier")]
    public string? CodeVerifier { get; set; }

    /// <summary>
    /// Refresh token (required for refresh_token grant).
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// OAuth client ID.
    /// </summary>
    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }

    /// <summary>
    /// OAuth client secret (for confidential clients).
    /// </summary>
    [JsonPropertyName("client_secret")]
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Redirect URI that was used in the authorization request.
    /// </summary>
    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; set; }

    /// <summary>
    /// OAuth scope for the token request.
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>
    /// Username (for password grant type).
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// Password (for password grant type).
    /// </summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}