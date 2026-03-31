using Microsoft.Extensions.Logging;
using Paradise.Common.Extensions;
using Paradise.Localization.Logging;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;

namespace Paradise.Common.Tests.Unit.Extensions;

/// <summary>
/// <see cref="ILoggerExtensions"/> test class.
/// </summary>
public sealed class ILoggerExtensionsTests : IDisposable
{
    #region Fields
    private readonly IList<MessageLoggedEventArgs> _loggedMessages;

    private readonly FakeLogger<ILoggerExtensionsTests> _logger;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ILoggerExtensionsTests"/> class.
    /// </summary>
    public ILoggerExtensionsTests()
    {
        _loggedMessages = [];

        _logger = new FakeLogger<ILoggerExtensionsTests>();
        _logger.MessageLogged += OnMessageLogged;

        Logger = _logger;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The <see cref="ILogger"/> instance onto which to execute
    /// methods under test.
    /// </summary>
    internal ILogger Logger { get; }

    /// <summary>
    /// Contains messages which were logged during tests.
    /// </summary>
    internal IEnumerable<MessageLoggedEventArgs> LoggedMessages
        => _loggedMessages.AsReadOnly();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => _logger.MessageLogged -= OnMessageLogged;

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogUnhandledException"/> method should
    /// log the input exception with <see cref="LogLevel.Critical"/>
    /// and event Id of <c>55002</c>.
    /// </summary>
    [Fact]
    public void LogUnhandledException()
    {
        // Arrange
        var expectedEventId = new EventId(55002, nameof(LogMessagesDefinition.UnhandledExceptionOccurred));

        var exception = new InvalidOperationException("Test exception");

        // Act
        Logger.LogUnhandledException(exception);

        // Assert
        var entry = Assert.Single(LoggedMessages);

        Assert.Equal(LogLevel.Critical, entry.LogLevel);
        Assert.Equal(expectedEventId, entry.EventId);
        Assert.Equal("An unhandled exception has occurred.", entry.RawMessage);
        Assert.IsType<InvalidOperationException>(entry.Exception);
        Assert.Contains(exception.GetType().Name, entry.FormattedMessage, StringComparison.Ordinal);
        Assert.Contains(exception.Message, entry.FormattedMessage, StringComparison.Ordinal);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// The <see cref="FakeLogger{T}.MessageLogged"/> event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MessageLoggedEventArgs"/> instance containing the event data.
    /// </param>
    private void OnMessageLogged(object? sender, MessageLoggedEventArgs e)
        => _loggedMessages.Add(e);
    #endregion
}