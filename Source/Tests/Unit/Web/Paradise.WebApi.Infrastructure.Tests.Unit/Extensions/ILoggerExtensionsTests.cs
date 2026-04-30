using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paradise.Localization.Logging;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;
using Paradise.WebApi.Infrastructure.Extensions;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Extensions;

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
    /// The <see cref="ILoggerExtensions.LogFallbackHandlerInvocation"/> method should
    /// create a log entry about fallback exception handler being reached
    /// by the exception handling middleware, containing information about
    /// the HTTP request caused such outcome.
    /// </summary>
    [Fact]
    public void LogFallbackHandlerInvocation()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "trace-id",
            Request =
            {
                Method = "POST",
                Scheme = "HTTPS",
                Host = new("host"),
                Path = new("/resource"),
                QueryString = new QueryString("?parameter=value")
            }
        };

        // Act
        Logger.LogFallbackHandlerInvocation(context.Request);

        // Assert
        var entry = Assert.Single(LoggedMessages);

        Assert.Equal(EventIdContainer.FallbackHandlerReached.Level, entry.LogLevel);
        Assert.Equal(EventIdContainer.FallbackHandlerReached.Id, entry.EventId);

        Assert.Contains(context.TraceIdentifier, entry.Message, StringComparison.Ordinal);
        Assert.Contains(context.Request.Method, entry.Message, StringComparison.Ordinal);
        Assert.Contains(context.Request.Scheme, entry.Message, StringComparison.Ordinal);
        Assert.Contains(context.Request.Host.ToString(), entry.Message, StringComparison.Ordinal);
        Assert.Contains(context.Request.Path.ToString(), entry.Message, StringComparison.Ordinal);
        Assert.Contains(context.Request.QueryString.ToString(), entry.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogFallbackHandlerInvocation"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="HttpRequest"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void LogFallbackHandlerInvocation_ThrowsOnNullRequest()
    {
        // Arrange
        var request = null as HttpRequest;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Logger.LogFallbackHandlerInvocation(request!));
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