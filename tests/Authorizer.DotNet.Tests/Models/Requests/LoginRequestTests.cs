using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Authorizer.DotNet.Models.Requests;
using Xunit;

namespace Authorizer.DotNet.Tests.Models.Requests;

/// <summary>
/// Unit tests for the LoginRequest class.
/// </summary>
public class LoginRequestTests
{
    /// <summary>
    /// Tests that valid email addresses pass validation.
    /// </summary>
    [Fact]
    public void Email_WhenValid_ShouldPassValidation()
    {
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var validationResults = ValidateModel(request);

        Assert.Empty(validationResults);
    }

    /// <summary>
    /// Tests that empty, whitespace, or null email addresses fail validation.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null!)]
    public void Email_WhenEmpty_ShouldFailValidation(string? email)
    {
        var request = new LoginRequest
        {
            Email = email!,
            Password = "password123"
        };

        var validationResults = ValidateModel(request);

        Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Email"));
    }

    /// <summary>
    /// Tests that email addresses with invalid format fail validation.
    /// </summary>
    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test.example.com")]
    public void Email_WhenInvalidFormat_ShouldFailValidation(string email)
    {
        var request = new LoginRequest
        {
            Email = email,
            Password = "password123"
        };

        var validationResults = ValidateModel(request);

        Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Email"));
    }

    /// <summary>
    /// Tests that empty, whitespace, or null passwords fail validation.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null!)]
    public void Password_WhenEmpty_ShouldFailValidation(string? password)
    {
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = password!
        };

        var validationResults = ValidateModel(request);

        Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Password"));
    }

    /// <summary>
    /// Tests that optional fields can be null without failing validation.
    /// </summary>
    [Fact]
    public void OptionalFields_WhenNull_ShouldPassValidation()
    {
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123",
            Scope = null,
            State = null,
            Roles = null
        };

        var validationResults = ValidateModel(request);

        Assert.Empty(validationResults);
    }

    /// <summary>
    /// Tests that IsRememberMe property defaults to false.
    /// </summary>
    [Fact]
    public void IsRememberMe_DefaultValue_ShouldBeFalse()
    {
        var request = new LoginRequest();

        Assert.False(request.IsRememberMe);
    }

    /// <summary>
    /// Tests that IsRememberMe property can be set to true.
    /// </summary>
    [Fact]
    public void IsRememberMe_WhenSetToTrue_ShouldReturnTrue()
    {
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123",
            IsRememberMe = true
        };

        Assert.True(request.IsRememberMe);
    }

    /// <summary>
    /// Tests that Roles property retains provided values.
    /// </summary>
    [Fact]
    public void Roles_WhenProvided_ShouldRetainValues()
    {
        var roles = new List<string> { "admin", "user" };
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123",
            Roles = roles
        };

        Assert.Equal(roles, request.Roles);
    }

    /// <summary>
    /// Tests that Scope property retains provided values.
    /// </summary>
    [Theory]
    [InlineData("openid profile")]
    [InlineData("read write")]
    [InlineData("")]
    public void Scope_WhenProvided_ShouldRetainValue(string scope)
    {
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123",
            Scope = scope
        };

        Assert.Equal(scope, request.Scope);
    }

    /// <summary>
    /// Tests that State property retains provided values.
    /// </summary>
    [Theory]
    [InlineData("state123")]
    [InlineData("random-state-value")]
    [InlineData("")]
    public void State_WhenProvided_ShouldRetainValue(string state)
    {
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123",
            State = state
        };

        Assert.Equal(state, request.State);
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}