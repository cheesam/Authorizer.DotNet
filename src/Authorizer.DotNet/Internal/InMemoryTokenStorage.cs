using System.Threading;
using System.Threading.Tasks;

namespace Authorizer.DotNet.Internal;

/// <summary>
/// Simple in-memory token storage implementation.
/// Note: Tokens are lost when the application restarts.
/// </summary>
public class InMemoryTokenStorage : ITokenStorage
{
    private string? _accessToken;
    private string? _refreshToken;

    /// <inheritdoc />
    public Task SetAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        _accessToken = accessToken;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_accessToken);
    }

    /// <inheritdoc />
    public Task SetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        _refreshToken = refreshToken;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_refreshToken);
    }

    /// <inheritdoc />
    public Task ClearTokensAsync(CancellationToken cancellationToken = default)
    {
        _accessToken = null;
        _refreshToken = null;
        return Task.CompletedTask;
    }
}