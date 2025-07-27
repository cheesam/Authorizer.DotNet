using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Requests;

/// <summary>
/// Request model for changing user password.
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// User's current password.
    /// </summary>
    [Required]
    [JsonPropertyName("old_password")]
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password for the user.
    /// </summary>
    [Required]
    [JsonPropertyName("new_password")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password confirmation (must match new_password).
    /// </summary>
    [Required]
    [JsonPropertyName("confirm_new_password")]
    public string ConfirmNewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Access token for authentication.
    /// </summary>
    [Required]
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}