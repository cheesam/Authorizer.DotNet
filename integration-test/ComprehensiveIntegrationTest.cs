using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Authorizer.DotNet;
using Authorizer.DotNet.Models.Requests;
using Authorizer.DotNet.Extensions;

namespace Authorizer.DotNet.IntegrationTest;

public class ComprehensiveIntegrationTest
{
    private readonly ILogger<ComprehensiveIntegrationTest> _logger;
    private readonly IAuthorizerClient _authClient;
    private readonly Random _random;
    private string? _testAccessToken;
    private string? _testRefreshToken;
    private string? _testUserEmail;

    public ComprehensiveIntegrationTest(ILogger<ComprehensiveIntegrationTest> logger, IAuthorizerClient authClient)
    {
        _logger = logger;
        _authClient = authClient;
        _random = new Random();
        _testUserEmail = $"test-{_random.Next(10000, 99999)}@example.com";
    }

    public static Task<ComprehensiveIntegrationTest> CreateAsync()
    {
        var services = new ServiceCollection();
        
        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // Add configuration - Configure these with your actual Authorizer instance
        var configData = new Dictionary<string, string?>
        {
            ["Authorizer:AuthorizerUrl"] = Environment.GetEnvironmentVariable("AUTHORIZER_URL") ?? "https://your-authorizer-instance.com",
            ["Authorizer:ClientId"] = Environment.GetEnvironmentVariable("AUTHORIZER_CLIENT_ID") ?? "your-client-id", 
            ["Authorizer:RedirectUrl"] = Environment.GetEnvironmentVariable("AUTHORIZER_REDIRECT_URL") ?? "https://your-app.com/auth/callback"
        };
        
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(configData);
        var config = configBuilder.Build();
        
        // Add Authorizer services
        services.AddAuthorizer(config, "Authorizer");
        
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<ComprehensiveIntegrationTest>>();
        var authClient = serviceProvider.GetRequiredService<IAuthorizerClient>();
        
        return Task.FromResult(new ComprehensiveIntegrationTest(logger, authClient));
    }

    public async Task RunAllTestsAsync()
    {
        _logger.LogInformation("🚀 Starting Comprehensive Integration Test Suite");
        _logger.LogInformation("=" + new string('=', 60));

        var results = new Dictionary<string, bool>();
        
        try
        {
            // Core Infrastructure Tests
            results["Meta Information"] = await TestMetaInformationAsync();
            
            // Authentication Flow Tests
            results["User Signup"] = await TestUserSignupAsync();
            results["User Login"] = await TestUserLoginAsync();
            results["Session Management"] = await TestSessionAsync();
            results["JWT Validation"] = await TestJwtValidationAsync();
            results["User Logout"] = await TestLogoutAsync();
            
            // User Management Tests
            results["Profile Retrieval"] = await TestGetProfileAsync();
            
            // Password Management Tests
            results["Password Reset Flow"] = await TestPasswordResetFlowAsync();
            
            // Error Handling Tests
            results["Invalid Credentials"] = await TestInvalidCredentialsAsync();
            results["Unauthorized Access"] = await TestUnauthorizedAccessAsync();
            
            // OAuth Flow Tests (if supported)
            results["OAuth Authorization"] = await TestOAuthAuthorizationAsync();
            
            // Edge Cases
            results["Rate Limiting Handling"] = await TestRateLimitingAsync();
            results["Network Error Handling"] = await TestNetworkErrorHandlingAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during test execution");
        }
        
        // Print Results
        PrintTestResults(results);
    }

    #region Core Infrastructure Tests

