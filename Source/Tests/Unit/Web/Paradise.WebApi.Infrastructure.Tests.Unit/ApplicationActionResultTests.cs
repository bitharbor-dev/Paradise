using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paradise.WebApi.Infrastructure.Extensions;
using System.Text.Json;

namespace Paradise.WebApi.Infrastructure.Tests.Unit;

/// <summary>
/// <see cref="ApplicationActionResult"/> test class.
/// </summary>
public sealed partial class ApplicationActionResultTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="ExecuteResultAsync"/> method.
    /// </summary>
    public static TheoryData<bool> ExecuteResultAsync_MemberData { get; } = new()
    {
        { true  },
        { false }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="ApplicationActionResult.ExecuteResultAsync"/> method should
    /// write JSON content, set the status code and complete the response body.
    /// </summary>
    /// <param name="useNullSerializerOptions">
    /// Indicates whether the default or custom <see cref="JsonSerializerOptions"/>
    /// should be used during the <see cref="ApplicationActionResult.ExecuteResultAsync"/>
    /// method call.
    /// </param>
    [Theory, MemberData(nameof(ExecuteResultAsync_MemberData))]
    public async Task ExecuteResultAsync(bool useNullSerializerOptions)
    {
        // Arrange
        if (useNullSerializerOptions)
            Test.Options = null;

        var context = Test.CreateContext();

        // Act
        await Test.Target.ExecuteResultAsync(context);

        // Assert
        Assert.Equal((int)Test.Result.Status.GetStatusCode(), context.HttpContext.Response.StatusCode);
        Assert.True(context.HttpContext.Response.Body.Length > 0);
    }

    /// <summary>
    /// The <see cref="ApplicationActionResult.ExecuteResultAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="ActionContext"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task ExecuteResultAsync_ThrowsOnNull()
    {
        // Arrange
        var actionContext = null as ActionContext;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => Test.Target.ExecuteResultAsync(actionContext!));
    }

    /// <summary>
    /// The <see cref="ApplicationActionResult.WriteResponseContentAsync"/> method should
    /// write JSON content and complete the response when it has not started.
    /// </summary>
    [Fact]
    public async Task WriteResponseContentAsync()
    {
        // Arrange
        var context = Test.CreateContext();

        // Act
        await Test.Target.WriteResponseContentAsync(context.HttpContext.Response);

        // Assert
        Assert.Equal((int)Test.Result.Status.GetStatusCode(), context.HttpContext.Response.StatusCode);
        Assert.True(context.HttpContext.Response.Body.Length > 0);
    }

    /// <summary>
    /// The <see cref="ApplicationActionResult.WriteResponseContentAsync"/> method should
    /// not write JSON content and complete the response when it has started.
    /// </summary>
    [Fact]
    public async Task WriteResponseContentAsync_SkipsStartedResponse()
    {
        // Arrange
        var context = Test.CreateContext();
        var response = context.HttpContext.Response;

        var expectedStatusCode = response.StatusCode;
        var expectedResponseLength = response.Body.Length;

        response.OnStarting(() => Task.CompletedTask);

        // Act
        await Test.Target.WriteResponseContentAsync(response);

        // Assert
        Assert.Equal(expectedStatusCode, response.StatusCode);
        Assert.Equal(expectedResponseLength, response.Body.Length);
    }

    /// <summary>
    /// The <see cref="ApplicationActionResult.WriteResponseContentAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="HttpResponse"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task WriteResponseContentAsync_ThrowsOnNull()
    {
        // Arrange
        var response = null as HttpResponse;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => Test.Target.WriteResponseContentAsync(response!));
    }
    #endregion
}