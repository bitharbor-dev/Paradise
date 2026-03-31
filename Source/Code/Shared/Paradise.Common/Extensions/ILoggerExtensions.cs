using Microsoft.Extensions.Logging;
using static Paradise.Localization.Logging.LogMessagesDefinition;

namespace Paradise.Common.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ILogger"/> <see langword="interface"/>.
/// </summary>
public static class ILoggerExtensions
{
    #region Public methods
    /// <summary>
    /// Creates a log entry containing information
    /// about unhandled exception which occurred.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="exception">
    /// <see cref="Exception"/> to be logged.
    /// </param>
    public static void LogUnhandledException(this ILogger logger, Exception exception)
        => UnhandledExceptionOccurred(logger, exception);
    #endregion
}