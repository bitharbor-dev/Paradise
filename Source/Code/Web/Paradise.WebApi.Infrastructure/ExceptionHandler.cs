using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Paradise.Common.Extensions;
using Paradise.Models;
using Paradise.WebApi.Infrastructure.Extensions;
using System.Text.Json;

namespace Paradise.WebApi.Infrastructure;

/// <summary>
/// Global exception handler.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
/// </remarks>
/// <param name="logger">
/// Logger.
/// </param>
/// <param name="jsonSerializerOptions">
/// The accessor used to access the <see cref="JsonSerializerOptions"/>.
/// </param>
public sealed class ExceptionHandler(ILogger<ExceptionHandler> logger, IOptions<JsonSerializerOptions> jsonSerializerOptions)
    : IExceptionHandler
{
    #region Public methods
    /// <summary>
    /// Writes a fallback error response when exception handling
    /// cannot be completed through the normal <see cref="IExceptionHandler"/> pipeline.
    /// </summary>
    /// <param name="httpContext">
    /// The HTTP context associated with the current request.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public static Task HandleFallbackAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var logger = httpContext
            .RequestServices
            .GetRequiredService<ILogger<ExceptionHandler>>();

        var jsonSerializerOptions = httpContext
            .RequestServices
            .GetRequiredService<IOptions<JsonSerializerOptions>>();

        logger.LogFallbackHandlerInvocation(httpContext.Request);

        return WriteDefaultFailureResult(httpContext.Response, jsonSerializerOptions, httpContext.RequestAborted);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogUnhandledException(exception);

        ArgumentNullException.ThrowIfNull(httpContext);

        if (httpContext.Response.HasStarted)
            return false;

        await WriteDefaultFailureResult(httpContext.Response, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        return true;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Writes the default failure response using the standard API
    /// error envelope and configured JSON serialization options.
    /// </summary>
    /// <param name="response">
    /// The HTTP response receiving the serialized error payload.
    /// </param>
    /// <param name="options">
    /// The accessor used to access the <see cref="JsonSerializerOptions"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    private static Task WriteDefaultFailureResult(HttpResponse response,
                                                  IOptions<JsonSerializerOptions> options,
                                                  CancellationToken cancellationToken)
    {
        var actionResult = new Result(OperationStatus.Failure, ErrorCode.DefaultError)
            .AsActionResult();

        return actionResult.WriteResponseContentAsync(response, options.Value, cancellationToken);
    }
    #endregion
}