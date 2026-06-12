using Microsoft.AspNetCore.Http;
using Paradise.Models;
using Paradise.WebApi.Base.Extensions;

namespace Paradise.WebApi.Infrastructure.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ResultBase"/> <see langword="class"/>.
/// </summary>
public static class ResultBaseExtensions
{
    #region Public methods
    /// <summary>
    /// Converts the completed <see cref="Task"/> containing a <see cref="Result"/>
    /// into an HTTP-specific <see cref="IResult"/> representation.
    /// </summary>
    /// <param name="resultingTask">
    /// The task that produces the operation result.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation,
    /// containing the <see cref="IResult"/> of the operation.
    /// </returns>
    public static async Task<IResult> AsHttpResultAsync(this Task<Result> resultingTask)
    {
        ArgumentNullException.ThrowIfNull(resultingTask);

        var result = await resultingTask
            .ConfigureAwait(false);

        return result.AsHttpResult();
    }

    /// <summary>
    /// Converts the completed <see cref="Task"/> containing a <see cref="Result{T}"/>
    /// into an HTTP-specific <see cref="IResult"/> representation.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value returned by the operation.
    /// </typeparam>
    /// <param name="resultingTask">
    /// The task that produces the operation result.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation,
    /// containing the <see cref="IResult"/> of the operation.
    /// </returns>
    public static async Task<IResult> AsHttpResultAsync<T>(this Task<Result<T>> resultingTask)
    {
        ArgumentNullException.ThrowIfNull(resultingTask);

        var result = await resultingTask
            .ConfigureAwait(false);

        return result.AsHttpResult();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Converts the given <paramref name="result"/> into
    /// a new <see cref="IResult"/> instance.
    /// </summary>
    /// <param name="result">
    /// The <see cref="ResultBase"/> to convert.
    /// </param>
    /// <returns>
    /// New <see cref="IResult"/> instance.
    /// </returns>
    private static IResult AsHttpResult(this Result result)
    {
        var statusCode = result.Status.GetStatusCode();

        return statusCode switch
        {
            StatusCodes.Status200OK => Results.Ok(),
            StatusCodes.Status201Created => Results.Created(),
            StatusCodes.Status202Accepted => Results.Accepted(),
            _ => Results.Problem(new ApplicationProblemDetails(statusCode, result.Errors))
        };
    }

    /// <summary>
    /// Converts the given <paramref name="result"/> into
    /// a new <see cref="IResult"/> instance.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value returned by the operation.
    /// </typeparam>
    /// <param name="result">
    /// The <see cref="ResultBase"/> to convert.
    /// </param>
    /// <returns>
    /// New <see cref="IResult"/> instance.
    /// </returns>
    private static IResult AsHttpResult<T>(this Result<T> result)
    {
        var statusCode = result.Status.GetStatusCode();

        return statusCode switch
        {
            StatusCodes.Status200OK => Results.Ok(result.Value),
            StatusCodes.Status201Created => Results.Created(null as Uri, result.Value),
            StatusCodes.Status202Accepted => Results.Accepted(null, result.Value),
            _ => Results.Problem(new ApplicationProblemDetails(statusCode, result.Errors))
        };
    }
    #endregion
}