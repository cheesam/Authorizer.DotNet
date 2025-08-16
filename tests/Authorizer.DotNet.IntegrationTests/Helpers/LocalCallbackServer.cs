using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Authorizer.DotNet.IntegrationTests.Helpers;

/// <summary>
/// Simple HTTP listener to handle OAuth callbacks during integration tests
/// </summary>
public class LocalCallbackServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly string _baseUrl;
    private readonly TaskCompletionSource<CallbackResult> _callbackReceived;
    private CancellationTokenSource? _cancellationTokenSource;

    public LocalCallbackServer(int port = 8080)
    {
        _baseUrl = $"http://localhost:{port}/";
        _listener = new HttpListener();
        _listener.Prefixes.Add(_baseUrl);
        _callbackReceived = new TaskCompletionSource<CallbackResult>();
    }

    public string CallbackUrl => $"{_baseUrl}auth/callback";

    public async Task<CallbackResult> StartAndWaitForCallbackAsync(TimeSpan timeout = default)
    {
        if (timeout == default)
            timeout = TimeSpan.FromMinutes(2);

        _cancellationTokenSource = new CancellationTokenSource(timeout);
        
        try
        {
            _listener.Start();
            
            // Start listening for requests in background
            _ = Task.Run(async () =>
            {
                try
                {
                    while (!_cancellationTokenSource.Token.IsCancellationRequested && _listener.IsListening)
                    {
                        var context = await _listener.GetContextAsync();
                        await HandleRequestAsync(context);
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Expected when stopping
                }
                catch (HttpListenerException)
                {
                    // Expected when stopping
                }
            }, _cancellationTokenSource.Token);

            // Wait for callback or timeout
            var completedTask = await Task.WhenAny(
                _callbackReceived.Task,
                Task.Delay(timeout, _cancellationTokenSource.Token));

            if (completedTask == _callbackReceived.Task)
            {
                return await _callbackReceived.Task;
            }
            else
            {
                throw new TimeoutException($"OAuth callback not received within {timeout.TotalSeconds} seconds");
            }
        }
        finally
        {
            Stop();
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        try
        {
            if (request.Url?.AbsolutePath == "/auth/callback")
            {
                // Parse query parameters
                var query = HttpUtility.ParseQueryString(request.Url.Query);
                var code = query["code"];
                var error = query["error"];
                var errorDescription = query["error_description"];
                var state = query["state"];

                // Send a simple HTML response
                var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>OAuth Callback - Integration Test</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        .success {{ color: green; }}
        .error {{ color: red; }}
    </style>
</head>
<body>
    <h1>OAuth Callback Received</h1>
    {(string.IsNullOrEmpty(error) 
        ? $"<p class='success'>✅ Authorization successful!</p><p><strong>Code:</strong> {code}</p>" 
        : $"<p class='error'>❌ Authorization failed!</p><p><strong>Error:</strong> {error}</p><p><strong>Description:</strong> {errorDescription}</p>")}
    <p><em>This window can be closed. Integration test will continue...</em></p>
</body>
</html>";

                var buffer = System.Text.Encoding.UTF8.GetBytes(html);
                response.ContentType = "text/html; charset=utf-8";
                response.ContentLength64 = buffer.Length;
                response.StatusCode = 200;

                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();

                // Complete the callback task
                var result = new CallbackResult
                {
                    Success = string.IsNullOrEmpty(error),
                    Code = code,
                    Error = error,
                    ErrorDescription = errorDescription,
                    State = state
                };

                _callbackReceived.SetResult(result);
            }
            else
            {
                // Return 404 for other paths
                response.StatusCode = 404;
                response.Close();
            }
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Close();
            
            if (!_callbackReceived.Task.IsCompleted)
            {
                _callbackReceived.SetException(ex);
            }
        }
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        if (_listener.IsListening)
        {
            _listener.Stop();
        }
    }

    public void Dispose()
    {
        Stop();
        _cancellationTokenSource?.Dispose();
        _listener?.Close();
    }
}

public class CallbackResult
{
    public bool Success { get; set; }
    public string? Code { get; set; }
    public string? Error { get; set; }
    public string? ErrorDescription { get; set; }
    public string? State { get; set; }
}