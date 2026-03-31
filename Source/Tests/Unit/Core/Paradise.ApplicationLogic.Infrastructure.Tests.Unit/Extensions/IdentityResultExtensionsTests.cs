using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Infrastructure.Extensions;
using Paradise.Models;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Extensions;

/// <summary>
/// <see cref="IdentityResultExtensions"/> test class.
/// </summary>
public sealed class IdentityResultExtensionsTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="IdentityResultExtensions.GetResult"/> method should
    /// return the <see cref="Result"/> instance representing the input <see cref="IdentityResult"/>.
    /// </summary>
    [Fact]
    public void GetResult()
    {
        // Arrange
        var error1 = new IdentityError() { Description = "Error 1" };
        var error2 = new IdentityError() { Description = "Error 2" };
        var identityResult = IdentityResult.Failed(error1, error2);
        var status = OperationStatus.Failure;

        // Act
        var result = identityResult.GetResult(status);

        // Assert
        Assert.Equal(status, result.Status);
        Assert.All(result.Errors, error => Assert.Equal(ErrorCode.DefaultError, error.Code));

        Assert.Collection(result.Errors, error => Assert.Equal(error1.Description, error.Description),
                                         error => Assert.Equal(error2.Description, error.Description));
    }

    /// <summary>
    /// The <see cref="IdentityResultExtensions.GetResult"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="IdentityResult"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void GetResult_ThrowsOnNull()
    {
        // Arrange
        var identityResult = null as IdentityResult;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => identityResult!.GetResult());
    }

    /// <summary>
    /// The <see cref="IdentityResultExtensions.GetResult{TValue}"/> method should
    /// return the <see cref="Result{TValue}"/> instance representing the input <see cref="IdentityResult"/>.
    /// </summary>
    [Fact]
    public void GetResult_WithValue()
    {
        // Arrange
        var error1 = new IdentityError() { Description = "Error 1" };
        var error2 = new IdentityError() { Description = "Error 2" };
        var identityResult = IdentityResult.Failed(error1, error2);
        var status = OperationStatus.Failure;

        // Act
        var result = identityResult.GetResult<object>(status);

        // Assert
        Assert.Null(result.Value);
        Assert.Equal(status, result.Status);
        Assert.All(result.Errors, error => Assert.Equal(ErrorCode.DefaultError, error.Code));

        Assert.Collection(result.Errors, error => Assert.Equal(error1.Description, error.Description),
                                         error => Assert.Equal(error2.Description, error.Description));
    }

    /// <summary>
    /// The <see cref="IdentityResultExtensions.GetResult{TValue}"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="IdentityResult"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void GetResult_WithValue_ThrowsOnNull()
    {
        // Arrange
        var identityResult = null as IdentityResult;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => identityResult!.GetResult<object>());
    }
    #endregion
}