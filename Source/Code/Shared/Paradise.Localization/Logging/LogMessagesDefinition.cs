using Microsoft.Extensions.Logging;

namespace Paradise.Localization.Logging;

/// <summary>
/// Contains a runtime-defined actions to be executed while logging.
/// </summary>
public static class LogMessagesDefinition
{
    #region Properties
    /// <summary>
    /// Gets an action to be executed while logging database seed failure.
    /// </summary>
    public static Action<ILogger, string?, Exception?> CriticalDatabaseSeedFailure { get; }
        = LoggerMessage.Define<string?>(
            LogLevel.Critical,
            LogMessagesEventIds.LogEventDatabaseSeedFailure,
            LogMessages.CriticalDatabaseSeedFailure);

    /// <summary>
    /// Gets an action to be executed while logging the database-related exception.
    /// </summary>
    public static Action<ILogger, string?, Exception?> CriticalDatabaseException { get; }
        = LoggerMessage.Define<string?>(
            LogLevel.Critical,
            LogMessagesEventIds.LogEventDatabaseException,
            LogMessages.CriticalDatabaseException);

    /// <summary>
    /// Gets an action to be executed while logging background worker execution failure time.
    /// </summary>
    public static Action<ILogger, DateTime, Exception?> CriticalWorkerExecutionFailure { get; }
        = LoggerMessage.Define<DateTime>(
            LogLevel.Critical,
            LogMessagesEventIds.LogEventWorkerExecutionFailure,
            LogMessages.CriticalWorkerExecutionFailure);

    /// <summary>
    /// Gets an action to be executed while logging background worker options initial state.
    /// </summary>
    public static Action<ILogger, string, object, Exception?> InformationWorkerOptionsInitialState { get; }
        = LoggerMessage.Define<string, object>(
            LogLevel.Information,
            LogMessagesEventIds.LogEventWorkerOptionsInitialState,
            LogMessages.InformationWorkerOptionsInitialState);

    /// <summary>
    /// Gets an action to be executed while logging background worker options changed event.
    /// </summary>
    public static Action<ILogger, DateTime, string, object, Exception?> InformationWorkerOptionsChangedState { get; }
        = LoggerMessage.Define<DateTime, string, object>(
            LogLevel.Information,
            LogMessagesEventIds.LogEventWorkerOptionsChangedState,
            LogMessages.InformationWorkerOptionsChangedState);

    /// <summary>
    /// Gets an action to be executed while logging application result errors.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> ErrorResultErrors { get; }
        = LoggerMessage.Define<string, string>(
            LogLevel.Error,
            LogMessagesEventIds.LogEventResultErrors,
            LogMessages.ErrorResultErrors);

    /// <summary>
    /// Gets an action to be executed while logging application result critical errors.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> CriticalResultCriticalErrors { get; }
        = LoggerMessage.Define<string, string>(
            LogLevel.Critical,
            LogMessagesEventIds.LogEventResultCriticalErrors,
            LogMessages.CriticalResultErrors);

    /// <summary>
    /// Gets an action to be executed while logging added seed item information.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> InformationAddedSeedItem { get; }
        = LoggerMessage.Define<string, string>(
            LogLevel.Information,
            LogMessagesEventIds.LogEventAddedSeedItem,
            LogMessages.InformationAddedSeedItem);

    /// <summary>
    /// Gets an action to be executed while logging updated seed item information.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> InformationUpdatedSeedItem { get; }
        = LoggerMessage.Define<string, string>(
            LogLevel.Information,
            LogMessagesEventIds.LogEventUpdatedSeedItem,
            LogMessages.InformationUpdatedSeedItem);

    /// <summary>
    /// Gets an action to be executed while logging the number of pending deletion users.
    /// </summary>
    public static Action<ILogger, int, Exception?> InformationPendingDeletionUsersNumber { get; }
        = LoggerMessage.Define<int>(
            LogLevel.Information,
            LogMessagesEventIds.LogEventPendingDeletionUsersNumber,
            LogMessages.InformationPendingDeletionUsersNumber);

    /// <summary>
    /// Gets an action to be executed while logging the number of outdated user refresh tokens.
    /// </summary>
    public static Action<ILogger, int, Exception?> InformationOutdatedTokensNumber { get; }
        = LoggerMessage.Define<int>(
            LogLevel.Information,
            LogMessagesEventIds.LogEventOutdatedTokensNumber,
            LogMessages.InformationOutdatedTokensNumber);

    /// <summary>
    /// Gets an action to be executed while logging the number of unconfirmed users.
    /// </summary>
    public static Action<ILogger, int, Exception?> InformationUnconfirmedUsersNumber { get; }
        = LoggerMessage.Define<int>(
            LogLevel.Information,
            LogMessagesEventIds.LogEventUnconfirmedUsersNumber,
            LogMessages.InformationUnconfirmedUsersNumber);

    /// <summary>
    /// Gets an action to be executed while logging background worker execution time.
    /// </summary>
    public static Action<ILogger, DateTime, Exception?> InformationWorkerRunning { get; }
        = LoggerMessage.Define<DateTime>(
            LogLevel.Information,
            LogMessagesEventIds.LogEventWorkerRunning,
            LogMessages.InformationWorkerRunning);

    /// <summary>
    /// Gets an action to be executed while logging database entry seed failure.
    /// </summary>
    public static Action<ILogger, string, string, string, Exception?> WarningDatabaseEntrySeedFailure { get; }
        = LoggerMessage.Define<string, string, string>(
            LogLevel.Warning,
            LogMessagesEventIds.LogEventDatabaseEntrySeedFailure,
            LogMessages.WarningDatabaseEntrySeedFailure);

    /// <summary>
    /// Gets an action to be executed while logging claims addition failure.
    /// </summary>
    public static Action<ILogger, Exception?> CriticalClaimsAdditionFailure { get; }
        = LoggerMessage.Define(
            LogLevel.Critical,
            LogMessagesEventIds.LogEventClaimsAdditionFailure,
            LogMessages.CriticalClaimsAdditionFailure);

    /// <summary>
    /// Gets an action to be executed while logging an unhandled exception.
    /// </summary>
    public static Action<ILogger, Exception> CriticalUnhandledExceptionOccurred { get; }
        = LoggerMessage.Define(
            LogLevel.Critical,
            LogMessagesEventIds.LogEventUnhandledExceptionOccurred,
            LogMessages.CriticalUnhandledExceptionOccurred);

    /// <summary>
    /// Gets an action to be executed while logging user deletion failure after unsuccessful invitation.
    /// </summary>
    public static Action<ILogger, string, string, string, Exception?> CriticalUnsuccessfulUserDeletionAfterFailedInvitation { get; }
        = LoggerMessage.Define<string, string, string>(
            LogLevel.Critical,
            LogMessagesEventIds.LogEventUnsuccessfulUserDeletionAfterFailedInvitation,
            LogMessages.CriticalUnsuccessfulUserDeletionAfterFailedInvitation);
    #endregion
}