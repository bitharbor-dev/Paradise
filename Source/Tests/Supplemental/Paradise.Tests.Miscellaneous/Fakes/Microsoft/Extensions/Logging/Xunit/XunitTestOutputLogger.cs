using Microsoft.Extensions.Logging;
using System.Text;
using Xunit.Abstractions;
using static System.Environment;

namespace Paradise.Tests.Miscellaneous.Fakes.Microsoft.Extensions.Logging.Xunit;

/// <summary>
/// Provides log-to-_output functionality for Xunit tests.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="XunitTestOutputLogger{T}"/> class.
/// </remarks>
/// <param name="output">
/// Xunit _output helper.
/// </param>
internal sealed class XunitTestOutputLogger<T>(ITestOutputHelper output) : ILogger<T>
{
    #region Constants
    /// <summary>
    /// Trace log level prefix.
    /// </summary>
    private const string TraceLogLevel = "trce";
    /// <summary>
    /// Debug log level prefix.
    /// </summary>
    private const string DebugLogLevel = "dbug";
    /// <summary>
    /// Information log level prefix.
    /// </summary>
    private const string InformationLogLevel = "info";
    /// <summary>
    /// Warning log level prefix.
    /// </summary>
    private const string WarningLogLevel = "warn";
    /// <summary>
    /// Error log level prefix.
    /// </summary>
    private const string ErrorLogLevel = "fail";
    /// <summary>
    /// Critical log level prefix.
    /// </summary>
    private const string CriticalLogLevel = "crit";
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

    private const ushort DefaultStringBuilderCapacity = 1024;
    #endregion

    #region Fields
    [ThreadStatic]
    private static StringBuilder? _logBuilder;

    private static readonly string _messagePadding = new(' ', GetLogLevelString(default).Length + LogLevelPadding.Length);
    private static readonly string _newLineWithMessagePadding = NewLine + _messagePadding;

    private readonly ITestOutputHelper _output = output;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => Scope.Instance;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
        => true;

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);

        if (!string.IsNullOrWhiteSpace(message) || exception != null)
            WriteMessage(logLevel, typeof(T).Name, eventId.Id, message, exception);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Writes the log message.
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
    private void WriteMessage(LogLevel logLevel, string categoryName, EventId eventId, string? message, Exception? exception)
    {
        var builder = _logBuilder;
        _logBuilder = null;

        builder ??= new();

        var logLevelString = GetLogLevelString(logLevel);

        builder
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
            builder.Replace(NewLine, _newLineWithMessagePadding, currentTextLength, message.Length);
        }

        if (exception is not null)
            builder.AppendLine(exception.ToString());

        if (builder.Length > 0)
        {
            if (!string.IsNullOrWhiteSpace(logLevelString))
                builder.Insert(0, logLevelString);

            try
            {
                _output.WriteLine(builder.ToString().Trim());
            }
            catch (InvalidOperationException) { }

            builder.Clear();

            if (builder.Capacity > DefaultStringBuilderCapacity)
                builder.Capacity = DefaultStringBuilderCapacity;
        }

        _logBuilder = builder;
    }

    /// <summary>
    /// Gets the log level prefix string depending on the input <paramref name="logLevel"/>.
    /// </summary>
    /// <param name="logLevel">
    /// <see cref="LogLevel"/> value to be used to get the prefix string.
    /// </param>
    /// <returns>
    /// A string representation of the input <paramref name="logLevel"/>.
    /// </returns>
    private static string GetLogLevelString(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace => TraceLogLevel,
        LogLevel.Debug => DebugLogLevel,
        LogLevel.Information => InformationLogLevel,
        LogLevel.Warning => WarningLogLevel,
        LogLevel.Error => ErrorLogLevel,
        LogLevel.Critical => CriticalLogLevel,
        LogLevel.None => string.Empty,
        _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
    };
    #endregion

    #region Nested types
    /// <summary>
    /// Fake <see cref="IDisposable"/> implementation.
    /// </summary>
    private sealed class Scope : IDisposable
    {
        #region Public methods
        /// <inheritdoc/>
        public void Dispose() { }
        #endregion

        #region Properties
        /// <summary>
        /// Singleton <see cref="Scope"/> instance.
        /// </summary>
        public static Scope Instance { get; } = new();
        #endregion
    }
    #endregion
}