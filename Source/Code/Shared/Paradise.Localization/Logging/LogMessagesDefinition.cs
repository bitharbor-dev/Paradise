using Microsoft.Extensions.Logging;

namespace Paradise.Localization.Logging;

/// <summary>
/// Contains a runtime-defined actions to be executed while logging.
/// </summary>
public static class LogMessagesDefinition
{
    #region Properties
    /// <summary>
    /// Gets an action to be executed while logging added seed item information.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> AddedSeedItem { get; }
        = LoggerMessage.Define<string, string>(
            EventIdContainer.AddedSeedItem.Level,
            EventIdContainer.AddedSeedItem.Id,
            LogMessages.AddedSeedItem);

    /// <summary>
    /// Gets an action to be executed while logging database seed failure.
    /// </summary>
    public static Action<ILogger, string?, Exception?> DatabaseSeedFailure { get; }
        = LoggerMessage.Define<string?>(
            EventIdContainer.DatabaseSeedFailure.Level,
            EventIdContainer.DatabaseSeedFailure.Id,
            LogMessages.DatabaseSeedFailure);

    /// <summary>
    /// Gets an action to be executed while logging the identity failure.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> IdentityFailure { get; }
        = LoggerMessage.Define<string, string>(
            EventIdContainer.IdentityFailure.Level,
            EventIdContainer.IdentityFailure.Id,
            LogMessages.IdentityFailure);

    /// <summary>
    /// Gets an action to be executed while logging an unhandled exception.
    /// </summary>
    public static Action<ILogger, Exception> UnhandledExceptionOccurred { get; }
        = LoggerMessage.Define(
            EventIdContainer.UnhandledExceptionOccurred.Level,
            EventIdContainer.UnhandledExceptionOccurred.Id,
            LogMessages.UnhandledExceptionOccurred);

    /// <summary>
    /// Gets an action to be executed while logging updated seed item information.
    /// </summary>
    public static Action<ILogger, string, string, Exception?> UpdatedSeedItem { get; }
        = LoggerMessage.Define<string, string>(
            EventIdContainer.UpdatedSeedItem.Level,
            EventIdContainer.UpdatedSeedItem.Id,
            LogMessages.UpdatedSeedItem);
    #endregion
}