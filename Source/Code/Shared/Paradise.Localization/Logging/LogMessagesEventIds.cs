using Microsoft.Extensions.Logging;
using static Paradise.Localization.Logging.LogMessagesDefinition;

namespace Paradise.Localization.Logging;

/// <summary>
/// Contains <see cref="EventId"/> instances used by the application.
/// </summary>
/// <remarks>
/// There is a convention on how to define a new event id:
/// <list type="number">
/// <item>
/// All numbers start from 5 to leave some capacity
/// for events being logged from packages used by the application.
/// </item>
/// <item>
/// The second number is an integer representation of the <see cref="LogLevel"/> <see cref="Enum"/>.
/// <list type="bullet">
/// <item>
/// <see cref="LogLevel.Trace"/> = 0.
/// </item>
/// <item>
/// <see cref="LogLevel.Debug"/> = 1.
/// </item>
/// <item>
/// <see cref="LogLevel.Information"/> = 2.
/// </item>
/// <item>
/// <see cref="LogLevel.Warning"/> = 3.
/// </item>
/// <item>
/// <see cref="LogLevel.Error"/> = 4.
/// </item>
/// <item>
/// <see cref="LogLevel.Critical"/> = 5.
/// </item>
/// </list>
/// Considering this, we can easily find critical events (starting from 55) and handle such issues.
/// </item>
/// <item>
/// The rest of the numbers can be used as you like,
/// but avoid the number 55 except at the beginning of the id
/// to make it easier to find critical errors.
/// </item>
/// </list>
/// There is also a naming convention:
/// <list type="number">
/// <item>
/// Take the name of the logging action from <see cref="LogMessagesDefinition"/> and remove <see cref="LogLevel"/> prefix.
/// <para>
/// <c>CriticalDatabaseSeedFailure => DatabaseSeedFailure</c>
/// </para>
/// </item>
/// <item>
/// Add "LogEvent" to the end.
/// <para>
/// <c>DatabaseSeedFailure => DatabaseSeedFailureLogEvent</c>
/// </para>
/// </item>
/// </list>
/// </remarks>
internal static class LogMessagesEventIds
{
    #region Properties
    /// <summary>
    /// Event id for <see cref="CriticalDatabaseSeedFailure"/>.
    /// </summary>
    public static EventId LogEventDatabaseSeedFailure { get; } = new(55000, nameof(CriticalDatabaseSeedFailure));

    /// <summary>
    /// Event id for <see cref="CriticalDatabaseException"/>.
    /// </summary>
    public static EventId LogEventDatabaseException { get; } = new(55001, nameof(CriticalDatabaseException));

    /// <summary>
    /// Event id for <see cref="CriticalWorkerExecutionFailure"/>.
    /// </summary>
    public static EventId LogEventWorkerExecutionFailure { get; } = new(55002, nameof(CriticalWorkerExecutionFailure));

    /// <summary>
    /// Event id for <see cref="InformationWorkerOptionsInitialState"/>.
    /// </summary>
    public static EventId LogEventWorkerOptionsInitialState { get; } = new(52000, nameof(InformationWorkerOptionsInitialState));

    /// <summary>
    /// Event id for <see cref="InformationWorkerOptionsChangedState"/>.
    /// </summary>
    public static EventId LogEventWorkerOptionsChangedState { get; } = new(52001, nameof(InformationWorkerOptionsChangedState));

    /// <summary>
    /// Event id for <see cref="ErrorResultErrors"/>.
    /// </summary>
    public static EventId LogEventResultErrors { get; } = new(54000, nameof(ErrorResultErrors));

    /// <summary>
    /// Event id for <see cref="CriticalResultCriticalErrors"/>.
    /// </summary>
    public static EventId LogEventResultCriticalErrors { get; } = new(55003, nameof(CriticalResultCriticalErrors));

    /// <summary>
    /// Event id for <see cref="InformationAddedSeedItem"/>.
    /// </summary>
    public static EventId LogEventAddedSeedItem { get; } = new(52001, nameof(InformationAddedSeedItem));

    /// <summary>
    /// Event id for <see cref="InformationUpdatedSeedItem"/>.
    /// </summary>
    public static EventId LogEventUpdatedSeedItem { get; } = new(52002, nameof(InformationUpdatedSeedItem));

    /// <summary>
    /// Event id for <see cref="InformationPendingDeletionUsersNumber"/>.
    /// </summary>
    public static EventId LogEventPendingDeletionUsersNumber { get; } = new(52003, nameof(InformationPendingDeletionUsersNumber));

    /// <summary>
    /// Event id for <see cref="InformationOutdatedTokensNumber"/>.
    /// </summary>
    public static EventId LogEventOutdatedTokensNumber { get; } = new(52004, nameof(InformationOutdatedTokensNumber));

    /// <summary>
    /// Event id for <see cref="InformationUnconfirmedUsersNumber"/>.
    /// </summary>
    public static EventId LogEventUnconfirmedUsersNumber { get; } = new(52005, nameof(InformationUnconfirmedUsersNumber));

    /// <summary>
    /// Event id for <see cref="InformationWorkerRunning"/>.
    /// </summary>
    public static EventId LogEventWorkerRunning { get; } = new(52006, nameof(InformationWorkerRunning));

    /// <summary>
    /// Event id for <see cref="WarningDatabaseEntrySeedFailure"/>.
    /// </summary>
    public static EventId LogEventDatabaseEntrySeedFailure { get; } = new(53000, nameof(WarningDatabaseEntrySeedFailure));

    /// <summary>
    /// Event id for <see cref="CriticalClaimsAdditionFailure"/>.
    /// </summary>
    public static EventId LogEventClaimsAdditionFailure { get; } = new(55004, nameof(CriticalClaimsAdditionFailure));

    /// <summary>
    /// Event id for <see cref="CriticalUnhandledExceptionOccurred"/>.
    /// </summary>
    public static EventId LogEventUnhandledExceptionOccurred { get; } = new(55005, nameof(CriticalUnhandledExceptionOccurred));

    /// <summary>
    /// Event id for <see cref="CriticalUnsuccessfulUserDeletionAfterFailedInvitation"/>.
    /// </summary>
    public static EventId LogEventUnsuccessfulUserDeletionAfterFailedInvitation { get; } = new(55006, nameof(CriticalUnsuccessfulUserDeletionAfterFailedInvitation));
    #endregion
}