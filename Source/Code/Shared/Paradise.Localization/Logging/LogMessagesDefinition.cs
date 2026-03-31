using Microsoft.Extensions.Logging;

namespace Paradise.Localization.Logging;

/// <summary>
/// Contains a runtime-defined actions to be executed while logging.
/// </summary>
public static class LogMessagesDefinition
{
    #region Properties
    /// <summary>
    /// Contains <see cref="EventId"/> instances used by the application.
    /// </summary>
    /// <remarks>
    /// <strong>There is a convention on how to define a new event id:</strong>
    /// <list type="number">
    /// <item>
    /// All numbers start from 5 to leave some capacity
    /// for events being logged from packages used by the application.
    /// </item>
    /// <item>
    /// The second number is an integer representation of the <see cref="LogLevel"/> <see langword="enum"/>.
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
    /// </item>
    /// <item>
    /// The rest of the numbers can be used freely,
    /// but number 55 should be avoided
    /// (apart from the first two digits)
    /// to easily find critical errors in the logs.
    /// </item>
    /// </list>
    /// </remarks>
    private static Dictionary<string, EventId> Map { get; } = new()
    {
        [nameof(AddedSeedItem)] = new(52000, nameof(LogMessages.AddedSeedItem)),
        [nameof(CriticalResultErrors)] = new(55000, nameof(LogMessages.ResultErrors)),
        [nameof(DatabaseSeedFailure)] = new(55001, nameof(LogMessages.DatabaseSeedFailure)),
        [nameof(ErrorResultErrors)] = new(54000, nameof(LogMessages.ResultErrors)),
        [nameof(UnhandledExceptionOccurred)] = new(55002, nameof(LogMessages.UnhandledExceptionOccurred)),
        [nameof(UpdatedSeedItem)] = new(52001, nameof(LogMessages.UpdatedSeedItem)),
    };

    /// <summary>
    /// Gets an action to be executed while logging added seed item information.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> AddedSeedItem { get; }
        = LoggerMessage.Define<string, string>(
            LogLevel.Information,
            Map[nameof(AddedSeedItem)],
            LogMessages.AddedSeedItem);

    /// <summary>
    /// Gets an action to be executed while logging the result critical errors.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> CriticalResultErrors { get; }
        = LoggerMessage.Define<string, string>(
            LogLevel.Critical,
            Map[nameof(CriticalResultErrors)],
            LogMessages.ResultErrors);

    /// <summary>
    /// Gets an action to be executed while logging database seed failure.
    /// </summary>
    public static Action<ILogger, string?, Exception?> DatabaseSeedFailure { get; }
        = LoggerMessage.Define<string?>(
            LogLevel.Critical,
            Map[nameof(DatabaseSeedFailure)],
            LogMessages.DatabaseSeedFailure);

    /// <summary>
    /// Gets an action to be executed while logging the result errors.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> ErrorResultErrors { get; }
        = LoggerMessage.Define<string, string>(
            LogLevel.Error,
            Map[nameof(ErrorResultErrors)],
            LogMessages.ResultErrors);

    /// <summary>
    /// Gets an action to be executed while logging an unhandled exception.
    /// </summary>
    public static Action<ILogger, Exception> UnhandledExceptionOccurred { get; }
        = LoggerMessage.Define(
            LogLevel.Critical,
            Map[nameof(UnhandledExceptionOccurred)],
            LogMessages.UnhandledExceptionOccurred);

    /// <summary>
    /// Gets an action to be executed while logging updated seed item information.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> UpdatedSeedItem { get; }
        = LoggerMessage.Define<string, string>(
            LogLevel.Information,
            Map[nameof(UpdatedSeedItem)],
            LogMessages.UpdatedSeedItem);
    #endregion
}