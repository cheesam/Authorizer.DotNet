using System.Text.Json.Serialization;
using Authorizer.DotNet.Models.Common;

namespace Authorizer.DotNet.Models.Responses;

/// <summary>
/// Session information response.
/// </summary>
public class SessionInfo
{
    /// <summary>
    /// JWT access token for the session.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// Refresh token for the session.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// ID token for the session.
    /// </summary>
    [JsonPropertyName("id_token")]
    public string? IdToken { get; set; }

    /// <summary>
    /// User information for the session.
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Session expiration timestamp.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public long? ExpiresAt { get; set; }

    /// <summary>
    /// When the session was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; set; }

    /// <summary>
    /// Session token for cookie-based authentication.
    /// </summary>
    [JsonPropertyName("session_token")]
    public string? SessionToken { get; set; }

    /// <summary>
    /// Whether the session is still valid.
    /// </summary>
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }
}