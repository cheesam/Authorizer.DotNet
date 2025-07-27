using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Authorizer.DotNet.Options;

/// <summary>
/// Configuration options for the Authorizer.dev client.
/// </summary>
public class AuthorizerOptions
{
    /// <summary>
    /// The base URL of your Authorizer.dev instance.
    /// </summary>
    [Required]
    public string AuthorizerUrl { get; set; } = string.Empty;

    /// <summary>
    /// The redirect URL for OAuth flows.
    /// </summary>
    [Required]
    public string RedirectUrl { get; set; } = string.Empty;

    /// <summary>
    /// Optional client ID for OAuth flows.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// HTTP timeout for API requests. Default is 30 seconds.
    /// </summary>
    public TimeSpan HttpTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Optional API key for server-to-server authentication.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Additional headers to include with all requests.
    /// </summary>
    public Dictionary<string, string> ExtraHeaders { get; set; } = new();

    /// <summary>
    /// Whether to use secure cookies. Default is true.
    /// </summary>
    public bool UseSecureCookies { get; set; } = true;

    /// <summary>
    /// Cookie domain for session management.
    /// </summary>
    public string? CookieDomain { get; set; }

    /// <summary>
    /// Whether to disable browser history for OAuth flows. Default is false.
    /// </summary>
    public bool DisableBrowserHistory { get; set; } = false;
}