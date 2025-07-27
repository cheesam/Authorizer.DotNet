using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Authorizer.DotNet;
using Authorizer.DotNet.Extensions;
using Authorizer.DotNet.Models.Requests;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

builder.Services.AddAuthorizer(builder.Configuration, "Authorizer");

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var authorizerClient = host.Services.GetRequiredService<IAuthorizerClient>();

logger.LogInformation("Authorizer.dev Console Sample Starting...");

try
{
    await RunSampleAsync(authorizerClient, logger);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during execution");
}

logger.LogInformation("Sample execution completed.");

static async Task RunSampleAsync(IAuthorizerClient client, ILogger logger)
{
    logger.LogInformation("=== Authorizer.dev .NET SDK Console Sample ===");

    logger.LogInformation("1. Getting Authorizer metadata...");
    var metaResponse = await client.GetMetaAsync();
    if (metaResponse.IsSuccess)
    {
        logger.LogInformation("Authorizer Version: {Version}", metaResponse.Data?.Version);
        logger.LogInformation("Signup Enabled: {SignupEnabled}", metaResponse.Data?.IsSignupEnabled);
        logger.LogInformation("Social Login Enabled: {SocialLoginEnabled}", metaResponse.Data?.IsSocialLoginEnabled);
        
        if (metaResponse.Data?.SocialLoginProviders?.Any() == true)
        {
            logger.LogInformation("Social Providers: {Providers}", string.Join(", ", metaResponse.Data.SocialLoginProviders));
        }
    }
    else
    {
        logger.LogWarning("Failed to get metadata: {Errors}", 
            string.Join(", ", metaResponse.Errors?.Select(e => e.Message) ?? Array.Empty<string>()));
    }

    Console.WriteLine("\nPress Enter to continue to login demo, or 'q' to quit...");
    var input = Console.ReadLine();
    if (input?.ToLower() == "q") return;

    logger.LogInformation("2. Testing authentication flow...");
    
    Console.Write("Enter test email (or press Enter for demo@example.com): ");
    var email = Console.ReadLine();
    if (string.IsNullOrEmpty(email))
        email = "demo@example.com";

    Console.Write("Enter test password (or press Enter for 'password123'): ");
    var password = ReadPassword();
    if (string.IsNullOrEmpty(password))
        password = "password123";

    logger.LogInformation("3. Attempting signup first (in case user doesn't exist)...");
    var signupRequest = new SignupRequest
    {
        Email = email,
        Password = password,
        ConfirmPassword = password,
        GivenName = "Demo",
        FamilyName = "User"
    };

    var signupResponse = await client.SignupAsync(signupRequest);
    if (signupResponse.IsSuccess)
    {
        logger.LogInformation("Signup successful! User ID: {UserId}", signupResponse.Data?.User?.Id);
        
        if (!string.IsNullOrEmpty(signupResponse.Data?.AccessToken))
        {
            await DemonstrateProfileOperations(client, signupResponse.Data.AccessToken, logger);
        }
    }
    else
    {
        logger.LogInformation("Signup failed (user might already exist): {Errors}", 
            string.Join(", ", signupResponse.Errors?.Select(e => e.Message) ?? Array.Empty<string>()));

        logger.LogInformation("4. Attempting login...");
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var loginResponse = await client.LoginAsync(loginRequest);
        if (loginResponse.IsSuccess)
        {
            logger.LogInformation("Login successful! User ID: {UserId}", loginResponse.Data?.User?.Id);
            
            if (!string.IsNullOrEmpty(loginResponse.Data?.AccessToken))
            {
                await DemonstrateProfileOperations(client, loginResponse.Data.AccessToken, logger);
            }
        }
        else
        {
            logger.LogWarning("Login failed: {Errors}", 
                string.Join(", ", loginResponse.Errors?.Select(e => e.Message) ?? Array.Empty<string>()));
        }
    }

    logger.LogInformation("5. Testing forgot password flow...");
    var forgotPasswordResponse = await client.ForgotPasswordAsync(email);
    if (forgotPasswordResponse.IsSuccess)
    {
        logger.LogInformation("Forgot password email sent successfully (if user exists)");
    }
    else
    {
        logger.LogWarning("Forgot password failed: {Errors}", 
            string.Join(", ", forgotPasswordResponse.Errors?.Select(e => e.Message) ?? Array.Empty<string>()));
    }
}

static async Task DemonstrateProfileOperations(IAuthorizerClient client, string accessToken, ILogger logger)
{
    logger.LogInformation("6. Getting user profile...");
    var profileResponse = await client.GetProfileAsync(accessToken);
    if (profileResponse.IsSuccess && profileResponse.Data != null)
    {
        logger.LogInformation("Profile retrieved successfully:");
        logger.LogInformation("  Email: {Email}", profileResponse.Data.Email);
        logger.LogInformation("  Name: {Name}", profileResponse.Data.Name ?? "Not set");
        logger.LogInformation("  Email Verified: {EmailVerified}", profileResponse.Data.EmailVerified);
        logger.LogInformation("  Roles: {Roles}", 
            profileResponse.Data.Roles?.Any() == true ? string.Join(", ", profileResponse.Data.Roles) : "None");
    }
    else
    {
        logger.LogWarning("Failed to get profile: {Errors}", 
            string.Join(", ", profileResponse.Errors?.Select(e => e.Message) ?? Array.Empty<string>()));
    }

    logger.LogInformation("7. Validating JWT token...");
    var validateResponse = await client.ValidateJwtAsync(accessToken);
    if (validateResponse.IsSuccess)
    {
        logger.LogInformation("JWT token is valid");
    }
    else
    {
        logger.LogWarning("JWT validation failed: {Errors}", 
            string.Join(", ", validateResponse.Errors?.Select(e => e.Message) ?? Array.Empty<string>()));
    }

    logger.LogInformation("8. Getting session info...");
    var sessionResponse = await client.GetSessionAsync();
    if (sessionResponse.IsSuccess && sessionResponse.Data != null)
    {
        logger.LogInformation("Session is valid: {IsValid}", sessionResponse.Data.IsValid);
        logger.LogInformation("Session user: {UserEmail}", sessionResponse.Data.User?.Email);
    }
    else
    {
        logger.LogWarning("Failed to get session: {Errors}", 
            string.Join(", ", sessionResponse.Errors?.Select(e => e.Message) ?? Array.Empty<string>()));
    }

    logger.LogInformation("9. Logging out...");
    var logoutResponse = await client.LogoutAsync();
    if (logoutResponse.IsSuccess)
    {
        logger.LogInformation("Logout successful");
    }
    else
    {
        logger.LogWarning("Logout failed: {Errors}", 
            string.Join(", ", logoutResponse.Errors?.Select(e => e.Message) ?? Array.Empty<string>()));
    }
}

static string ReadPassword()
{
    var password = string.Empty;
    ConsoleKeyInfo keyInfo;

    do
    {
        keyInfo = Console.ReadKey(true);
        
        if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
        {
            password += keyInfo.KeyChar;
            Console.Write("*");
        }
        else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
        {
            password = password[0..^1];
            Console.Write("\b \b");
        }
    }
    while (keyInfo.Key != ConsoleKey.Enter);

    Console.WriteLine();
    return password;
}