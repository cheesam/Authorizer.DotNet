using System.ComponentModel.DataAnnotations;

namespace Authorizer.DotNet.Models.Requests;

/// <summary>
/// Request model for deleting a user account.
/// </summary>
public class DeleteUserRequest
{
    /// <summary>
    /// The email address of the user to delete.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}