using System.Text.Json.Serialization;
using Authorizer.DotNet.Models.Common;

namespace Authorizer.DotNet.Models.Responses;

/// <summary>
/// Response from the login API endpoint.
/// </summary>
public class LoginResponse
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
    public string? TokenType { get; set; }

    /// <summary>
    /// Access token expiration time in seconds.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public long? ExpiresIn { get; set; }

    /// <summary>
    /// User information returned with the login response.
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

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

    /// <summary>
    /// Timestamp when the tokens were issued.
    /// </summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; set; }

    /// <summary>
    /// Whether the login should be remembered (for persistent sessions).
    /// </summary>
    [JsonPropertyName("should_show_email_otp_screen")]
    public bool? ShouldShowEmailOtpScreen { get; set; }

    /// <summary>
    /// Whether MFA is required for this login.
    /// </summary>
    [JsonPropertyName("should_show_mobile_otp_screen")]
    public bool? ShouldShowMobileOtpScreen { get; set; }

    /// <summary>
    /// Session token for cookie-based authentication.
    /// </summary>
    [JsonPropertyName("session_token")]
    public string? SessionToken { get; set; }
}