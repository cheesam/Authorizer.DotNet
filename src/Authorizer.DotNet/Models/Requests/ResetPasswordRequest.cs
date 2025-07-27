using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Requests;

/// <summary>
/// Request model for password reset.
/// </summary>
public class ResetPasswordRequest
{
    /// <summary>
    /// Password reset token received via email.
    /// </summary>
    [Required]
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// New password for the user.
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
}