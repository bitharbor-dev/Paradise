using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using static Paradise.Localization.Logging.LogMessagesDefinition;
using static System.Environment;

namespace Paradise.ApplicationLogic.Infrastructure.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ILogger"/> <see langword="interface"/>.
/// </summary>
public static class ILoggerExtensions
{
    #region Public methods
    /// <summary>
    /// Creates a log entry containing information
    /// about the given <paramref name="identityResult"/>.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="identityResult">
    /// The <see cref="IdentityResult"/> to be logged.
    /// </param>
    public static void LogIdentityResult(this ILogger logger, IdentityResult identityResult)
    {
        ArgumentNullException.ThrowIfNull(identityResult);

        var errors = identityResult.Errors.Select(error => error.Description);

        CriticalResultErrors(logger, NewLine, string.Join(NewLine, errors), null);
    }
    #endregion

    #region Internal methods
    /// <summary>
    /// Creates a log entry containing information
    /// about the added database seed item.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="itemName">
    /// Seed item name.
    /// </param>
    internal static void LogAddedSeedItem<T>(this ILogger logger, string itemName)
        => AddedSeedItem(logger, typeof(T).Name, itemName, null);

    /// <summary>
    /// Creates a log entry containing information
    /// about the updated database seed item.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="itemName">
    /// Seed item name.
    /// </param>
    internal static void LogUpdatedSeedItem<T>(this ILogger logger, string itemName)
        => UpdatedSeedItem(logger, typeof(T).Name, itemName, null);

    /// <summary>
    /// Creates a log entry containing information
    /// about the database seed failure.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="exception">
    /// <see cref="Exception"/> to be logged.
    /// </param>
    /// <param name="methodName">
    /// Method name to be logged.
    /// </param>
    internal static void LogDatabaseSeedFailure(this ILogger logger, Exception exception, [CallerMemberName] string? methodName = null)
        => DatabaseSeedFailure(logger, methodName, exception);
    #endregion
}