using Microsoft.Extensions.Logging;
using Paradise.Models;
using System.Diagnostics.CodeAnalysis;
using static Paradise.Localization.Logging.LogMessagesDefinition;
using static System.Environment;

namespace Paradise.ApplicationLogic.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ILogger"/> <see langword="interface"/>.
/// </summary>
public static class ILoggerExtensions
{
    #region Public methods
    /// <summary>
    /// Creates a log entry containing information
    /// about the given <paramref name="errors"/> and
    /// <paramref name="exception"/>.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="errors">
    /// The list of errors to be logged.
    /// </param>
    /// <param name="exception">
    /// The <see cref="Exception"/> instance to be logged.
    /// </param>
    /// <param name="critical">
    /// Defines what errors have to be included in the log:
    /// <list type="bullet">
    /// <item>
    /// <see langword="true"/> - critical only.
    /// </item>
    /// <item>
    /// <see langword="false"/> - non critical only.
    /// </item>
    /// <item>
    /// <see langword="null"/> - all errors.
    /// </item>
    /// </list>
    /// </param>
    [SuppressMessage("Style", "IDE0030:Use coalesce expression", Justification = "Omitted for readability.")]
    public static void LogResultErrors(this ILogger logger, IEnumerable<ApplicationError> errors,
                                       Exception? exception, bool? critical = null)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var filteredErrors = critical.HasValue
            ? errors.Where(error => error.IsCritical == critical.Value)
            : errors;

        if (!filteredErrors.Any())
            return;

        var errorMessage = string.Join(NewLine, filteredErrors);

        var logCriticalErrors = critical.HasValue
            ? critical.Value
            : errors.Any(error => error.IsCritical);

        if (logCriticalErrors)
            CriticalResultErrors(logger, NewLine, errorMessage, exception);
        else
            ErrorResultErrors(logger, NewLine, errorMessage, exception);
    }
    #endregion
}