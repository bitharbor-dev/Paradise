using Microsoft.AspNetCore.Mvc.ModelBinding;
using Paradise.ApplicationLogic.Services.Identity.Users;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Users;
using Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.ApplicationLogic.Services.Identity.Users;
using Paradise.WebApi.Pages;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.WebApi.Tests.Unit.Pages;

/// <summary>
/// <see cref="ConfirmEmailAddressModel"/> test class.
/// </summary>
public sealed class ConfirmEmailAddressModelTests
{
    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    internal ConfirmEmailAddressModel Model { get; } = new();
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="ConfirmEmailAddressModel.OnGetAsync"/> method should
    /// set <see cref="ConfirmEmailAddressModel.IsSuccess"/> to <see langword="true"/>
    /// when the confirmation succeeds.
    /// </summary>
    [Fact]
    public async Task OnGetAsync()
    {
        // Arrange
        var result = new Result<UserModel>(GetDefaultUserModel(), Success);

        var userService = GetUserService(()
            => Task.FromResult(result));

        // Act
        await Model.OnGetAsync(userService);

        // Assert
        Assert.True(Model.IsSuccess);
        Assert.Null(Model.ErrorMessage);
    }

    /// <summary>
    /// The <see cref="ConfirmEmailAddressModel.OnGetAsync"/> method should
    /// return early when <see cref="ModelStateDictionary.IsValid"/>
    /// is equal to <see langword="false"/>.
    /// </summary>
    [Fact]
    public async Task OnGetAsync_ReturnsOnInvalidModelState()
    {
        // Arrange
        Model.ModelState.AddModelError("Key", "ErrorMessage");

        var result = new Result<UserModel>(null, InvalidInput);

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
    /// The <see cref="ConfirmEmailAddressModel.OnGetAsync"/> method should
    /// set <see cref="ResetEmailAddressModel.ErrorMessage"/> if the
    /// inner <see cref="IUserService"/> instance reports error
    /// upon confirming user email address.
    /// </summary>
    [Fact]
    public async Task OnGetAsync_FailsOnEmailConfirmationFailure()
    {
        // Arrange
        var result = new Result<UserModel>(Failure, InvalidToken);
        var expectedMessage = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));

        var userService = GetUserService(()
            => Task.FromResult(result));

        // Act
        await Model.OnGetAsync(userService);

        // Assert
        Assert.False(Model.IsSuccess);
        Assert.Equal(expectedMessage, Model.ErrorMessage);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates a default <see cref="UserModel"/> instance for testing purposes.
    /// </summary>
    /// <remarks>
    /// Uses default values for all fields as the actual data is not relevant
    /// for email confirmation behavior.
    /// </remarks>
    /// <returns>
    /// A <see cref="UserModel"/> instance.
    /// </returns>
    private static UserModel GetDefaultUserModel()
        => new(default, default!, default!, default, default, default, default, default, default);

    /// <summary>
    /// Creates a <see cref="StubUserService"/> configured to return a predefined result
    /// from <see cref="IUserService.ConfirmEmailAddressAsync"/>.
    /// </summary>
    /// <param name="confirmEmailAddressAsyncResult">
    /// The delegate that produces the result of the email confirmation operation.
    /// </param>
    /// <returns>
    /// A configured <see cref="StubUserService"/> instance.
    /// </returns>
    private static StubUserService GetUserService(Func<Task<Result<UserModel>>> confirmEmailAddressAsyncResult) => new()
    {
        ConfirmEmailAddressAsyncResult = confirmEmailAddressAsyncResult
    };
    #endregion
}