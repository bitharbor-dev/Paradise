using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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

        var problemDetails = new ApplicationProblemDetails(httpContext.Response.StatusCode, []);
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