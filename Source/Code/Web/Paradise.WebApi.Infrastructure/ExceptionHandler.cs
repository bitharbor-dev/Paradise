using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Paradise.Models;
using Paradise.Models.Extensions;
using Paradise.Primitives.Extensions;

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
public sealed class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    #region Public methods
    /// <inheritdoc/>
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogUnhandledException(exception);

        ArgumentNullException.ThrowIfNull(httpContext);

        if (httpContext.Response.HasStarted)
            return ValueTask.FromResult(false);

        var statusCode = StatusCodes.Status500InternalServerError;
        var errorCode = ErrorCode.DefaultError;
        var error = new ApplicationError(errorCode, errorCode.GetFormattedDisplayValue());

        var problemDetails = new ApplicationProblemDetails(statusCode, [error]);
        var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

        return problemDetailsService.TryWriteAsync(new()
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
    #endregion
}