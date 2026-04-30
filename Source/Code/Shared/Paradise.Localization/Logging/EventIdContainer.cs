using Microsoft.Extensions.Logging;

namespace Paradise.Localization.Logging;

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
public static class EventIdContainer
{
    #region Properties
    /// <summary>
    /// <see cref="LogMessagesDefinition.AddedSeedItem"/> event identifier.
    /// </summary>
    public static LoggedEvent AddedSeedItem { get; } = new(new(52_000, nameof(AddedSeedItem)), LogLevel.Information);

    /// <summary>
    /// <see cref="LogMessagesDefinition.DatabaseSeedFailure"/> event identifier.
    /// </summary>
    public static LoggedEvent DatabaseSeedFailure { get; } = new(new(55_003, nameof(DatabaseSeedFailure)), LogLevel.Critical);

    /// <summary>
    /// <see cref="LogMessagesDefinition.FallbackHandlerReached"/> event identifier.
    /// </summary>
    public static LoggedEvent FallbackHandlerReached { get; } = new(new(55_001, nameof(FallbackHandlerReached)), LogLevel.Critical);

    /// <summary>
    /// <see cref="LogMessagesDefinition.IdentityFailure"/> event identifier.
    /// </summary>
    public static LoggedEvent IdentityFailure { get; } = new(new(55_002, nameof(IdentityFailure)), LogLevel.Critical);

    /// <summary>
    /// <see cref="LogMessagesDefinition.UnhandledExceptionOccurred"/> event identifier.
    /// </summary>
    public static LoggedEvent UnhandledExceptionOccurred { get; } = new(new(55_000, nameof(UnhandledExceptionOccurred)), LogLevel.Critical);

    /// <summary>
    /// <see cref="LogMessagesDefinition.UpdatedSeedItem"/> event identifier.
    /// </summary>
    public static LoggedEvent UpdatedSeedItem { get; } = new(new(52_001, nameof(UpdatedSeedItem)), LogLevel.Information);
    #endregion
}