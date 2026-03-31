using Microsoft.Extensions.Logging;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;

/// <summary>
/// Provides event data for the <see cref="FakeLogger{T}.MessageLogged"/> event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MessageLoggedEventArgs"/> class.
/// </remarks>
/// <param name="logLevel">
/// Log level.
/// </param>
/// <param name="categoryName">
/// Category name.
/// </param>
/// <param name="eventId">
/// Event id.
/// </param>
/// <param name="formattedMessage">
/// Formatted log message.
/// </param>
/// <param name="rawMessage">
/// Raw log message.
/// </param>
/// <param name="exception">
/// Exception.
/// </param>
public sealed class MessageLoggedEventArgs(LogLevel logLevel, string categoryName,
                                           EventId eventId, string formattedMessage,
                                           string rawMessage, Exception? exception) : EventArgs
{
    #region Properties
    /// <summary>
    /// Log level.
    /// </summary>
    public LogLevel LogLevel { get; } = logLevel;

    /// <summary>
    /// Category name.
    /// </summary>
    public string CategoryName { get; } = categoryName;

    /// <summary>
    /// Event Id.
    /// </summary>
    public EventId EventId { get; } = eventId;

    /// <summary>
    /// Formatted log message.
    /// </summary>
    public string FormattedMessage { get; } = formattedMessage;

    /// <summary>
    /// Raw log message.
    /// </summary>
    public string RawMessage { get; } = rawMessage;

    /// <summary>
    /// Exception.
    /// </summary>
    public Exception? Exception { get; } = exception;
    #endregion
}