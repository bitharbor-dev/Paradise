using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Paradise.ApplicationLogic.Infrastructure.Extensions;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;
using static System.Text.RegularExpressions.Regex;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Extensions;

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
    /// The <see cref="ILoggerExtensions.LogIdentityResult"/> method should
    /// concatenate all errors from the input <see cref="IdentityResult"/> object
    /// and log the concatenated string with <see cref="LogLevel.Critical"/>.
    /// </summary>
    [Fact]
    public void LogIdentityResult()
    {
        // Arrange
        var error1 = new IdentityError { Description = "Error 1" };
        var error2 = new IdentityError { Description = "Error 2" };

        var identityResult = IdentityResult.Failed(error1, error2);

        // Act
        Logger.LogIdentityResult(identityResult);

        // Assert
        var message = Assert.Single(LoggedMessages);
        Assert.Equal(LogLevel.Critical, message.LogLevel);

        var newline = Escape(Environment.NewLine);

        Assert.Matches($"^Errors:{newline}(?:.+{newline})*.+$", message.RawMessage);
    }

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogIdentityResult"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="IdentityResult"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void LogIdentityResult_ThrowsOnNull()
    {
        // Arrange
        var identityResult = null as IdentityResult;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Logger.LogIdentityResult(identityResult!));
    }

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogAddedSeedItem"/> method should
    /// create a log entry about successful item seeding,
    /// containing the added item type name and friendly name.
    /// </summary>
    [Fact]
    public void LogAddedSeedItem()
    {
        // Arrange
        var itemName = "ItemName";

        // Act
        Logger.LogAddedSeedItem<object>(itemName);

        // Assert
        var message = Assert.Single(LoggedMessages);
        Assert.Equal(LogLevel.Information, message.LogLevel);

        var escapedName = Escape(itemName);
        var escapedType = Escape(nameof(Object));

        Assert.Matches($"^Added seed item {escapedType}: '{escapedName}'\\.$", message.RawMessage);
    }

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogUpdatedSeedItem"/> method should
    /// create a log entry about successful item update,
    /// containing the updated item type name and friendly name.
    /// </summary>
    [Fact]
    public void LogUpdatedSeedItem()
    {
        // Arrange
        var itemName = "ItemName";

        // Act
        Logger.LogUpdatedSeedItem<object>(itemName);

        // Assert
        var message = Assert.Single(LoggedMessages);
        Assert.Equal(LogLevel.Information, message.LogLevel);

        var escapedName = Escape(itemName);
        var escapedType = Escape(nameof(Object));

        Assert.Matches($"^Updated seed item {escapedType}: '{escapedName}'\\.$", message.RawMessage);
    }

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogUpdatedSeedItem"/> method should
    /// create a log entry about unsuccessful seeding,
    /// containing the exception information and failed method name.
    /// </summary>
    [Fact]
    public void LogDatabaseSeedFailure()
    {
        // Arrange
        var exception = new InvalidOperationException();
        var methodName = nameof(ILoggerExtensions.LogDatabaseSeedFailure);

        // Act
        Logger.LogDatabaseSeedFailure(exception, methodName);

        // Assert
        var message = Assert.Single(LoggedMessages);
        Assert.Equal(LogLevel.Critical, message.LogLevel);
        Assert.Same(exception, message.Exception);

        var escapedName = Escape(methodName);

        Assert.Matches($"^Failed to seed the database. Method '{escapedName}'\\.$", message.RawMessage);
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