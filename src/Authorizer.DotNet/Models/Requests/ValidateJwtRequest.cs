using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Authorizer.DotNet.Models.Requests;

/// <summary>
/// Request model for JWT token validation.
/// </summary>
public class ValidateJwtRequest
{
    /// <summary>
    /// JWT token to validate.
    /// </summary>
    [Required]
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token type (typically "Bearer").
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "Bearer";
}