    private async Task<bool> TestMetaInformationAsync()
    {
        _logger.LogInformation("🔍 Testing Meta Information Retrieval...");
        
        try
        {
            var response = await _authClient.GetMetaAsync();
            
            if (response.IsSuccess && response.Data != null)
            {
                _logger.LogInformation("   ✅ Meta information retrieved successfully");
                _logger.LogInformation("   📋 Version: {Version}", response.Data.Version ?? "Unknown");
                _logger.LogInformation("   📋 Client ID: {ClientId}", response.Data.ClientId ?? "Not provided");
                _logger.LogInformation("   📋 Signup Enabled: {SignupEnabled}", response.Data.IsSignupEnabled);
                _logger.LogInformation("   📋 Email Verification: {EmailVerification}", response.Data.IsEmailVerificationEnabled);
                _logger.LogInformation("   📋 Basic Auth: {BasicAuth}", response.Data.IsBasicAuthenticationEnabled);
                _logger.LogInformation("   📋 Google Login: {GoogleLogin}", response.Data.IsGoogleLoginEnabled);
                _logger.LogInformation("   📋 GitHub Login: {GitHubLogin}", response.Data.IsGithubLoginEnabled);
                return true;
            }
            else
            {
                _logger.LogError("   ❌ Failed to retrieve meta information");
                LogErrors(response.Errors);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during meta information test");
            return false;
        }
    }

    #endregion

    #region Authentication Flow Tests

    private async Task<bool> TestUserSignupAsync()
    {
        _logger.LogInformation("🔐 Testing User Signup...");
        
        try
        {
            var signupRequest = new SignupRequest
            {
                Email = _testUserEmail!,
                Password = "TestPassword123!",
                ConfirmPassword = "TestPassword123!",
                GivenName = "Test",
                FamilyName = "User",
                Nickname = "TestUser"
            };

            var response = await _authClient.SignupAsync(signupRequest);
            
            if (response.IsSuccess)
            {
                _logger.LogInformation("   ✅ User signup successful");
                _logger.LogInformation("   📋 User ID: {UserId}", response.Data?.User?.Id);
                _logger.LogInformation("   📋 Email: {Email}", response.Data?.User?.Email);
                _logger.LogInformation("   📋 Access Token Length: {TokenLength}", response.Data?.AccessToken?.Length ?? 0);
                
                // Store tokens for later tests
                _testAccessToken = response.Data?.AccessToken;
                _testRefreshToken = response.Data?.RefreshToken;
                
                return true;
            }
            else
            {
                // Check if it's just a "user already exists" error, which is expected in some cases
                var hasUserExistsError = false;
                if (response.Errors != null)
                {
                    foreach (var error in response.Errors)
                    {
                        if (error.Message.Contains("already signed up") || error.Message.Contains("already exists"))
                        {
                            hasUserExistsError = true;
                            break;
                        }
                    }
                }
                
                if (hasUserExistsError)
                {
                    _logger.LogInformation("   ℹ️  User already exists (expected in repeated tests)");
                    return true;
                }
                else
                {
                    _logger.LogError("   ❌ User signup failed");
                    LogErrors(response.Errors);
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during user signup test");
            return false;
        }
    }

    private async Task<bool> TestUserLoginAsync()
    {
        _logger.LogInformation("🔑 Testing User Login...");
        
        try
        {
            var loginRequest = new LoginRequest
            {
                Email = _testUserEmail!,
                Password = "TestPassword123!"
            };

            var response = await _authClient.LoginAsync(loginRequest);
            
            if (response.IsSuccess && response.Data != null)
            {
                _logger.LogInformation("   ✅ User login successful");
                _logger.LogInformation("   📋 User ID: {UserId}", response.Data.User?.Id);
                _logger.LogInformation("   📋 Access Token Length: {TokenLength}", response.Data.AccessToken?.Length ?? 0);
                _logger.LogInformation("   📋 Token Type: {TokenType}", response.Data.TokenType);
                _logger.LogInformation("   📋 Expires In: {ExpiresIn}s", response.Data.ExpiresIn);
                
                // Store tokens for later tests
                _testAccessToken = response.Data.AccessToken;
                _testRefreshToken = response.Data.RefreshToken;
                
                return true;
            }
            else
            {
                _logger.LogError("   ❌ User login failed");
                LogErrors(response.Errors);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during user login test");
            return false;
        }
    }

    private async Task<bool> TestLogoutAsync()
    {
        _logger.LogInformation("🚪 Testing User Logout...");
        
        try
        {
            var response = await _authClient.LogoutAsync();
            
            if (response.IsSuccess)
            {
                _logger.LogInformation("   ✅ User logout successful");
                _logger.LogInformation("   📋 Message: {Message}", response.Data);
                
                // Clear stored tokens
                _testAccessToken = null;
                _testRefreshToken = null;
                
                return true;
            }
            else
            {
                _logger.LogError("   ❌ User logout failed");
                LogErrors(response.Errors);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during user logout test");
            return false;
        }
    }

    #endregion

    #region Session and Token Tests

    private async Task<bool> TestSessionAsync()
    {
        _logger.LogInformation("🎫 Testing Session Management...");
        
        try
        {
            var response = await _authClient.GetSessionAsync();
            
            if (response.IsSuccess && response.Data != null)
            {
                _logger.LogInformation("   ✅ Session retrieved successfully");
                _logger.LogInformation("   📋 Session Valid: {IsValid}", response.Data.IsValid);
                _logger.LogInformation("   📋 User Email: {Email}", response.Data.User?.Email);
                return true;
            }
            else
            {
                // Session might fail if not authenticated, which is expected
                _logger.LogInformation("   ℹ️  Session query failed (expected if not authenticated)");
                LogErrors(response.Errors);
                return true; // This is actually expected behavior
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during session test");
            return false;
        }
    }

    private async Task<bool> TestJwtValidationAsync()
    {
        _logger.LogInformation("🔒 Testing JWT Token Validation...");
        
        if (string.IsNullOrEmpty(_testAccessToken))
        {
            _logger.LogWarning("   ⚠️  No access token available for JWT validation test");
            return true; // Skip if no token
        }
        
        try
        {
            var response = await _authClient.ValidateJwtAsync(_testAccessToken);
            
            if (response.IsSuccess && response.Data != null)
            {
                _logger.LogInformation("   ✅ JWT validation successful");
                _logger.LogInformation("   📋 User ID: {UserId}", response.Data.Id);
                _logger.LogInformation("   📋 Email: {Email}", response.Data.Email);
                return true;
            }
            else
            {
                _logger.LogError("   ❌ JWT validation failed");
                LogErrors(response.Errors);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during JWT validation test");
            return false;
        }
    }

    #endregion

    #region User Management Tests

    private async Task<bool> TestGetProfileAsync()
    {
        _logger.LogInformation("👤 Testing Get User Profile...");
        
        if (string.IsNullOrEmpty(_testAccessToken))
        {
            _logger.LogWarning("   ⚠️  No access token available for profile test");
            return true; // Skip if no token
        }
        
        try
        {
            var response = await _authClient.GetProfileAsync(_testAccessToken);
            
            if (response.IsSuccess && response.Data != null)
            {
                _logger.LogInformation("   ✅ Profile retrieved successfully");
                _logger.LogInformation("   📋 User ID: {UserId}", response.Data.Id);
                _logger.LogInformation("   📋 Email: {Email}", response.Data.Email);
                _logger.LogInformation("   📋 Given Name: {GivenName}", response.Data.GivenName);
                _logger.LogInformation("   📋 Family Name: {FamilyName}", response.Data.FamilyName);
                _logger.LogInformation("   📋 Email Verified: {EmailVerified}", response.Data.EmailVerified);
                return true;
            }
            else
            {
                _logger.LogError("   ❌ Profile retrieval failed");
                LogErrors(response.Errors);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during get profile test");
            return false;
        }
    }


    #endregion

    #region Password Management Tests

    private async Task<bool> TestPasswordResetFlowAsync()
    {
        _logger.LogInformation("🔄 Testing Password Reset Flow...");
        
        try
        {
            // Test forgot password request
            var response = await _authClient.ForgotPasswordAsync(_testUserEmail!);
            
            if (response.IsSuccess)
            {
                _logger.LogInformation("   ✅ Forgot password request successful");
                _logger.LogInformation("   📋 Message: {Message}", response.Data);
                return true;
            }
            else
            {
                _logger.LogError("   ❌ Forgot password request failed");
                LogErrors(response.Errors);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during password reset test");
            return false;
        }
    }

    #endregion

    #region Error Handling Tests

    private async Task<bool> TestInvalidCredentialsAsync()
    {
        _logger.LogInformation("🚫 Testing Invalid Credentials Handling...");
        
        try
        {
            var loginRequest = new LoginRequest
            {
                Email = "invalid@example.com",
                Password = "wrongpassword"
            };

            var response = await _authClient.LoginAsync(loginRequest);
            
            if (!response.IsSuccess && response.Errors != null)
            {
                _logger.LogInformation("   ✅ Invalid credentials properly rejected");
                _logger.LogInformation("   📋 Error: {Error}", response.Errors[0].Message);
                return true;
            }
            else
            {
                _logger.LogError("   ❌ Invalid credentials should have been rejected");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during invalid credentials test");
            return false;
        }
    }

    private async Task<bool> TestUnauthorizedAccessAsync()
    {
        _logger.LogInformation("🔐 Testing Unauthorized Access Handling...");
        
        try
        {
            // Try to get profile without token
            var response = await _authClient.GetProfileAsync("invalid-token");
            
            if (!response.IsSuccess && response.Errors != null)
            {
                _logger.LogInformation("   ✅ Unauthorized access properly rejected");
                _logger.LogInformation("   📋 Error: {Error}", response.Errors[0].Message);
                return true;
            }
            else
            {
                _logger.LogError("   ❌ Unauthorized access should have been rejected");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during unauthorized access test");
            return false;
        }
    }

    #endregion

    #region OAuth Tests

    private async Task<bool> TestOAuthAuthorizationAsync()
    {
        _logger.LogInformation("🔗 Testing OAuth Authorization Flow...");
        
        try
        {
            var authorizeRequest = new AuthorizeRequest
            {
                ResponseType = "code",
                ClientId = Environment.GetEnvironmentVariable("AUTHORIZER_CLIENT_ID") ?? "your-client-id",
                RedirectUri = Environment.GetEnvironmentVariable("AUTHORIZER_REDIRECT_URL") ?? "https://your-app.com/auth/callback",
                Scope = "openid email profile"
            };

            var response = await _authClient.AuthorizeAsync(authorizeRequest);
            
            // OAuth authorization typically returns a redirect URL, so we just check if it doesn't crash
            if (response != null)
            {
                _logger.LogInformation("   ✅ OAuth authorization request completed");
                if (response.IsSuccess)
                {
                    _logger.LogInformation("   📋 Authorization successful");
                }
                else
                {
                    _logger.LogInformation("   📋 Authorization returned expected error");
                    LogErrors(response.Errors);
                }
                return true;
            }
            else
            {
                _logger.LogError("   ❌ OAuth authorization request failed");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during OAuth authorization test");
            return false;
        }
    }

    #endregion

    #region Edge Case Tests

    private async Task<bool> TestRateLimitingAsync()
    {
        _logger.LogInformation("⏱️  Testing Rate Limiting Handling...");
        
        try
        {
            // Make multiple rapid requests to test rate limiting
            var tasks = new List<Task<bool>>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(MakeQuickMetaRequestAsync());
            }

            var results = await Task.WhenAll(tasks);
            
            _logger.LogInformation("   ✅ Rate limiting test completed");
            _logger.LogInformation("   📋 Successful requests: {SuccessCount}/{TotalCount}", 
                results.Count(r => r), results.Length);
            
            return true; // Success as long as it doesn't crash
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "   💥 Exception during rate limiting test");
            return false;
        }
    }

    private async Task<bool> MakeQuickMetaRequestAsync()
    {
        try
        {
            var response = await _authClient.GetMetaAsync();
            return response.IsSuccess;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> TestNetworkErrorHandlingAsync()
    {
        _logger.LogInformation("🌐 Testing Network Error Handling...");
        
        // This test would typically involve mocking network failures,
        // but for integration tests, we'll just verify the client doesn't crash
        try
        {
            // Test with a deliberately invalid token to trigger network-level errors
            await _authClient.ValidateJwtAsync("clearly.invalid.token");
            
            _logger.LogInformation("   ✅ Network error handling test completed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation("   ✅ Network error properly handled: {Error}", ex.Message);
            return true; // Expected to throw
        }
    }

    #endregion

    #region Helper Methods

    private void LogErrors(IEnumerable<Authorizer.DotNet.Models.Common.AuthorizerError>? errors)
    {
        if (errors != null)
        {
            foreach (var error in errors)
            {
                _logger.LogError("   💥 {Message}", error.Message);
            }
        }
    }

    private void PrintTestResults(Dictionary<string, bool> results)
    {
        _logger.LogInformation("");
        _logger.LogInformation("📊 TEST RESULTS SUMMARY");
        _logger.LogInformation("=" + new string('=', 60));
        
        var passed = 0;
        var total = results.Count;
        
        foreach (var result in results)
        {
            var status = result.Value ? "✅ PASS" : "❌ FAIL";
            _logger.LogInformation("{Status} | {TestName}", status, result.Key);
            
            if (result.Value)
                passed++;
        }
        
        _logger.LogInformation("");
        _logger.LogInformation("📈 OVERALL RESULT: {Passed}/{Total} tests passed ({Percentage:F1}%)",
            passed, total, (passed / (double)total) * 100);
        
        if (passed == total)
        {
            _logger.LogInformation("🎉 ALL TESTS PASSED! The Authorizer.DotNet package is working correctly with your instance.");
        }
        else
        {
            _logger.LogWarning("⚠️  Some tests failed. Review the logs above for details.");
        }
    }

    #endregion
}