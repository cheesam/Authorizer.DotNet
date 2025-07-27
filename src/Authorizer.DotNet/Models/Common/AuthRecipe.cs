using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Common;

/// <summary>
/// Authentication recipe types supported by Authorizer.dev.
/// </summary>
public enum AuthRecipe
{
    /// <summary>
    /// Basic email/password authentication.
    /// </summary>
    BasicAuth,
    
    /// <summary>
    /// Magic link login via email.
    /// </summary>
    MagicLinkLogin,
    
    /// <summary>
    /// Mobile OTP (One-Time Password) authentication.
    /// </summary>
    MobileOtp
}

/// <summary>
/// Configuration for authentication recipes.
/// </summary>
public class AuthRecipeConfig
{
    /// <summary>
    /// Whether basic auth (email/password) is enabled.
    /// </summary>
    [JsonPropertyName("basic_auth_enabled")]
    public bool BasicAuthEnabled { get; set; }

    /// <summary>
    /// Whether magic link login is enabled.
    /// </summary>
    [JsonPropertyName("magic_link_login_enabled")]
    public bool MagicLinkLoginEnabled { get; set; }

    /// <summary>
    /// Whether mobile OTP login is enabled.
    /// </summary>
    [JsonPropertyName("mobile_otp_enabled")]
    public bool MobileOtpEnabled { get; set; }

    /// <summary>
    /// Whether social login is enabled.
    /// </summary>
    [JsonPropertyName("social_login_enabled")]
    public bool SocialLoginEnabled { get; set; }

    /// <summary>
    /// List of enabled social login providers.
    /// </summary>
    [JsonPropertyName("social_login_providers")]
    public List<string>? SocialLoginProviders { get; set; }
}