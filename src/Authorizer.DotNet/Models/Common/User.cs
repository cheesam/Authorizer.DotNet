using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Common;

/// <summary>
/// Represents a user in the Authorizer.dev system.
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User's email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Whether the user's email has been verified.
    /// </summary>
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }

    /// <summary>
    /// User's given name (first name).
    /// </summary>
    [JsonPropertyName("given_name")]
    public string? GivenName { get; set; }

    /// <summary>
    /// User's family name (last name).
    /// </summary>
    [JsonPropertyName("family_name")]
    public string? FamilyName { get; set; }

    /// <summary>
    /// User's middle name.
    /// </summary>
    [JsonPropertyName("middle_name")]
    public string? MiddleName { get; set; }

    /// <summary>
    /// User's full name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// User's nickname or display name.
    /// </summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>
    /// User's preferred username.
    /// </summary>
    [JsonPropertyName("preferred_username")]
    public string? PreferredUsername { get; set; }

    /// <summary>
    /// URL to the user's profile picture.
    /// </summary>
    [JsonPropertyName("picture")]
    public string? Picture { get; set; }

    /// <summary>
    /// User's website URL.
    /// </summary>
    [JsonPropertyName("website")]
    public string? Website { get; set; }

    /// <summary>
    /// User's gender.
    /// </summary>
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    /// <summary>
    /// User's birthdate.
    /// </summary>
    [JsonPropertyName("birthdate")]
    public string? Birthdate { get; set; }

    /// <summary>
    /// User's phone number.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Whether the user's phone number has been verified.
    /// </summary>
    [JsonPropertyName("phone_number_verified")]
    public bool PhoneNumberVerified { get; set; }

    /// <summary>
    /// Timestamp when the user was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the user was last updated.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public long UpdatedAt { get; set; }

    /// <summary>
    /// List of roles assigned to the user.
    /// </summary>
    [JsonPropertyName("roles")]
    public List<string>? Roles { get; set; }

    /// <summary>
    /// Additional custom attributes for the user.
    /// </summary>
    [JsonPropertyName("app_data")]
    public Dictionary<string, object>? AppData { get; set; }

    /// <summary>
    /// Whether the user account is active.
    /// </summary>
    [JsonPropertyName("is_multi_factor_auth_enabled")]
    public bool IsMultiFactorAuthEnabled { get; set; }

    /// <summary>
    /// Timestamp when user access was revoked.
    /// </summary>
    [JsonPropertyName("revoked_timestamp")]
    public long? RevokedTimestamp { get; set; }

    /// <summary>
    /// Reason for signup (if provided during registration).
    /// </summary>
    [JsonPropertyName("signup_methods")]
    public string? SignupMethods { get; set; }
}