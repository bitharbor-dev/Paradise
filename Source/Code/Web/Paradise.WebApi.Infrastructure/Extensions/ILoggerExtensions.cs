using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static Paradise.Localization.Logging.LogMessagesDefinition;

namespace Paradise.WebApi.Infrastructure.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ILogger"/> <see langword="interface"/>.
/// </summary>
public static class ILoggerExtensions
{
    #region Public methods
    /// <summary>
    /// Creates a log entry containing information
    /// about the given <paramref name="request"/>.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="request">
    /// The <see cref="HttpRequest"/> to be logged.
    /// </param>
    public static void LogFallbackHandlerInvocation(this ILogger logger, HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var method = request.Method;
        var path = $"{request.Path}{request.QueryString}";
        var scheme = request.Scheme;
        var host = request.Host.ToString();
        var traceId = request.HttpContext.TraceIdentifier;

        FallbackHandlerReached(logger, method, path, scheme, host, traceId, null);
    }
    #endregion
}