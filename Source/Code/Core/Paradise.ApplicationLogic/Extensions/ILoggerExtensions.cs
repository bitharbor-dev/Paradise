using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Paradise.ApplicationLogic.Exceptions;
using Paradise.Models;
using System.Runtime.CompilerServices;
using System.Text.Json;
using static Paradise.ApplicationLogic.Logging.HighPerformance.LogMessagesDefinition;
using static System.Environment;

namespace Paradise.ApplicationLogic.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ILogger"/> interface.
/// </summary>
public static class ILoggerExtensions
{
    #region Public methods
    /// <summary>
    /// Creates a log entry containing information
    /// about the given <paramref name="result"/> errors.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="result">
    /// The <see cref="Result"/> which errors to be logged.
    /// </param>
    public static void LogResultErrors(this ILogger logger, Result result)
    {
        var nonCriticalErrors = result.Errors.Where(error => !error.IsCritical);

        if (nonCriticalErrors.Any())
        {
            var errorMessage = string.Join(NewLine, nonCriticalErrors);

            ErrorResultErrors(logger, NewLine, errorMessage, result.Exception);
        }
    }

    /// <summary>
    /// Creates a log entry containing information
    /// about the given <paramref name="result"/> critical errors.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="result">
    /// The <see cref="Result"/> which errors to be logged.
    /// </param>
    public static void LogResultCriticalErrors(this ILogger logger, Result result)
    {
        var criticalErrors = result.Errors.Where(error => error.IsCritical);

        if (criticalErrors.Any())
        {
            var errorMessage = string.Join(NewLine, criticalErrors);

            CriticalResultCriticalErrors(logger, NewLine, errorMessage, result.Exception);
        }
    }

    /// <summary>
    /// Creates a log entry containing information
    /// about the background worker execution time.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    public static void LogWorkerRunning(this ILogger logger)
        => InformationWorkerRunning(logger, DateTime.UtcNow, null);

    /// <summary>
    /// Creates a log entry containing information
    /// about the background worker execution failure time.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="exception">
    /// <see cref="Exception"/> to be logged.
    /// </param>
    public static void LogWorkerExecutionFailure(this ILogger logger, Exception exception)
        => CriticalWorkerExecutionFailure(logger, DateTime.UtcNow, exception);

    /// <summary>
    /// Creates a log entry containing information
    /// about the background worker's options initial state.
    /// </summary>
    /// <typeparam name="T">
    /// Options type.
    /// </typeparam>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="options">
    /// Options.
    /// </param>
    /// <param name="jsonOptions">
    /// Options to control serialization behavior.
    /// </param>
    public static void LogWorkerOptionsInitialState<T>(this ILogger logger, T options, JsonSerializerOptions? jsonOptions = null)
    {
        var json = JsonSerializer.Serialize(options, jsonOptions);

        InformationWorkerOptionsInitialState(logger, NewLine, json, null);
    }

    /// <summary>
    /// Creates a log entry containing information
    /// about the background worker's new options.
    /// </summary>
    /// <typeparam name="T">
    /// Options type.
    /// </typeparam>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="newOptions">
    /// New options value.
    /// </param>
    /// <param name="jsonOptions">
    /// Options to control serialization behavior.
    /// </param>
    public static void LogWorkerOptionsChangedState<T>(this ILogger logger, T newOptions, JsonSerializerOptions? jsonOptions = null)
    {
        var json = JsonSerializer.Serialize(newOptions, jsonOptions);

        InformationWorkerOptionsChangedState(logger, DateTime.UtcNow, NewLine, json, null);
    }

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
    public static void LogDatabaseSeedFailure(this ILogger logger, Exception exception, [CallerMemberName] string? methodName = null)
        => CriticalDatabaseSeedFailure(logger, methodName, exception);

    /// <summary>
    /// Creates a log entry containing information
    /// about the database entry seed failure.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="itemName">
    /// Seeded item name.
    /// </param>
    /// <param name="result">
    /// The <see cref="Result"/> which errors to be logged.
    /// </param>
    public static void LogDatabaseEntrySeedFailure(this ILogger logger, string itemName, Result result)
    {
        var errorMessage = string.Join(NewLine, result.Errors);

        WarningDatabaseEntrySeedFailure(logger, itemName, NewLine, errorMessage, null);
    }

    /// <summary>
    /// Creates a log entry containing information
    /// about the exception that occurred during
    /// database operation execution.
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
    public static void LogDatabaseException(this ILogger logger, Exception exception, [CallerMemberName] string? methodName = null)
        => CriticalDatabaseException(logger, methodName, exception);

    /// <summary>
    /// Creates a log entry containing information
    /// about the number of unconfirmed users.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="unconfirmedUsersNumber">
    /// The number of unconfirmed users.
    /// </param>
    public static void LogUnconfirmedUsersNumber(this ILogger logger, int unconfirmedUsersNumber)
        => InformationUnconfirmedUsersNumber(logger, unconfirmedUsersNumber, null);

    /// <summary>
    /// Creates a log entry containing information
    /// about the number of pending deletion users.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="pendingDeletionUsersNumber">
    /// The number of pending deletion users.
    /// </param>
    public static void LogPendingDeletionUsersNumber(this ILogger logger, int pendingDeletionUsersNumber)
        => InformationPendingDeletionUsersNumber(logger, pendingDeletionUsersNumber, null);

    /// <summary>
    /// Creates a log entry containing information
    /// about the number of outdated refresh tokens.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="outdatedTokensNumber">
    /// Outdated refresh tokens number.
    /// </param>
    public static void LogOutdatedTokensNumber(this ILogger logger, int outdatedTokensNumber)
        => InformationOutdatedTokensNumber(logger, outdatedTokensNumber, null);

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
    public static void LogAddedSeedItem<T>(this ILogger logger, string itemName)
        => InformationAddedSeedItem(logger, typeof(T).Name, itemName, null);

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
    public static void LogUpdatedSeedItem<T>(this ILogger logger, string itemName)
        => InformationUpdatedSeedItem(logger, typeof(T).Name, itemName, null);

    /// <summary>
    /// Creates a log entry containing information
    /// about the user claims creation failure.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    public static void LogClaimsAdditionFailure(this ILogger logger)
        => CriticalClaimsAdditionFailure(logger, null);

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
        => CriticalUnhandledExceptionOccurred(logger, exception);

    /// <summary>
    /// Logs error from the given <paramref name="resultException"/>.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="resultException">
    /// The <see cref="ResultException"/> which errors to be logged.
    /// </param>
    public static void LogResultException(this ILogger logger, ResultException resultException)
    {
        var result = resultException.GetResult();

        logger.LogResultErrors(result);
        logger.LogResultCriticalErrors(result);
    }

    /// <summary>
    /// Creates a log entry containing information
    /// about the user which deletion failed after unsuccessful invitation.
    /// </summary>
    /// <param name="logger">
    /// The input <see cref="ILogger"/> object.
    /// </param>
    /// <param name="email">
    /// User's email address.
    /// </param>
    /// <param name="identityResult">
    /// The <see cref="IdentityResult"/> to add errors from.
    /// </param>
    public static void LogUnsuccessfulUserDeletionAfterFailedInvitation(this ILogger logger, string email, IdentityResult identityResult)
    {
        var errors = string.Join(NewLine, identityResult.Errors.Select(error => error.Description));

        CriticalUnsuccessfulUserDeletionAfterFailedInvitation(logger, email, NewLine, errors, null);
    }
    #endregion
}