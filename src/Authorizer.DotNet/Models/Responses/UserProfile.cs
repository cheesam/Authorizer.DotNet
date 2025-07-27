using System.Collections.Generic;
using System.Text.Json.Serialization;
using Authorizer.DotNet.Models.Common;

namespace Authorizer.DotNet.Models.Responses;

/// <summary>
/// User profile information response.
/// </summary>
public class UserProfile : User
{
    /// <summary>
    /// Whether the user has multi-factor authentication enabled.
    /// </summary>
    [JsonPropertyName("has_mobile_otp")]
    public bool HasMobileOtp { get; set; }

    /// <summary>
    /// User's backup codes for account recovery.
    /// </summary>
    [JsonPropertyName("backup_codes")]
    public List<string>? BackupCodes { get; set; }

    /// <summary>
    /// User's enrolled authenticator devices.
    /// </summary>
    [JsonPropertyName("authenticator_devices")]
    public List<Dictionary<string, object>>? AuthenticatorDevices { get; set; }
}