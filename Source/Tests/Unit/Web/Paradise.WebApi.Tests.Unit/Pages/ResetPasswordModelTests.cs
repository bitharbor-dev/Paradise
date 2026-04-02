using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.Models;
using Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.ApplicationLogic.Services.Identity.Users;
using Paradise.WebApi.Pages;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.WebApi.Tests.Unit.Pages;

/// <summary>
/// <see cref="ResetPasswordModel"/> test class.
/// </summary>
public sealed class ResetPasswordModelTests
{
    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    internal ResetPasswordModel Model { get; } = new();
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="ResetPasswordModel.OnPostAsync"/> method should
    /// return <see cref="PageResult"/> and
    /// set <see cref="ResetPasswordModel.IsSuccess"/> to <see langword="true"/>
    /// when the password reset succeeds.
    /// </summary>
    [Fact]
    public async Task OnPostAsync()
    {
        // Arrange
        var result = new Result(Success);

        var userService = GetUserService(()
            => Task.FromResult(result));

        // Act
        var actionResult = await Model.OnPostAsync(userService);

        // Assert
        Assert.IsType<PageResult>(actionResult);

        Assert.True(Model.IsSuccess);
        Assert.Null(Model.ErrorMessage);
    }

    /// <summary>
    /// The <see cref="ResetPasswordModel.OnPostAsync"/> method should
    /// return early when <see cref="ModelStateDictionary.IsValid"/>
    /// is equal to <see langword="false"/>.
    /// </summary>
    [Fact]
    public async Task OnPostAsync_ReturnsOnInvalidModelState()
    {
        // Arrange
        Model.ModelState.AddModelError("Key", "ErrorMessage");

        var result = new Result(InvalidInput);

        var serviceInvoked = false;
        var userService = GetUserService(() =>
        {
            serviceInvoked = true;
            return Task.FromResult(result);
        });

        // Act
        var actionResult = await Model.OnPostAsync(userService);

        // Assert
        Assert.IsType<PageResult>(actionResult);

        Assert.False(serviceInvoked);
        Assert.False(Model.IsSuccess);
        Assert.Null(Model.ErrorMessage);
    }

    /// <summary>
    /// The <see cref="ResetPasswordModel.OnPostAsync"/> method should
    /// set <see cref="ResetPasswordModel.ErrorMessage"/> and return
    /// <see cref="PageResult"/> when the password reset fails.
    /// </summary>
    [Fact]
    public async Task OnPostAsync_FailsOnPasswordResetFailure()
    {
        // Arrange
        var result = new Result(Failure, InvalidToken);
        var expectedMessage = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));

        var userService = GetUserService(() =>
            Task.FromResult(result));

        // Act
        var actionResult = await Model.OnPostAsync(userService);

        // Assert
        Assert.IsType<PageResult>(actionResult);

        Assert.False(Model.IsSuccess);
        Assert.Equal(expectedMessage, Model.ErrorMessage);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates a <see cref="StubUserService"/> configured to return a predefined result
    /// from <see cref="IUserService.ResetPasswordAsync"/>.
    /// </summary>
    /// <param name="resetPasswordAsyncResult">
    /// The delegate that processes the password reset operation.
    /// </param>
    /// <returns>
    /// A configured <see cref="StubUserService"/> instance.
    /// </returns>
    private static StubUserService GetUserService(Func<Task<Result>> resetPasswordAsyncResult) => new()
    {
        ResetPasswordAsyncResult = resetPasswordAsyncResult
    };
    #endregion
}