using Microsoft.Extensions.Logging;

namespace Paradise.Tests.Doubles.Fakes.Microsoft.Extensions.Logging;

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
/// <param name="fullLogMessage">
/// Full log message.
/// <para>
/// Contains full log message with category name, log level string, exception stack trace, etc.
/// </para>
/// </param>
/// <param name="message">
/// Message.
/// <para>
/// Contains a single-row message produced by logger formatter.
/// </para>
/// </param>
/// <param name="exception">
/// Exception.
/// </param>
public sealed class MessageLoggedEventArgs(LogLevel logLevel, string categoryName,
                                           EventId eventId, string fullLogMessage,
                                           string message, Exception? exception) : EventArgs
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
    /// Event id.
    /// </summary>
    public EventId EventId { get; } = eventId;

    /// <summary>
    /// Full log message.
    /// </summary>
    /// <remarks>
    /// Contains full log message with category name, log level string, exception stack trace, etc.
    /// </remarks>
    public string FullLogMessage { get; } = fullLogMessage;

    /// <summary>
    /// Message.
    /// </summary>
    /// <remarks>
    /// Contains a single-row message produced by logger formatter.
    /// </remarks>
    public string Message { get; } = message;

    /// <summary>
    /// Exception.
    /// </summary>
    public Exception? Exception { get; } = exception;
    #endregion
}