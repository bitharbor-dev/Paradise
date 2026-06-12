using Microsoft.AspNetCore.Http.HttpResults;
using Paradise.Models;
using Paradise.WebApi.Infrastructure.Extensions;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Extensions;

/// <summary>
/// <see cref="ResultBaseExtensions"/> test class.
/// </summary>
public sealed class ResultBaseExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for the <see cref="AsHttpResultAsync_ReturnsProblem"/> method.
    /// </summary>
    public static TheoryData<OperationStatus> AsHttpResultAsync_ReturnsProblem_MemberData { get; } = new()
    {
        { OperationStatus.InvalidInput  },
        { OperationStatus.Unauthorized  },
        { OperationStatus.Prohibited    },
        { OperationStatus.Missing       },
        { OperationStatus.Blocked       },
        { OperationStatus.Failure       }
    };

    /// <summary>
    /// Provides member data for the <see cref="AsHttpResultAsyncGeneric_ReturnsProblem"/> method.
    /// </summary>
    public static TheoryData<OperationStatus> AsHttpResultAsyncGeneric_ReturnsProblem_MemberData { get; } = new()
    {
        { OperationStatus.InvalidInput  },
        { OperationStatus.Unauthorized  },
        { OperationStatus.Prohibited    },
        { OperationStatus.Missing       },
        { OperationStatus.Blocked       },
        { OperationStatus.Failure       }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="ResultBaseExtensions.AsHttpResultAsync(Task{Result})"/> method should
    /// return an <see cref="Ok"/> result for a successful operation.
    /// </summary>
    [Fact]
    public async Task AsHttpResultAsync_ReturnsOk()
    {
        // Arrange
        var task = CreateResultingTask(OperationStatus.Success);

        // Act
        var result = await task.AsHttpResultAsync();

        // Assert
        Assert.IsType<Ok>(result);
    }

    /// <summary>
    /// The <see cref="ResultBaseExtensions.AsHttpResultAsync(Task{Result})"/> method should
    /// return a <see cref="Created"/> result for a created operation.
    /// </summary>
    [Fact]
    public async Task AsHttpResultAsync_ReturnsCreated()
    {
        // Arrange
        var task = CreateResultingTask(OperationStatus.Created);

        // Act
        var result = await task.AsHttpResultAsync();

        // Assert
        var createdResult = Assert.IsType<Created>(result);

        Assert.Null(createdResult.Location);
    }

    /// <summary>
    /// The <see cref="ResultBaseExtensions.AsHttpResultAsync(Task{Result})"/> method should
    /// return an <see cref="Accepted"/> result for an accepted operation.
    /// </summary>
    [Fact]
    public async Task AsHttpResultAsync_ReturnsAccepted()
    {
        // Arrange
        var task = CreateResultingTask(OperationStatus.Received);

        // Act
        var result = await task.AsHttpResultAsync();

        // Assert
        var acceptedResult = Assert.IsType<Accepted>(result);

        Assert.Null(acceptedResult.Location);
    }

    /// <summary>
    /// The <see cref="ResultBaseExtensions.AsHttpResultAsync(Task{Result})"/> method should
    /// return a <see cref="ProblemHttpResult"/> for a request resulted in an error.
    /// </summary>
    [Theory, MemberData(nameof(AsHttpResultAsync_ReturnsProblem_MemberData))]
    public async Task AsHttpResultAsync_ReturnsProblem(OperationStatus status)
    {
        // Arrange
        var errors = new[]
        {
            new ApplicationError(ErrorCode.DefaultError, "Error 1"),
            new ApplicationError(ErrorCode.DefaultError, "Error 2")
        };

        var task = CreateResultingTask(status, errors: errors);

        // Act
        var result = await task.AsHttpResultAsync();

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        var details = Assert.IsType<ApplicationProblemDetails>(problemResult.ProblemDetails);

        Assert.Equal(errors, details.Errors);
    }

    /// <summary>
    /// The <see cref="ResultBaseExtensions.AsHttpResultAsync(Task{Result})"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="Task{T}"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task AsHttpResultAsync_ThrowsOnNull()
    {
        // Arrange
        var task = null as Task<Result>;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => task!.AsHttpResultAsync());
    }

    /// <summary>
    /// The <see cref="ResultBaseExtensions.AsHttpResultAsync{T}(Task{Result{T}})"/> method should
    /// return an <see cref="Ok{TValue}"/> result for a successful operation.
    /// </summary>
    [Fact]
    public async Task AsHttpResultAsyncGeneric_ReturnsOk()
    {
        // Arrange
        var value = "Test Value";

        var task = CreateResultingTask(OperationStatus.Success, null, value);

        // Act
        var result = await task.AsHttpResultAsync();

        // Assert
        var okResult = Assert.IsType<Ok<string>>(result);

        Assert.Equal(value, okResult.Value);
    }

    /// <summary>
    /// The <see cref="ResultBaseExtensions.AsHttpResultAsync{T}(Task{Result{T}})"/> method should
    /// return a <see cref="Created{TValue}"/> result for a created operation.
    /// </summary>
    [Fact]
    public async Task AsHttpResultAsyncGeneric_ReturnsCreated()
    {
        // Arrange
        var value = "Test Value";

        var task = CreateResultingTask(OperationStatus.Created, null, value);

        // Act
        var result = await task.AsHttpResultAsync();

        // Assert
        var createdResult = Assert.IsType<Created<string>>(result);

        Assert.Equal(value, createdResult.Value);
        Assert.Null(createdResult.Location);
    }

    /// <summary>
    /// The <see cref="ResultBaseExtensions.AsHttpResultAsync{T}(Task{Result{T}})"/> method should
    /// return an <see cref="Accepted{TValue}"/> result for an accepted operation.
    /// </summary>
    [Fact]
    public async Task AsHttpResultAsyncGeneric_ReturnsAccepted()
    {
        // Arrange
        var value = "Test Value";

        var task = CreateResultingTask(OperationStatus.Received, null, value);

        // Act
        var result = await task.AsHttpResultAsync();

        // Assert
        var acceptedResult = Assert.IsType<Accepted<string>>(result);

        Assert.Equal(value, acceptedResult.Value);
    }

    /// <summary>
    /// The <see cref="ResultBaseExtensions.AsHttpResultAsync{T}(Task{Result{T}})"/> method should
    /// return a <see cref="ProblemHttpResult"/> for a request resulted in an error.
    /// </summary>
    [Theory, MemberData(nameof(AsHttpResultAsyncGeneric_ReturnsProblem_MemberData))]
    public async Task AsHttpResultAsyncGeneric_ReturnsProblem(OperationStatus status)
    {
        // Arrange
        var value = null as object;
        var errors = new[]
        {
            new ApplicationError(ErrorCode.DefaultError, "Error 1"),
            new ApplicationError(ErrorCode.DefaultError, "Error 2")
        };
        var task = CreateResultingTask(status, errors, value);

        // Act
        var result = await task.AsHttpResultAsync();

        // Assert
        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        var details = Assert.IsType<ApplicationProblemDetails>(problemResult.ProblemDetails);

        Assert.Equal(errors, details.Errors);
    }

    /// <summary>
    /// The <see cref="ResultBaseExtensions.AsHttpResultAsync{T}(Task{Result{T}})"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="Task{T}"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task AsHttpResultAsyncGeneric_ThrowsOnNull()
    {
        // Arrange
        var task = null as Task<Result<string>>;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => task!.AsHttpResultAsync());
    }

    #endregion

    #region Private methods
    /// <summary>
    /// Creates a <see cref="Task{T}"/> of <see cref="Result"/> with the specified status and errors.
    /// </summary>
    /// <param name="status">
    /// The status of the result.
    /// </param>
    /// <param name="errors">
    /// The errors associated with the result.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    private static Task<Result> CreateResultingTask(OperationStatus status, IEnumerable<ApplicationError>? errors = null)
    {
        var result = new Result(status, errors ?? []);

        return Task.FromResult(result);
    }

    /// <summary>
    /// Creates a <see cref="Task{T}"/> of <see cref="Result{T}"/> with the specified status, value, and errors.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value associated with the result.
    /// </typeparam>
    /// <param name="status">
    /// The status of the result.
    /// </param>
    /// <param name="errors">
    /// The errors associated with the result.
    /// </param>
    /// <param name="value">
    /// The value associated with the result.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    private static Task<Result<T>> CreateResultingTask<T>(OperationStatus status, IEnumerable<ApplicationError>? errors = null, T? value = default)
    {
        var result = new Result<T>(status, errors ?? [], value);

        return Task.FromResult(result);
    }
    #endregion
}