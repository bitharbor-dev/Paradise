using Microsoft.AspNetCore.Authentication;
using Paradise.Models;
using Paradise.WebApi.Authentication.JwtBearer;

namespace Paradise.WebApi.Tests.Unit.Authentication.JwtBearer;

/// <summary>
/// <see cref="JwtEvents"/> test class.
/// </summary>
public sealed partial class JwtEventsTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="JwtEvents.TokenValidated"/> method should
    /// succeed when refresh token validation passes.
    /// </summary>
    [Fact]
    public async Task TokenValidatedAsync()
    {
        // Arrange
        var refreshTokenValidationResult = new Result(OperationStatus.Success);

        var context = Test.GetTokenValidatedContext();

        Test.SetAuthenticationServiceCheckSessionAsyncResult(()
            => Task.FromResult(refreshTokenValidationResult));

        // Act
        await Test.Target.TokenValidated(context);

        // Assert
        Assert.Null(context.Result);
    }

    /// <summary>
    /// The <see cref="JwtEvents.TokenValidated"/> method should
    /// fail when refresh token validation does not pass.
    /// </summary>
    [Fact]
    public async Task TokenValidatedAsync_FailsOnInvalidRefreshToken()
    {
        // Arrange
        var refreshTokenValidationResult = new Result(OperationStatus.Unauthorized, ErrorCode.InvalidToken);

        var context = Test.GetTokenValidatedContext();

        Test.SetAuthenticationServiceCheckSessionAsyncResult(()
            => Task.FromResult(refreshTokenValidationResult));

        // Act
        await Test.Target.TokenValidated(context);

        // Assert
        Assert.NotNull(context.Result);
        Assert.False(context.Result.Succeeded);
        Assert.IsType<AuthenticationFailureException>(context.Result.Failure);
    }
    #endregion
}