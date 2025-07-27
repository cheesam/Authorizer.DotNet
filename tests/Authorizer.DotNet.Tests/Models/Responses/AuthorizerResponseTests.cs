using System.Collections.Generic;
using Authorizer.DotNet.Models.Common;
using Authorizer.DotNet.Models.Responses;
using Xunit;

namespace Authorizer.DotNet.Tests.Models.Responses;

/// <summary>
/// Unit tests for the AuthorizerResponse class.
/// </summary>
public class AuthorizerResponseTests
{
    /// <summary>
    /// Tests that Success method creates a response with data and no errors.
    /// </summary>
    [Fact]
    public void Success_ShouldCreateResponseWithData()
    {
        var testData = "test data";
        
        var response = AuthorizerResponse<string>.Success(testData);
        
        Assert.Equal(testData, response.Data);
        Assert.Null(response.Errors);
        Assert.True(response.IsSuccess);
        Assert.False(response.HasErrors);
        Assert.Null(response.FirstErrorMessage);
    }

    /// <summary>
    /// Tests that Failure method with error list creates a response with errors.
    /// </summary>
    [Fact]
    public void Failure_WithErrors_ShouldCreateResponseWithErrors()
    {
        var errors = new List<AuthorizerError>
        {
            new() { Message = "Error 1", Code = "E001" },
            new() { Message = "Error 2", Code = "E002" }
        };
        
        var response = AuthorizerResponse<string>.Failure(errors);
        
        Assert.Null(response.Data);
        Assert.Equal(errors, response.Errors);
        Assert.False(response.IsSuccess);
        Assert.True(response.HasErrors);
        Assert.Equal("Error 1", response.FirstErrorMessage);
    }

    /// <summary>
    /// Tests that Failure method with single error creates a response with one error.
    /// </summary>
    [Fact]
    public void Failure_WithSingleError_ShouldCreateResponseWithSingleError()
    {
        var error = new AuthorizerError { Message = "Single error", Code = "E001" };
        
        var response = AuthorizerResponse<string>.Failure(error);
        
        Assert.Null(response.Data);
        Assert.Single(response.Errors!);
        Assert.Equal(error, response.Errors![0]);
        Assert.False(response.IsSuccess);
        Assert.True(response.HasErrors);
        Assert.Equal("Single error", response.FirstErrorMessage);
    }

    /// <summary>
    /// Tests that Failure method with message and code creates a response with error.
    /// </summary>
    [Fact]
    public void Failure_WithMessage_ShouldCreateResponseWithErrorMessage()
    {
        var message = "Error message";
        var code = "E001";
        
        var response = AuthorizerResponse<string>.Failure(message, code);
        
        Assert.Null(response.Data);
        Assert.Single(response.Errors!);
        Assert.Equal(message, response.Errors![0].Message);
        Assert.Equal(code, response.Errors![0].Code);
        Assert.False(response.IsSuccess);
        Assert.True(response.HasErrors);
        Assert.Equal(message, response.FirstErrorMessage);
    }

    /// <summary>
    /// Tests that Failure method with message only creates a response with null error code.
    /// </summary>
    [Fact]
    public void Failure_WithMessageOnly_ShouldCreateResponseWithNullCode()
    {
        var message = "Error message";
        
        var response = AuthorizerResponse<string>.Failure(message);
        
        Assert.Null(response.Data);
        Assert.Single(response.Errors!);
        Assert.Equal(message, response.Errors![0].Message);
        Assert.Null(response.Errors![0].Code);
        Assert.False(response.IsSuccess);
        Assert.True(response.HasErrors);
        Assert.Equal(message, response.FirstErrorMessage);
    }

    /// <summary>
    /// Tests that IsSuccess returns true when errors collection is empty.
    /// </summary>
    [Fact]
    public void IsSuccess_WithEmptyErrors_ShouldReturnTrue()
    {
        var response = new AuthorizerResponse<string>
        {
            Data = "test",
            Errors = new List<AuthorizerError>()
        };
        
        Assert.True(response.IsSuccess);
        Assert.False(response.HasErrors);
    }

    /// <summary>
    /// Tests that FirstErrorMessage returns null when there are no errors.
    /// </summary>
    [Fact]
    public void FirstErrorMessage_WithNoErrors_ShouldReturnNull()
    {
        var response = AuthorizerResponse<string>.Success("test");
        
        Assert.Null(response.FirstErrorMessage);
    }

    /// <summary>
    /// Tests that FirstErrorMessage returns null when errors collection is empty.
    /// </summary>
    [Fact]
    public void FirstErrorMessage_WithEmptyErrors_ShouldReturnNull()
    {
        var response = new AuthorizerResponse<string>
        {
            Errors = new List<AuthorizerError>()
        };
        
        Assert.Null(response.FirstErrorMessage);
    }
}