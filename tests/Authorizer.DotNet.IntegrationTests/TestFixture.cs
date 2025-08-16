using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Authorizer.DotNet;
using Authorizer.DotNet.Extensions;

namespace Authorizer.DotNet.IntegrationTests;

public class TestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }
    public IAuthorizerClient AuthorizerClient { get; }
    
    public TestFixture()
    {
        var services = new ServiceCollection();
        
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.personal.json", optional: true) // Personal settings override defaults
            .AddEnvironmentVariables(); // Environment variables override everything
        
        var config = configBuilder.Build();
        
        services.AddAuthorizer(config, "Authorizer");
        
        ServiceProvider = services.BuildServiceProvider();
        AuthorizerClient = ServiceProvider.GetRequiredService<IAuthorizerClient>();
    }

    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
            disposable.Dispose();
    }
}