using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Requests;

/// <summary>
/// Request model for user login.
/// </summary>
public class LoginRequest
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
    /// OAuth scope for the login request.
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>
    /// OAuth state parameter for CSRF protection.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// Roles to assign to the user session.
    /// </summary>
    [JsonPropertyName("roles")]
    public List<string>? Roles { get; set; }

    /// <summary>
    /// Whether to remember the login session.
    /// </summary>
    [JsonPropertyName("is_remember_me")]
    public bool IsRememberMe { get; set; }
}