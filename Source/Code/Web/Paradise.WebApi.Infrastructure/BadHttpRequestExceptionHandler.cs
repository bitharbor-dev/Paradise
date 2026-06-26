using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Paradise.Models;
using Paradise.Models.Extensions;

namespace Paradise.WebApi.Infrastructure;

/// <summary>
/// <see cref="BadHttpRequestException"/> handler.
/// </summary>
public sealed class BadHttpRequestExceptionHandler : IExceptionHandler
{
    #region Public methods
    /// <inheritdoc/>
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException badHttpRequestException)
            return ValueTask.FromResult(false);

        ArgumentNullException.ThrowIfNull(httpContext);

        if (httpContext.Response.HasStarted)
            return ValueTask.FromResult(false);

        httpContext.Response.StatusCode = badHttpRequestException.StatusCode;

        var errorCode = ErrorCode.InvalidModel;
        var error = new ApplicationError(errorCode, errorCode.GetFormattedDisplayValue(exception.Message));

        var problemDetails = new ApplicationProblemDetails(httpContext.Response.StatusCode, [error]);
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