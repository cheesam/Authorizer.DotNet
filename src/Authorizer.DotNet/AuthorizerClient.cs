using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Authorizer.DotNet.Internal;
using Authorizer.DotNet.Models.Common;
using Authorizer.DotNet.Models.Requests;
using Authorizer.DotNet.Models.Responses;
using Authorizer.DotNet.Options;

namespace Authorizer.DotNet;

/// <summary>
/// Client for interacting with the Authorizer.dev authentication service.
/// </summary>
public class AuthorizerClient : IAuthorizerClient
{
    private readonly AuthorizerHttpClient _httpClient;
    private readonly AuthorizerOptions _options;
    private readonly ILogger<AuthorizerClient> _logger;

    /// <summary>
    /// Initializes a new instance of the AuthorizerClient class.
    /// </summary>
    /// <param name="httpClient">The internal HTTP client for API communication.</param>
    /// <param name="options">Configuration options for the client.</param>
    /// <param name="logger">Logger instance for diagnostic information.</param>
    public AuthorizerClient(
        AuthorizerHttpClient httpClient,
        IOptions<AuthorizerOptions> options,
        ILogger<AuthorizerClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Authentication Methods

    /// <inheritdoc />
    public async Task<AuthorizerResponse<LoginResponse>> LoginAsync(
        LoginRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Attempting to login user with email: {Email}", request.Email);

        const string query = @"
            mutation login($email: String!, $password: String!, $scope: String, $state: String, $roles: [String!], $is_remember_me: Boolean) {
                login(params: {
                    email: $email
                    password: $password
                    scope: $scope
                    state: $state
                    roles: $roles
                    is_remember_me: $is_remember_me
                }) {
                    access_token
                    refresh_token
                    id_token
                    token_type
                    expires_in
                    scope
                    state
                    created_at
                    session_token
                    should_show_email_otp_screen
                    should_show_mobile_otp_screen
                    user {
                        id
                        email
                        email_verified
                        given_name
                        family_name
                        middle_name
                        name
                        nickname
                        preferred_username
                        picture
                        website
                        gender
                        birthdate
                        phone_number
                        phone_number_verified
                        created_at
                        updated_at
                        roles
                        is_active
                        signup_methods
                    }
                }
            }";

        var variables = new
        {
            email = request.Email,
            password = request.Password,
            scope = request.Scope,
            state = request.State,
            roles = request.Roles,
            is_remember_me = request.IsRememberMe
        };

        var response = await _httpClient.PostGraphQLAsync<LoginResponse>(query, variables, cancellationToken);
        
        if (response.IsSuccess)
        {
            _logger.LogInformation("User login successful for email: {Email}", request.Email);
        }
        else
        {
            _logger.LogWarning("User login failed for email: {Email}. Errors: {Errors}", 
                request.Email, string.Join(", ", response.Errors?.Select(e => e.Message) ?? Array.Empty<string>()));
        }

        return response;
    }

    /// <inheritdoc />
    public async Task<AuthorizerResponse<SignupResponse>> SignupAsync(
        SignupRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Attempting to signup user with email: {Email}", request.Email);

        const string query = @"
            mutation signup($email: String!, $password: String!, $confirm_password: String!, $given_name: String, $family_name: String, $middle_name: String, $nickname: String, $preferred_username: String, $phone_number: String, $picture: String, $birthdate: String, $gender: String, $scope: String, $state: String, $roles: [String!], $app_data: JSON, $redirect_uri: String) {
                signup(params: {
                    email: $email
                    password: $password
                    confirm_password: $confirm_password
                    given_name: $given_name
                    family_name: $family_name
                    middle_name: $middle_name
                    nickname: $nickname
                    preferred_username: $preferred_username
                    phone_number: $phone_number
                    picture: $picture
                    birthdate: $birthdate
                    gender: $gender
                    scope: $scope
                    state: $state
                    roles: $roles
                    app_data: $app_data
                    redirect_uri: $redirect_uri
                }) {
                    access_token
                    refresh_token
                    id_token
                    token_type
                    expires_in
                    scope
                    state
                    created_at
                    session_token
                    message
                    should_show_email_otp_screen
                    user {
                        id
                        email
                        email_verified
                        given_name
                        family_name
                        middle_name
                        name
                        nickname
                        preferred_username
                        picture
                        website
                        gender
                        birthdate
                        phone_number
                        phone_number_verified
                        created_at
                        updated_at
                        roles
                        is_active
                        signup_methods
                    }
                }
            }";

        var variables = new
        {
            email = request.Email,
            password = request.Password,
            confirm_password = request.ConfirmPassword,
            given_name = request.GivenName,
            family_name = request.FamilyName,
            middle_name = request.MiddleName,
            nickname = request.Nickname,
            preferred_username = request.PreferredUsername,
            phone_number = request.PhoneNumber,
            picture = request.Picture,
            birthdate = request.Birthdate,
            gender = request.Gender,
            scope = request.Scope,
            state = request.State,
            roles = request.Roles,
            app_data = request.AppData,
            redirect_uri = request.RedirectUri
        };

        var response = await _httpClient.PostGraphQLAsync<SignupResponse>(query, variables, cancellationToken);
        
        if (response.IsSuccess)
        {
            _logger.LogInformation("User signup successful for email: {Email}", request.Email);
        }
        else
        {
            _logger.LogWarning("User signup failed for email: {Email}. Errors: {Errors}", 
                request.Email, string.Join(", ", response.Errors?.Select(e => e.Message) ?? Array.Empty<string>()));
        }

        return response;
    }

    /// <inheritdoc />
    public async Task<AuthorizerResponse<AuthorizeResponse>> AuthorizeAsync(
        AuthorizeRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Initiating OAuth authorization for client: {ClientId}", request.ClientId);

        var formData = new Dictionary<string, string>
        {
            ["response_type"] = request.ResponseType,
            ["client_id"] = request.ClientId,
            ["redirect_uri"] = request.RedirectUri
        };

        if (!string.IsNullOrEmpty(request.Scope))
            formData["scope"] = request.Scope;
        if (!string.IsNullOrEmpty(request.State))
            formData["state"] = request.State;
        if (!string.IsNullOrEmpty(request.CodeChallenge))
            formData["code_challenge"] = request.CodeChallenge;
        if (!string.IsNullOrEmpty(request.CodeChallengeMethod))
            formData["code_challenge_method"] = request.CodeChallengeMethod;
        if (!string.IsNullOrEmpty(request.Nonce))
            formData["nonce"] = request.Nonce;
        if (!string.IsNullOrEmpty(request.ResponseMode))
            formData["response_mode"] = request.ResponseMode;
        if (!string.IsNullOrEmpty(request.Prompt))
            formData["prompt"] = request.Prompt;
        if (request.MaxAge.HasValue)
            formData["max_age"] = request.MaxAge.Value.ToString();
        if (!string.IsNullOrEmpty(request.UiLocales))
            formData["ui_locales"] = request.UiLocales;
        if (!string.IsNullOrEmpty(request.LoginHint))
            formData["login_hint"] = request.LoginHint;

        return await _httpClient.PostFormAsync<AuthorizeResponse>("oauth/authorize", formData, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AuthorizerResponse<TokenResponse>> GetTokenAsync(
        GetTokenRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Exchanging token for grant type: {GrantType}", request.GrantType);

        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = request.GrantType
        };

        if (!string.IsNullOrEmpty(request.Code))
            formData["code"] = request.Code;
        if (!string.IsNullOrEmpty(request.CodeVerifier))
            formData["code_verifier"] = request.CodeVerifier;
        if (!string.IsNullOrEmpty(request.RefreshToken))
            formData["refresh_token"] = request.RefreshToken;
        if (!string.IsNullOrEmpty(request.ClientId))
            formData["client_id"] = request.ClientId;
        if (!string.IsNullOrEmpty(request.ClientSecret))
            formData["client_secret"] = request.ClientSecret;
        if (!string.IsNullOrEmpty(request.RedirectUri))
            formData["redirect_uri"] = request.RedirectUri;
        if (!string.IsNullOrEmpty(request.Scope))
            formData["scope"] = request.Scope;
        if (!string.IsNullOrEmpty(request.Username))
            formData["username"] = request.Username;
        if (!string.IsNullOrEmpty(request.Password))
            formData["password"] = request.Password;

        return await _httpClient.PostFormAsync<TokenResponse>("oauth/token", formData, cancellationToken);
    }

    #endregion

    #region User Management

    /// <inheritdoc />
    public async Task<AuthorizerResponse<UserProfile>> GetProfileAsync(
        string accessToken, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));

        _logger.LogDebug("Retrieving user profile");

        const string query = @"
            query profile($token: String!) {
                profile(token: $token) {
                    id
                    email
                    email_verified
                    given_name
                    family_name
                    middle_name
                    name
                    nickname
                    preferred_username
                    picture
                    website
                    gender
                    birthdate
                    phone_number
                    phone_number_verified
                    created_at
                    updated_at
                    roles
                    is_active
                    signup_methods
                    has_mobile_otp
                    backup_codes
                    authenticator_devices
                }
            }";

        var variables = new { token = accessToken };

        return await _httpClient.PostGraphQLAsync<UserProfile>(query, variables, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AuthorizerResponse<SessionInfo>> GetSessionAsync(
        string? sessionToken = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving session information");

        const string query = @"
            query getSession($session_token: String) {
                getSession(session_token: $session_token) {
                    access_token
                    refresh_token
                    id_token
                    expires_at
                    created_at
                    session_token
                    is_valid
                    user {
                        id
                        email
                        email_verified
                        given_name
                        family_name
                        middle_name
                        name
                        nickname
                        preferred_username
                        picture
                        website
                        gender
                        birthdate
                        phone_number
                        phone_number_verified
                        created_at
                        updated_at
                        roles
                        is_active
                        signup_methods
                    }
                }
            }";

        var variables = new { session_token = sessionToken };

        return await _httpClient.PostGraphQLAsync<SessionInfo>(query, variables, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AuthorizerResponse<bool>> LogoutAsync(
        string? sessionToken = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Logging out user session");

        const string query = @"
            mutation logout($session_token: String) {
                logout(session_token: $session_token) {
                    message
                }
            }";

        var variables = new { session_token = sessionToken };

        var response = await _httpClient.PostGraphQLAsync<Dictionary<string, string>>(query, variables, cancellationToken);
        
        if (response.IsSuccess)
        {
            return AuthorizerResponse<bool>.Success(true);
        }

        return AuthorizerResponse<bool>.Failure(response.Errors ?? Array.Empty<AuthorizerError>());
    }

    #endregion

    #region Verification and Validation

    /// <inheritdoc />
    public async Task<AuthorizerResponse<bool>> VerifyEmailAsync(
        VerifyEmailRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Verifying email for: {Email}", request.Email);

        const string query = @"
            mutation verifyEmail($token: String!, $email: String!) {
                verifyEmail(params: {
                    token: $token
                    email: $email
                }) {
                    message
                }
            }";

        var variables = new
        {
            token = request.Token,
            email = request.Email
        };

        var response = await _httpClient.PostGraphQLAsync<Dictionary<string, string>>(query, variables, cancellationToken);
        
        if (response.IsSuccess)
        {
            return AuthorizerResponse<bool>.Success(true);
        }

        return AuthorizerResponse<bool>.Failure(response.Errors ?? Array.Empty<AuthorizerError>());
    }

    /// <inheritdoc />
    public async Task<AuthorizerResponse<UserProfile>> ValidateJwtAsync(
        string token, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));

        _logger.LogDebug("Validating JWT token");

        const string query = @"
            query validateJWTToken($token: String!, $token_type: String) {
                validateJWTToken(params: {
                    token: $token
                    token_type: $token_type
                }) {
                    is_valid
                    claims
                }
            }";

        var variables = new
        {
            token,
            token_type = "Bearer"
        };

        var response = await _httpClient.PostGraphQLAsync<Dictionary<string, object>>(query, variables, cancellationToken);
        
        if (response.IsSuccess && response.Data != null && response.Data.TryGetValue("is_valid", out var isValidObj) && isValidObj is bool isValid && isValid)
        {
            if (response.Data.TryGetValue("claims", out var claimsObj))
            {
                var profile = System.Text.Json.JsonSerializer.Deserialize<UserProfile>(claimsObj.ToString()!);
                return AuthorizerResponse<UserProfile>.Success(profile!);
            }
        }

        return AuthorizerResponse<UserProfile>.Failure("Invalid or expired token");
    }

    #endregion

    #region Password Management

    /// <inheritdoc />
    public async Task<AuthorizerResponse<bool>> ForgotPasswordAsync(
        string email, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));

        _logger.LogDebug("Initiating forgot password for email: {Email}", email);

        const string query = @"
            mutation forgotPassword($email: String!) {
                forgotPassword(params: {
                    email: $email
                }) {
                    message
                }
            }";

        var variables = new { email };

        var response = await _httpClient.PostGraphQLAsync<Dictionary<string, string>>(query, variables, cancellationToken);
        
        if (response.IsSuccess)
        {
            return AuthorizerResponse<bool>.Success(true);
        }

        return AuthorizerResponse<bool>.Failure(response.Errors ?? Array.Empty<AuthorizerError>());
    }

