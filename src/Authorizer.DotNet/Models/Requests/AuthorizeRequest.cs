using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Requests;

/// <summary>
/// Request model for OAuth authorization.
/// </summary>
public class AuthorizeRequest
{
    /// <summary>
    /// OAuth response type (typically "code" for authorization code flow).
    /// </summary>
    [Required]
    [JsonPropertyName("response_type")]
    public string ResponseType { get; set; } = "code";

    /// <summary>
    /// OAuth client ID.
    /// </summary>
    [Required]
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Redirect URI where the authorization response will be sent.
    /// </summary>
    [Required]
    [JsonPropertyName("redirect_uri")]
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// OAuth scope for the authorization request.
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>
    /// OAuth state parameter for CSRF protection.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// PKCE code challenge for enhanced security.
    /// </summary>
    [JsonPropertyName("code_challenge")]
    public string? CodeChallenge { get; set; }

    /// <summary>
    /// PKCE code challenge method (typically "S256").
    /// </summary>
    [JsonPropertyName("code_challenge_method")]
    public string? CodeChallengeMethod { get; set; }

    /// <summary>
    /// OAuth nonce for additional security.
    /// </summary>
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }

    /// <summary>
    /// OAuth response mode (query, fragment, form_post).
    /// </summary>
    [JsonPropertyName("response_mode")]
    public string? ResponseMode { get; set; }

    /// <summary>
    /// OAuth prompt parameter (none, login, consent, select_account).
    /// </summary>
    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    /// <summary>
    /// Maximum age of authentication in seconds.
    /// </summary>
    [JsonPropertyName("max_age")]
    public int? MaxAge { get; set; }

    /// <summary>
    /// UI locales for the authorization request.
    /// </summary>
    [JsonPropertyName("ui_locales")]
    public string? UiLocales { get; set; }

    /// <summary>
    /// Login hint (email or username) to pre-fill the login form.
    /// </summary>
    [JsonPropertyName("login_hint")]
    public string? LoginHint { get; set; }
}