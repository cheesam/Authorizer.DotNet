using System.Threading;
using System.Threading.Tasks;

namespace Authorizer.DotNet.Internal;

/// <summary>
/// Interface for storing and retrieving authentication tokens.
/// </summary>
public interface ITokenStorage
{
    /// <summary>
    /// Stores an access token.
    /// </summary>
    /// <param name="accessToken">The access token to store.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the stored access token.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The stored access token, or null if not found.</returns>
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to store.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the stored refresh token.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The stored refresh token, or null if not found.</returns>
    Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all stored tokens.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ClearTokensAsync(CancellationToken cancellationToken = default);
}