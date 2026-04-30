using Microsoft.Extensions.Logging;
using System.Text;
using Xunit;
using static System.Environment;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;

/// <summary>
/// Fake <see cref="ILogger{TCategoryName}"/> implementation.
/// </summary>
public sealed class FakeLogger<T> : ILogger<T>
{
    #region Constants
    /// <summary>
    /// Trace log level prefix.
    /// </summary>
    private const string TraceLevel = "trce";
    /// <summary>
    /// Debug log level prefix.
    /// </summary>
    private const string DebugLevel = "dbug";
    /// <summary>
    /// Information log level prefix.
    /// </summary>
    private const string InformationLevel = "info";
    /// <summary>
    /// Warning log level prefix.
    /// </summary>
    private const string WarningLevel = "warn";
    /// <summary>
    /// Error log level prefix.
    /// </summary>
    private const string ErrorLevel = "fail";
    /// <summary>
    /// Critical log level prefix.
    /// </summary>
    private const string CriticalLevel = "crit";
    /// <summary>
    /// Default log level padding.
    /// </summary>
    private const string LogLevelPadding = ": ";
    /// <summary>
    /// Opening brace to wrap log event id.
    /// </summary>
    private const char OpenBrace = '[';
    /// <summary>
    /// Closing brace to wrap log event id.
    /// </summary>
    private const char CloseBrace = ']';
    #endregion

    #region Fields
    private static readonly int _maxLogLevelLength = new[]
    {
        TraceLevel.Length,
        DebugLevel.Length,
        InformationLevel.Length,
        WarningLevel.Length,
        ErrorLevel.Length,
        CriticalLevel.Length
    }.Max();

    private static readonly string _messagePadding = new(' ', _maxLogLevelLength + LogLevelPadding.Length);
    private static readonly string _newLineWithMessagePadding = NewLine + _messagePadding;
    private static readonly string _categoryName = typeof(T).FullName ?? typeof(T).Name;

    private readonly Dictionary<LogLevel, bool> _isEnabledMap = new()
    {
        { LogLevel.Trace,       true },
        { LogLevel.Debug,       true },
        { LogLevel.Information, true },
        { LogLevel.Warning,     true },
        { LogLevel.Error,       true },
        { LogLevel.Critical,    true },
        { LogLevel.None,        false }
    };
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => null;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
        => _isEnabledMap[logLevel];

    /// <summary>
    /// Enables logging for the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <param name="logLevel">
    /// The <see cref="LogLevel"/> for which logging should be enabled.
    /// </param>
    public void EnableFor(LogLevel logLevel)
        => _isEnabledMap[logLevel] = true;

    /// <summary>
    /// Disables logging for the specified <paramref name="logLevel"/>.
    /// </summary>
    /// <param name="logLevel">
    /// The <see cref="LogLevel"/> for which logging should be disabled.
    /// </param>
    public void DisableFor(LogLevel logLevel)
        => _isEnabledMap[logLevel] = false;

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);

        var message = formatter(state, exception);

        if (!string.IsNullOrWhiteSpace(message) || exception is not null)
        {
            var fullLogMessage = CreateLog(logLevel, _categoryName, eventId, message, exception);

            TestContext.Current.TestOutputHelper?.WriteLine(fullLogMessage);

            var eventArgs = new MessageLoggedEventArgs(logLevel,
                                                       _categoryName,
                                                       eventId,
                                                       fullLogMessage,
                                                       message,
                                                       exception);

            MessageLogged?.Invoke(this, eventArgs);
        }
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates the log message.
    /// </summary>
    /// <param name="logLevel">
    /// Log level.
    /// </param>
    /// <param name="categoryName">
    /// Category name.
    /// </param>
    /// <param name="eventId">
    /// Event id.
    /// </param>
    /// <param name="message">
    /// Message to be written.
    /// </param>
    /// <param name="exception">
    /// Exception.
    /// </param>
    private static string CreateLog(LogLevel logLevel, string categoryName, EventId eventId, string message, Exception? exception)
    {
        var logLevelString = GetLogLevelString(logLevel);

        var builder = new StringBuilder()
            .Append(LogLevelPadding)
            .Append(categoryName)
            .Append(OpenBrace)
            .Append(eventId)
            .Append(CloseBrace)
            .AppendLine();

        if (!string.IsNullOrWhiteSpace(message))
        {
            builder.Append(_messagePadding);

            var currentTextLength = builder.Length;

            builder.AppendLine(message);
            builder.Replace(NewLine, _newLineWithMessagePadding, currentTextLength, builder.Length - currentTextLength);
        }

        if (exception is not null)
            builder.AppendLine(exception.ToString());

        if (!string.IsNullOrWhiteSpace(logLevelString))
            builder.Insert(0, logLevelString);

        return builder.ToString();
    }

    /// <summary>
    /// Gets the log level prefix string
    /// depending on the input <paramref name="logLevel"/>.
    /// </summary>
    /// <param name="logLevel">
    /// <see cref="LogLevel"/> value to determine the prefix.
    /// </param>
    /// <returns>
    /// A string representation of the input <paramref name="logLevel"/>.
    /// </returns>
    private static string GetLogLevelString(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace => TraceLevel,
        LogLevel.Debug => DebugLevel,
        LogLevel.Information => InformationLevel,
        LogLevel.Warning => WarningLevel,
        LogLevel.Error => ErrorLevel,
        LogLevel.Critical => CriticalLevel,
        LogLevel.None => string.Empty,
        _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
    };
    #endregion

    #region Events
    /// <summary>
    /// Occurs when a new message is logged.
    /// </summary>
    public event EventHandler<MessageLoggedEventArgs>? MessageLogged;
    #endregion
}