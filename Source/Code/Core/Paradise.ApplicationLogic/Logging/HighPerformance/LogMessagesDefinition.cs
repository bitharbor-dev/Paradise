using Microsoft.Extensions.Logging;
using static Microsoft.Extensions.Logging.LoggerMessage;
using static Microsoft.Extensions.Logging.LogLevel;
using static Paradise.ApplicationLogic.Logging.EventIdContainer;
using Logs = Paradise.Localization.Logging.LogMessages;

namespace Paradise.ApplicationLogic.Logging.HighPerformance;

/// <summary>
/// Contains a runtime-defined actions to be executed while logging.
/// </summary>
internal static class LogMessagesDefinition
{
    #region Properties
    /// <summary>
    /// Gets an action to be executed while logging database seed failure.
    /// </summary>
    public static Action<ILogger, string?, Exception?> CriticalDatabaseSeedFailure { get; }
        = Define<string?>(Critical, DatabaseSeedFailureLogEvent, Logs.CriticalDatabaseSeedFailure);

    /// <summary>
    /// Gets an action to be executed while logging the database-related exception.
    /// </summary>
    public static Action<ILogger, string?, Exception?> CriticalDatabaseException { get; }
        = Define<string?>(Critical, DatabaseExceptionLogEvent, Logs.CriticalDatabaseException);

    /// <summary>
    /// Gets an action to be executed while logging background worker execution failure time.
    /// </summary>
    public static Action<ILogger, DateTime, Exception?> CriticalWorkerExecutionFailure { get; }
        = Define<DateTime>(Critical, WorkerExecutionFailureLogEvent, Logs.CriticalWorkerExecutionFailure);

    /// <summary>
    /// Gets an action to be executed while logging background worker options initial state.
    /// </summary>
    public static Action<ILogger, string, object, Exception?> InformationWorkerOptionsInitialState { get; }
        = Define<string, object>(Information, WorkerOptionsInitialStateLogEvent, Logs.InformationWorkerOptionsInitialState);

    /// <summary>
    /// Gets an action to be executed while logging background worker options changed event.
    /// </summary>
    public static Action<ILogger, DateTime, string, object, Exception?> InformationWorkerOptionsChangedState { get; }
        = Define<DateTime, string, object>(Information, WorkerOptionsChangedStateLogEvent, Logs.InformationWorkerOptionsChangedState);

    /// <summary>
    /// Gets an action to be executed while logging application result errors.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> ErrorResultErrors { get; }
        = Define<string, string>(Error, ResultErrorsLogEvent, Logs.ErrorResultErrors);

    /// <summary>
    /// Gets an action to be executed while logging application result critical errors.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> CriticalResultCriticalErrors { get; }
        = Define<string, string>(Critical, ResultCriticalErrorsLogEvent, Logs.CriticalResultErrors);

    /// <summary>
    /// Gets an action to be executed while logging added seed item information.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> InformationAddedSeedItem { get; }
        = Define<string, string>(Information, AddedSeedItemLogEvent, Logs.InformationAddedSeedItem);

    /// <summary>
    /// Gets an action to be executed while logging updated seed item information.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> InformationUpdatedSeedItem { get; }
        = Define<string, string>(Information, UpdatedSeedItemLogEvent, Logs.InformationUpdatedSeedItem);

    /// <summary>
    /// Gets an action to be executed while logging the number of pending deletion users.
    /// </summary>
    public static Action<ILogger, int, Exception?> InformationPendingDeletionUsersNumber { get; }
        = Define<int>(Information, PendingDeletionUsersNumberLogEvent, Logs.InformationPendingDeletionUsersNumber);

    /// <summary>
    /// Gets an action to be executed while logging the number of outdated user refresh tokens.
    /// </summary>
    public static Action<ILogger, int, Exception?> InformationOutdatedTokensNumber { get; }
        = Define<int>(Information, OutdatedTokensNumberLogEvent, Logs.InformationOutdatedTokensNumber);

    /// <summary>
    /// Gets an action to be executed while logging the number of unconfirmed users.
    /// </summary>
    public static Action<ILogger, int, Exception?> InformationUnconfirmedUsersNumber { get; }
        = Define<int>(Information, UnconfirmedUsersNumberLogEvent, Logs.InformationUnconfirmedUsersNumber);

    /// <summary>
    /// Gets an action to be executed while logging background worker execution time.
    /// </summary>
    public static Action<ILogger, DateTime, Exception?> InformationWorkerRunning { get; }
        = Define<DateTime>(Information, WorkerRunningLogEvent, Logs.InformationWorkerRunning);

    /// <summary>
    /// Gets an action to be executed while logging database entry seed failure.
    /// </summary>
    public static Action<ILogger, string, string, string, Exception?> WarningDatabaseEntrySeedFailure { get; }
        = Define<string, string, string>(Warning, DatabaseEntrySeedFailureLogEvent, Logs.WarningDatabaseEntrySeedFailure);

    /// <summary>
    /// Gets an action to be executed while logging claims addition failure.
    /// </summary>
    public static Action<ILogger, Exception?> CriticalClaimsAdditionFailure { get; }
        = Define(Critical, ClaimsAdditionFailureLogEvent, Logs.CriticalClaimsAdditionFailure);

    /// <summary>
    /// Gets an action to be executed while logging an unhandled exception.
    /// </summary>
    public static Action<ILogger, Exception> CriticalUnhandledExceptionOccurred { get; }
        = Define(Critical, UnhandledExceptionOccurredLogEvent, Logs.CriticalUnhandledExceptionOccurred);

    /// <summary>
    /// Gets an action to be executed while logging user deletion failure after unsuccessful invitation.
    /// </summary>
    public static Action<ILogger, string, string, string, Exception?> CriticalUnsuccessfulUserDeletionAfterFailedInvitation { get; }
        = Define<string, string, string>(Critical, UnsuccessfulUserDeletionAfterFailedInvitationLogEvent, Logs.CriticalUnsuccessfulUserDeletionAfterFailedInvitation);
    #endregion
}