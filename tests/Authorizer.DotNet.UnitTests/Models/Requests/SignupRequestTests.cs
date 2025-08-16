using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Authorizer.DotNet.Models.Requests;
using Xunit;

namespace Authorizer.DotNet.Tests.Models.Requests;

/// <summary>
/// Unit tests for the SignupRequest class.
/// </summary>
public class SignupRequestTests
{
    /// <summary>
    /// Tests that SignupRequest with valid data passes validation.
    /// </summary>
    [Fact]
    public void SignupRequest_WithValidData_ShouldPassValidation()
    {
        var request = new SignupRequest
        {
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
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
        var request = new SignupRequest
        {
            Email = email!,
            Password = "password123",
            ConfirmPassword = "password123"
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
    public void Email_WhenInvalidFormat_ShouldFailValidation(string email)
    {
        var request = new SignupRequest
        {
            Email = email,
            Password = "password123",
            ConfirmPassword = "password123"
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
        var request = new SignupRequest
        {
            Email = "test@example.com",
            Password = password!,
            ConfirmPassword = "password123"
        };

        var validationResults = ValidateModel(request);

        Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Password"));
    }

    /// <summary>
    /// Tests that empty, whitespace, or null confirm passwords fail validation.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null!)]
    public void ConfirmPassword_WhenEmpty_ShouldFailValidation(string? confirmPassword)
    {
        var request = new SignupRequest
        {
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = confirmPassword!
        };

        var validationResults = ValidateModel(request);

        Assert.Contains(validationResults, vr => vr.MemberNames.Contains("ConfirmPassword"));
    }

    /// <summary>
    /// Tests that optional fields can be null without failing validation.
    /// </summary>
    [Fact]
    public void OptionalFields_WhenNull_ShouldPassValidation()
    {
        var request = new SignupRequest
        {
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            GivenName = null,
            FamilyName = null,
            Roles = null
        };

        var validationResults = ValidateModel(request);

        Assert.Empty(validationResults);
    }

    /// <summary>
    /// Tests that all provided field values are retained correctly.
    /// </summary>
    [Fact]
    public void AllFields_WhenProvided_ShouldRetainValues()
    {
        var roles = new List<string> { "user", "admin" };
        var request = new SignupRequest
        {
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123",
            GivenName = "John",
            FamilyName = "Doe",
            MiddleName = "Middle",
            Nickname = "Johnny",
            PreferredUsername = "johndoe",
            PhoneNumber = "+1234567890",
            Picture = "https://example.com/photo.jpg",
            Birthdate = "1990-01-01",
            Gender = "male",
            Scope = "openid profile",
            State = "random-state",
            Roles = roles,
            RedirectUri = "https://app.example.com/callback"
        };

        Assert.Equal("test@example.com", request.Email);
        Assert.Equal("password123", request.Password);
        Assert.Equal("password123", request.ConfirmPassword);
        Assert.Equal("John", request.GivenName);
        Assert.Equal("Doe", request.FamilyName);
        Assert.Equal("Middle", request.MiddleName);
        Assert.Equal("Johnny", request.Nickname);
        Assert.Equal("johndoe", request.PreferredUsername);
        Assert.Equal("+1234567890", request.PhoneNumber);
        Assert.Equal("https://example.com/photo.jpg", request.Picture);
        Assert.Equal("1990-01-01", request.Birthdate);
        Assert.Equal("male", request.Gender);
        Assert.Equal("openid profile", request.Scope);
        Assert.Equal("random-state", request.State);
        Assert.Equal(roles, request.Roles);
        Assert.Equal("https://app.example.com/callback", request.RedirectUri);
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}