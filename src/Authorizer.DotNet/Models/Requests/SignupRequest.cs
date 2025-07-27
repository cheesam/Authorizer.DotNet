using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Requests;

/// <summary>
/// Request model for user registration.
/// </summary>
public class SignupRequest
{
    /// <summary>
    /// User's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Password confirmation (must match password).
    /// </summary>
    [Required]
    [JsonPropertyName("confirm_password")]
    public string ConfirmPassword { get; set; } = string.Empty;

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
    /// User's phone number.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's profile picture URL.
    /// </summary>
    [JsonPropertyName("picture")]
    public string? Picture { get; set; }

    /// <summary>
    /// User's birthdate.
    /// </summary>
    [JsonPropertyName("birthdate")]
    public string? Birthdate { get; set; }

    /// <summary>
    /// User's gender.
    /// </summary>
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    /// <summary>
    /// OAuth scope for the signup request.
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>
    /// OAuth state parameter for CSRF protection.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// Roles to assign to the new user.
    /// </summary>
    [JsonPropertyName("roles")]
    public List<string>? Roles { get; set; }

    /// <summary>
    /// Additional app-specific data for the user.
    /// </summary>
    [JsonPropertyName("app_data")]
    public Dictionary<string, object>? AppData { get; set; }

    /// <summary>
    /// Redirect URL after successful signup.
    /// </summary>
    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; set; }
}