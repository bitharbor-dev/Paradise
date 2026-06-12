using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;
using TestResult = Paradise.Tests.Fixtures.Common.Models.TestResult;

namespace Paradise.Models.Tests.Unit;

/// <summary>
/// <see cref="ResultBase"/> test class.
/// </summary>
public sealed class ResultBaseTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="ResultBase()"/> constructor should
    /// initialize an empty successful result.
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // Arrange

        // Act
        var result = new TestResult();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Success, result.Status);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// The <see cref="ResultBase(OperationStatus)"/> constructor should
    /// initialize an empty result with the specified status.
    /// </summary>
    [Fact]
    public void Constructor_WithStatus()
    {
        // Arrange
        var status = Created;

        // Act
        var result = new TestResult(status);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(status, result.Status);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// The <see cref="ResultBase(OperationStatus, ErrorCode, object[])"/> constructor should
    /// initialize a non empty result with the specified status
    /// containing one <see cref="ApplicationError"/> generated
    /// from the specified <see cref="ErrorCode"/>.
    /// </summary>
    [Fact]
    public void Constructor_WithStatusAndError()
    {
        // Arrange
        var status = Failure;
        var errorCode = DefaultError;

        // Act
        var result = new TestResult(status, errorCode);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(status, result.Status);

        var error = Assert.Single(result.Errors);
        Assert.Equal(errorCode, error.Code);
    }

    /// <summary>
    /// The <see cref="ResultBase(OperationStatus, ErrorCode, object[])"/> constructor should
    /// initialize a non empty result with the specified status
    /// containing the provided list of errors.
    /// </summary>
    [Fact]
    public void Constructor_WithStatusAndErrors()
    {
        // Arrange
        var status = InvalidInput;
        var errors = new[]
        {
            new ApplicationError(InvalidUserName, "Invalid user name"),
            new ApplicationError(InvalidEmailAddress, "Invalid email address")
        };

        // Act
        var result = new TestResult(status, errors);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(status, result.Status);

        Assert.Collection(result.Errors, error => Assert.Equivalent(errors[0], error),
                                         error => Assert.Equivalent(errors[1], error));
    }

    /// <summary>
    /// The <see cref="ResultBase.this[int]"/> indexer should
    /// return the exact stored error.
    /// </summary>
    [Fact]
    public void Indexer()
    {
        // Arrange
        var status = Failure;
        var expectedError = new ApplicationError(DefaultError, "Error");

        var result = new TestResult(status, [expectedError]);

        // Act
        var actualError = result[0];

        // Assert
        Assert.Equivalent(expectedError, actualError);
    }

    /// <summary>
    /// The <see cref="ResultBase.IsSuccess"/> property should
    /// return <see langword="true"/> if the result instance
    /// does not contain any errors.
    /// </summary>
    [Fact]
    public void IsSuccess_ReturnsTrue()
    {
        // Arrange
        var result = new TestResult();

        // Act
        var isSuccess = result.IsSuccess;

        // Assert
        Assert.True(isSuccess);
    }

    /// <summary>
    /// The <see cref="ResultBase.IsSuccess"/> property should
    /// return <see langword="false"/> if the result instance
    /// contains any errors.
    /// </summary>
    [Fact]
    public void IsSuccess_ReturnsFalse()
    {
        // Arrange
        var status = Failure;
        var error = new ApplicationError(DefaultError, "Error");

        var result = new TestResult(status, [error]);

        // Act
        var isSuccess = result.IsSuccess;

        // Assert
        Assert.False(isSuccess);
    }

    /// <summary>
    /// The <see cref="ResultBase.AddError(ErrorCode, object?[])"/> method should
    /// initialize a single <see cref="ApplicationError"/> and
    /// add it to the <see cref="ResultBase.Errors"/>.
    /// </summary>
    [Fact]
    public void AddError_WithErrorCode()
    {
        // Arrange
        var result = new TestResult();

        // Act
        result.AddError(DefaultError);

        // Assert
        var error = Assert.Single(result.Errors);
        Assert.Equal(DefaultError, error.Code);
    }

    /// <summary>
    /// The <see cref="ResultBase.AddError(ApplicationError)"/> method should
    /// add the specified error to the <see cref="ResultBase.Errors"/>.
    /// </summary>
    [Fact]
    public void AddError_WithApplicationError()
    {
        // Arrange
        var expectedError = new ApplicationError(DefaultError, "Error");
        var result = new TestResult();

        // Act
        result.AddError(expectedError);

        // Assert
        var actualError = Assert.Single(result.Errors);
        Assert.Equivalent(expectedError, actualError);
    }

    /// <summary>
    /// The <see cref="ResultBase.AddErrors"/> method should
    /// add the list of specified errors to the <see cref="ResultBase.Errors"/>.
    /// </summary>
    [Fact]
    public void AddErrors()
    {
        // Arrange
        var errors = new[]
        {
            new ApplicationError(InvalidUserName, "Invalid user name"),
            new ApplicationError(InvalidEmailAddress, "Invalid email address")
        };

        var result = new TestResult();

        // Act
        result.AddErrors(errors);

        // Assert
        Assert.Collection(result.Errors, error => Assert.Equivalent(errors[0], error),
                                         error => Assert.Equivalent(errors[1], error));
    }
    #endregion
}