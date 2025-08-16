using System.Collections.Generic;
using System.Text.Json.Serialization;
using Authorizer.DotNet.Models.Common;

namespace Authorizer.DotNet.Models.Responses;

/// <summary>
/// Metadata information about the Authorizer instance.
/// </summary>
public class MetaInfo
{
    /// <summary>
    /// Version of the Authorizer instance.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    /// Client ID for OAuth flows.
    /// </summary>
    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }

    /// <summary>
    /// Whether signup is enabled.
    /// </summary>
    [JsonPropertyName("is_sign_up_enabled")]
    public bool IsSignupEnabled { get; set; }

    /// <summary>
    /// Whether email verification is required.
    /// </summary>
    [JsonPropertyName("is_email_verification_enabled")]
    public bool IsEmailVerificationEnabled { get; set; }

    /// <summary>
    /// Whether basic authentication is enabled.
    /// </summary>
    [JsonPropertyName("is_basic_authentication_enabled")]
    public bool IsBasicAuthenticationEnabled { get; set; }

    /// <summary>
    /// Whether magic link login is enabled.
    /// </summary>
    [JsonPropertyName("is_magic_link_login_enabled")]
    public bool IsMagicLinkLoginEnabled { get; set; }

    /// <summary>
    /// Whether Google login is enabled.
    /// </summary>
    [JsonPropertyName("is_google_login_enabled")]
    public bool IsGoogleLoginEnabled { get; set; }

    /// <summary>
    /// Whether GitHub login is enabled.
    /// </summary>
    [JsonPropertyName("is_github_login_enabled")]
    public bool IsGithubLoginEnabled { get; set; }

    /// <summary>
    /// Whether Facebook login is enabled.
    /// </summary>
    [JsonPropertyName("is_facebook_login_enabled")]
    public bool IsFacebookLoginEnabled { get; set; }

    /// <summary>
    /// Whether Apple login is enabled.
    /// </summary>
    [JsonPropertyName("is_apple_login_enabled")]
    public bool IsAppleLoginEnabled { get; set; }

    /// <summary>
    /// Whether Discord login is enabled.
    /// </summary>
    [JsonPropertyName("is_discord_login_enabled")]
    public bool IsDiscordLoginEnabled { get; set; }

    /// <summary>
    /// Whether Roblox login is enabled.
    /// </summary>
    [JsonPropertyName("is_roblox_login_enabled")]
    public bool IsRobloxLoginEnabled { get; set; }

    /// <summary>
    /// Whether strong password policy is enforced.
    /// </summary>
    [JsonPropertyName("is_strong_password_enabled")]
    public bool IsStrongPasswordEnabled { get; set; }


    /// <summary>
    /// Whether multi-factor authentication is enabled.
    /// </summary>
    [JsonPropertyName("is_multi_factor_auth_enabled")]
    public bool IsMultiFactorAuthEnabled { get; set; }
}