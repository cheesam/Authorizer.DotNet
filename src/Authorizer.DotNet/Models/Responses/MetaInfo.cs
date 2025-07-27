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
    [JsonPropertyName("is_signup_enabled")]
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
    /// Whether mobile OTP is enabled.
    /// </summary>
    [JsonPropertyName("is_mobile_otp_enabled")]
    public bool IsMobileOtpEnabled { get; set; }

    /// <summary>
    /// Whether social login is enabled.
    /// </summary>
    [JsonPropertyName("is_social_login_enabled")]
    public bool IsSocialLoginEnabled { get; set; }

    /// <summary>
    /// List of enabled social login providers.
    /// </summary>
    [JsonPropertyName("social_login_providers")]
    public List<string>? SocialLoginProviders { get; set; }

    /// <summary>
    /// Whether strong password policy is enforced.
    /// </summary>
    [JsonPropertyName("is_strong_password_enabled")]
    public bool IsStrongPasswordEnabled { get; set; }

    /// <summary>
    /// Configuration for authentication recipes.
    /// </summary>
    [JsonPropertyName("auth_recipe_config")]
    public AuthRecipeConfig? AuthRecipeConfig { get; set; }

    /// <summary>
    /// List of available roles.
    /// </summary>
    [JsonPropertyName("roles")]
    public List<string>? Roles { get; set; }

    /// <summary>
    /// Default roles assigned to new users.
    /// </summary>
    [JsonPropertyName("default_roles")]
    public List<string>? DefaultRoles { get; set; }

    /// <summary>
    /// Protected routes that require authentication.
    /// </summary>
    [JsonPropertyName("protected_routes")]
    public List<string>? ProtectedRoutes { get; set; }

    /// <summary>
    /// Unprotected routes that don't require authentication.
    /// </summary>
    [JsonPropertyName("unprotected_routes")]
    public List<string>? UnprotectedRoutes { get; set; }

    /// <summary>
    /// Optional logout URL.
    /// </summary>
    [JsonPropertyName("logout_url")]
    public string? LogoutUrl { get; set; }

    /// <summary>
    /// Whether multi-factor authentication is enabled.
    /// </summary>
    [JsonPropertyName("is_multi_factor_auth_enabled")]
    public bool IsMultiFactorAuthEnabled { get; set; }
}