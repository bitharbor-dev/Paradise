using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Paradise.Models;
using Paradise.WebApi.Authentication.JwtBearer;
using Paradise.WebApi.Authorization;

namespace Paradise.WebApi.Tests.Unit.Authorization;

/// <summary>
/// <see cref="AuthorizationResultHandler"/> test class.
/// </summary>
public sealed partial class AuthorizationResultHandlerTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="AuthorizationResultHandler.HandleAsync"/> method should
    /// pass the authorization flow to the framework implementation of the handler if the input
    /// <see cref="PolicyAuthorizationResult"/> is not challenged or forbidden.
    /// </summary>
    [Fact]
    public async Task HandleAsync()
    {
        // Arrange
        var context = new DefaultHttpContext()
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var authorizationResult = PolicyAuthorizationResult.Success();

        // Act
        await Test.Target.HandleAsync(Test.Next, context, Test.Policy, authorizationResult);

        // Assert
        Assert.True(Test.HandlingDelegated);
    }

    /// <summary>
    /// The <see cref="AuthorizationResultHandler.HandleAsync"/> method should
    /// handle the challenged <see cref="PolicyAuthorizationResult"/>
    /// by writing the corresponding HTTP response.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Challenged()
    {
        // Arrange
        var context = new DefaultHttpContext()
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var authorizationResult = PolicyAuthorizationResult.Challenge();

        // Act
        await Test.Target.HandleAsync(Test.Next, context, Test.Policy, authorizationResult);

        // Assert
        Assert.Equal(401, context.Response.StatusCode);
        Assert.False(Test.HandlingDelegated);
    }

    /// <summary>
    /// The <see cref="AuthorizationResultHandler.HandleAsync"/> method should
    /// handle the challenged <see cref="PolicyAuthorizationResult"/>
    /// by writing the corresponding HTTP response using the <see cref="Result"/>
    /// provided via <see cref="HttpContext.Items"/>.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Challenged_WithCustomResult()
    {
        // Arrange
        var result = new Result(OperationStatus.Unauthorized, ErrorCode.InvalidToken);
        var context = new DefaultHttpContext()
        {
            Items = new Dictionary<object, object?>
            {
                [JwtEvents.JwtEventsSessionCheckResult] = result
            },
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var authorizationResult = PolicyAuthorizationResult.Challenge();

        // Act
        await Test.Target.HandleAsync(Test.Next, context, Test.Policy, authorizationResult);

        // Assert
        Assert.Equal(401, context.Response.StatusCode);
        Assert.False(Test.HandlingDelegated);
    }

    /// <summary>
    /// The <see cref="AuthorizationResultHandler.HandleAsync"/> method should
    /// handle the forbidden <see cref="PolicyAuthorizationResult"/>
    /// by writing the corresponding HTTP response.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Forbidden()
    {
        // Arrange
        var context = new DefaultHttpContext()
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var authorizationResult = PolicyAuthorizationResult.Forbid();

        // Act
        await Test.Target.HandleAsync(Test.Next, context, Test.Policy, authorizationResult);

        // Assert
        Assert.Equal(403, context.Response.StatusCode);
        Assert.False(Test.HandlingDelegated);
    }
    #endregion
}