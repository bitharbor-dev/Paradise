using Microsoft.AspNetCore.Mvc.ModelBinding;
using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.Models;
using Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.ApplicationLogic.Services.Identity.Users;
using Paradise.WebApi.Pages;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.WebApi.Tests.Unit.Pages;

/// <summary>
/// <see cref="ResetEmailAddressModel"/> test class.
/// </summary>
public sealed class ResetEmailAddressModelTests
{
    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    internal ResetEmailAddressModel Model { get; } = new();
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="ResetEmailAddressModel.OnGetAsync"/> method should
    /// set <see cref="ResetEmailAddressModel.IsSuccess"/> to <see langword="true"/>
    /// when the email reset succeeds.
    /// </summary>
    [Fact]
    public async Task OnGetAsync()
    {
        // Arrange
        var result = new Result(Success);

        var userService = GetUserService(() =>
            Task.FromResult(result));

        // Act
        await Model.OnGetAsync(userService);

        // Assert
        Assert.True(Model.IsSuccess);
        Assert.Null(Model.ErrorMessage);
    }

    /// <summary>
    /// The <see cref="ResetEmailAddressModel.OnGetAsync"/> method should
    /// return early when <see cref="ModelStateDictionary.IsValid"/>
    /// is equal to <see langword="false"/>.
    /// </summary>
    [Fact]
    public async Task OnGetAsync_ReturnsOnInvalidModelState()
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
        await Model.OnGetAsync(userService);

        // Assert
        Assert.False(serviceInvoked);
        Assert.False(Model.IsSuccess);
        Assert.Null(Model.ErrorMessage);
    }

    /// <summary>
    /// The <see cref="ResetEmailAddressModel.OnGetAsync"/> method should
    /// set <see cref="ResetEmailAddressModel.ErrorMessage"/> if the
    /// inner <see cref="IUserService"/> instance reports error
    /// upon resetting user email address.
    /// </summary>
    [Fact]
    public async Task OnGetAsync_FailsOnEmailResetFailure()
    {
        // Arrange
        var result = new Result(Failure, InvalidToken);
        var expectedMessage = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));

        var userService = GetUserService(() =>
            Task.FromResult(result));

        // Act
        await Model.OnGetAsync(userService);

        // Assert
        Assert.False(Model.IsSuccess);
        Assert.Equal(expectedMessage, Model.ErrorMessage);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates a <see cref="StubUserService"/> configured to return a predefined result
    /// from <see cref="IUserService.ResetEmailAddressAsync"/>.
    /// </summary>
    /// <param name="resetEmailAddressAsyncResult">
    /// The delegate that produces the result of the email reset operation.
    /// </param>
    /// <returns>
    /// A configured <see cref="StubUserService"/> instance.
    /// </returns>
    private static StubUserService GetUserService(Func<Task<Result>> resetEmailAddressAsyncResult) => new()
    {
        ResetEmailAddressAsyncResult = resetEmailAddressAsyncResult
    };
    #endregion
}