    /// <inheritdoc />
    public async Task<AuthorizerResponse<bool>> ResetPasswordAsync(
        ResetPasswordRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Resetting password using token");

        const string query = @"
            mutation resetPassword($token: String!, $password: String!, $confirm_password: String!) {
                resetPassword(params: {
                    token: $token
                    password: $password
                    confirm_password: $confirm_password
                }) {
                    message
                }
            }";

        var variables = new
        {
            token = request.Token,
            password = request.Password,
            confirm_password = request.ConfirmPassword
        };

        var response = await _httpClient.PostGraphQLAsync<Dictionary<string, string>>(query, variables, cancellationToken);
        
        if (response.IsSuccess)
        {
            return AuthorizerResponse<bool>.Success(true);
        }

        return AuthorizerResponse<bool>.Failure(response.Errors ?? Array.Empty<AuthorizerError>());
    }

    /// <inheritdoc />
    public async Task<AuthorizerResponse<bool>> ChangePasswordAsync(
        ChangePasswordRequest request, 
        CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Changing user password");

        const string query = @"
            mutation changePassword($old_password: String!, $new_password: String!, $confirm_new_password: String!, $token: String!) {
                changePassword(params: {
                    old_password: $old_password
                    new_password: $new_password
                    confirm_new_password: $confirm_new_password
                    token: $token
                }) {
                    message
                }
            }";

        var variables = new
        {
            old_password = request.OldPassword,
            new_password = request.NewPassword,
            confirm_new_password = request.ConfirmNewPassword,
            token = request.Token
        };

        var response = await _httpClient.PostGraphQLAsync<Dictionary<string, string>>(query, variables, cancellationToken);
        
        if (response.IsSuccess)
        {
            return AuthorizerResponse<bool>.Success(true);
        }

        return AuthorizerResponse<bool>.Failure(response.Errors ?? Array.Empty<AuthorizerError>());
    }

    #endregion

    #region Metadata

    /// <inheritdoc />
    public async Task<AuthorizerResponse<MetaInfo>> GetMetaAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving Authorizer metadata");

        const string query = @"
            query {
                meta {
                    version
                    client_id
                    is_signup_enabled
                    is_email_verification_enabled
                    is_basic_authentication_enabled
                    is_magic_link_login_enabled
                    is_mobile_otp_enabled
                    is_social_login_enabled
                    social_login_providers
                    is_strong_password_enabled
                    roles
                    default_roles
                    protected_routes
                    unprotected_routes
                    logout_url
                    is_multi_factor_auth_enabled
                }
            }";

        return await _httpClient.PostGraphQLAsync<MetaInfo>(query, null, cancellationToken);
    }

    #endregion
}