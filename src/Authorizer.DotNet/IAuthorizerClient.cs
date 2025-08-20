using System.Threading;
using System.Threading.Tasks;
using Authorizer.DotNet.Models.Requests;
using Authorizer.DotNet.Models.Responses;

namespace Authorizer.DotNet;

/// <summary>
/// Interface for the Authorizer.dev client providing authentication and user management functionality.
/// </summary>
public interface IAuthorizerClient
{
    #region Authentication Methods

    /// <summary>
    /// Authenticates a user using email and password.
    /// </summary>
    /// <param name="request">The login request containing email and password.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Login response containing access token and user information.</returns>
    Task<AuthorizerResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The signup request containing user information.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Signup response containing access token and user information.</returns>
    Task<AuthorizerResponse<SignupResponse>> SignupAsync(SignupRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates OAuth authorization flow.
    /// </summary>
    /// <param name="request">The authorization request with OAuth parameters.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Authorization response with redirect URL or authorization code.</returns>
    Task<AuthorizerResponse<AuthorizeResponse>> AuthorizeAsync(AuthorizeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exchanges authorization code for access token.
    /// </summary>
    /// <param name="request">The token request containing authorization code.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Token response containing access and refresh tokens.</returns>
    Task<AuthorizerResponse<TokenResponse>> GetTokenAsync(GetTokenRequest request, CancellationToken cancellationToken = default);

    #endregion

    #region User Management

    /// <summary>
    /// Retrieves the current user's profile information.
    /// </summary>
    /// <param name="accessToken">Access token for authentication.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>User profile information.</returns>
    Task<AuthorizerResponse<UserProfile>> GetProfileAsync(string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves current session information.
    /// </summary>
    /// <param name="sessionToken">Optional session token. If null, uses cookie-based session.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Session information including user and token details.</returns>
    Task<AuthorizerResponse<SessionInfo>> GetSessionAsync(string? sessionToken = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out the current user session.
    /// </summary>
    /// <param name="sessionToken">Optional session token. If null, uses cookie-based session.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Success status of the logout operation.</returns>
    Task<AuthorizerResponse<bool>> LogoutAsync(string? sessionToken = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user account. This operation is irreversible.
    /// </summary>
    /// <param name="request">The delete user request containing the email address.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Success status of the user deletion.</returns>
    Task<AuthorizerResponse<bool>> DeleteUserAsync(DeleteUserRequest request, CancellationToken cancellationToken = default);

    #endregion

    #region Verification and Validation

    /// <summary>
    /// Verifies a user's email address using verification token.
    /// </summary>
    /// <param name="request">The email verification request.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Success status of the email verification.</returns>
    Task<AuthorizerResponse<bool>> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a JWT token and returns user information.
    /// </summary>
    /// <param name="token">JWT token to validate.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>User profile information if token is valid.</returns>
    Task<AuthorizerResponse<UserProfile>> ValidateJwtAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a session using a provided access token instead of cookies.
    /// This method is useful for cross-domain scenarios or when you prefer token-based authentication.
    /// </summary>
    /// <param name="accessToken">The access token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A response containing session information if the token is valid</returns>
    Task<AuthorizerResponse<SessionInfo>> ValidateSessionWithTokenAsync(string accessToken, CancellationToken cancellationToken = default);

    #endregion

    #region Password Management

    /// <summary>
    /// Initiates password reset flow by sending reset email.
    /// </summary>
    /// <param name="email">Email address to send password reset to.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Success status of the password reset initiation.</returns>
    Task<AuthorizerResponse<bool>> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets user password using reset token.
    /// </summary>
    /// <param name="request">The password reset request containing token and new password.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Success status of the password reset.</returns>
    Task<AuthorizerResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes user password (requires current password).
    /// </summary>
    /// <param name="request">The password change request.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Success status of the password change.</returns>
    Task<AuthorizerResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);

    #endregion

    #region Metadata

    /// <summary>
    /// Retrieves metadata information about the Authorizer instance.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>Metadata information including supported features and configuration.</returns>
    Task<AuthorizerResponse<MetaInfo>> GetMetaAsync(CancellationToken cancellationToken = default);

    #endregion
}