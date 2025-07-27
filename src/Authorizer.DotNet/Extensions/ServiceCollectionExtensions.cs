using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Authorizer.DotNet.Internal;
using Authorizer.DotNet.Options;

namespace Authorizer.DotNet.Extensions;

/// <summary>
/// Extension methods for configuring Authorizer.dev services in dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Authorizer.dev services to the service collection with configuration action.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">Action to configure AuthorizerOptions.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configureOptions is null.</exception>
    public static IServiceCollection AddAuthorizer(
        this IServiceCollection services, 
        Action<AuthorizerOptions> configureOptions)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));
        if (configureOptions == null) 
            throw new ArgumentNullException(nameof(configureOptions));

        services.Configure(configureOptions);
        return AddAuthorizerCore(services);
    }

    /// <summary>
    /// Adds Authorizer.dev services to the service collection with pre-configured options.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="options">Pre-configured AuthorizerOptions instance.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or options is null.</exception>
    public static IServiceCollection AddAuthorizer(
        this IServiceCollection services, 
        AuthorizerOptions options)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));
        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        services.Configure<AuthorizerOptions>(opts =>
        {
            opts.AuthorizerUrl = options.AuthorizerUrl;
            opts.RedirectUrl = options.RedirectUrl;
            opts.ClientId = options.ClientId;
            opts.HttpTimeout = options.HttpTimeout;
            opts.ApiKey = options.ApiKey;
            opts.ExtraHeaders = options.ExtraHeaders;
            opts.UseSecureCookies = options.UseSecureCookies;
            opts.CookieDomain = options.CookieDomain;
            opts.DisableBrowserHistory = options.DisableBrowserHistory;
        });

        return AddAuthorizerCore(services);
    }

    /// <summary>
    /// Adds Authorizer.dev services to the service collection using configuration section.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The configuration section name. Defaults to "Authorizer".</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configuration is null.</exception>
    public static IServiceCollection AddAuthorizer(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Authorizer")
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));
        if (configuration == null) 
            throw new ArgumentNullException(nameof(configuration));

        services.Configure<AuthorizerOptions>(configureOptions =>
        {
            configuration.GetSection(sectionName).Bind(configureOptions);
        });
        return AddAuthorizerCore(services);
    }

    /// <summary>
    /// Adds Authorizer.dev services to the service collection with advanced HTTP client configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">Action to configure AuthorizerOptions.</param>
    /// <param name="configureHttpClient">Action to configure the underlying HttpClient.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configureOptions is null.</exception>
    public static IServiceCollection AddAuthorizer(
        this IServiceCollection services,
        Action<AuthorizerOptions> configureOptions,
        Action<HttpClient> configureHttpClient)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));
        if (configureOptions == null) 
            throw new ArgumentNullException(nameof(configureOptions));
        if (configureHttpClient == null) 
            throw new ArgumentNullException(nameof(configureHttpClient));

        services.Configure(configureOptions);
        return AddAuthorizerCore(services, configureHttpClient);
    }

    /// <summary>
    /// Adds Authorizer.dev services to the service collection with advanced HTTP client configuration using IServiceProvider.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">Action to configure AuthorizerOptions.</param>
    /// <param name="configureHttpClient">Action to configure the underlying HttpClient with access to IServiceProvider.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configureOptions is null.</exception>
    public static IServiceCollection AddAuthorizer(
        this IServiceCollection services,
        Action<AuthorizerOptions> configureOptions,
        Action<IServiceProvider, HttpClient> configureHttpClient)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));
        if (configureOptions == null) 
            throw new ArgumentNullException(nameof(configureOptions));
        if (configureHttpClient == null) 
            throw new ArgumentNullException(nameof(configureHttpClient));

        services.Configure(configureOptions);
        return AddAuthorizerCore(services, configureHttpClient);
    }

    private static IServiceCollection AddAuthorizerCore(
        IServiceCollection services, 
        Action<HttpClient>? configureHttpClient = null)
    {
        var httpClientBuilder = services.AddHttpClient<AuthorizerHttpClient>("Authorizer.DotNet");

        if (configureHttpClient != null)
        {
            httpClientBuilder.ConfigureHttpClient(configureHttpClient);
        }

        services.AddScoped<IAuthorizerClient, AuthorizerClient>();

        services.AddOptions<AuthorizerOptions>()
            .PostConfigure<IServiceProvider>((options, serviceProvider) =>
            {
                ValidateOptions(options);
            });

        return services;
    }

    private static IServiceCollection AddAuthorizerCore(
        IServiceCollection services, 
        Action<IServiceProvider, HttpClient> configureHttpClient)
    {
        var httpClientBuilder = services.AddHttpClient<AuthorizerHttpClient>("Authorizer.DotNet");
        httpClientBuilder.ConfigureHttpClient(configureHttpClient);

        services.AddScoped<IAuthorizerClient, AuthorizerClient>();

        services.AddOptions<AuthorizerOptions>()
            .PostConfigure<IServiceProvider>((options, serviceProvider) =>
            {
                ValidateOptions(options);
            });

        return services;
    }

    private static void ValidateOptions(AuthorizerOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.AuthorizerUrl))
        {
            throw new InvalidOperationException("AuthorizerOptions.AuthorizerUrl must be configured.");
        }

        if (string.IsNullOrWhiteSpace(options.RedirectUrl))
        {
            throw new InvalidOperationException("AuthorizerOptions.RedirectUrl must be configured.");
        }

        if (!Uri.TryCreate(options.AuthorizerUrl, UriKind.Absolute, out var authorizerUri))
        {
            throw new InvalidOperationException("AuthorizerOptions.AuthorizerUrl must be a valid absolute URI.");
        }

        if (!Uri.TryCreate(options.RedirectUrl, UriKind.Absolute, out var redirectUri))
        {
            throw new InvalidOperationException("AuthorizerOptions.RedirectUrl must be a valid absolute URI.");
        }

        if (options.HttpTimeout <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("AuthorizerOptions.HttpTimeout must be greater than zero.");
        }
    }
}