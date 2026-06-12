using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Paradise.ApplicationLogic.Infrastructure.Extensions;
using Paradise.Localization.Logging;
using Paradise.Tests.Doubles.Fakes.Microsoft.Extensions.Logging;

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
    /// The <see cref="ILoggerExtensions.LogIdentityFailure"/> method should
    /// concatenate all errors from the input <see cref="IdentityResult"/> object
    /// and log the concatenated string with <see cref="LogLevel"/>
    /// equal to <see cref="EventIdContainer.IdentityFailure"/> log level.
    /// </summary>
    [Fact]
    public void LogIdentityFailure()
    {
        // Arrange
        var error1 = new IdentityError { Description = "Error 1" };
        var error2 = new IdentityError { Description = "Error 2" };

        var identityResult = IdentityResult.Failed(error1, error2);

        // Act
        Logger.LogIdentityFailure(identityResult);

        // Assert
        var entry = Assert.Single(LoggedMessages);

        Assert.Equal(EventIdContainer.IdentityFailure.Level, entry.LogLevel);
        Assert.Equal(EventIdContainer.IdentityFailure.Id, entry.EventId);

        Assert.Contains(error1.Description, entry.Message, StringComparison.Ordinal);
        Assert.Contains(error2.Description, entry.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogIdentityFailure"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="IdentityResult"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void LogIdentityFailure_ThrowsOnNull()
    {
        // Arrange
        var identityResult = null as IdentityResult;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Logger.LogIdentityFailure(identityResult!));
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
        var typeName = nameof(Object);

        // Act
        Logger.LogAddedSeedItem<object>(itemName);

        // Assert
        var entry = Assert.Single(LoggedMessages);

        Assert.Equal(EventIdContainer.AddedSeedItem.Level, entry.LogLevel);
        Assert.Equal(EventIdContainer.AddedSeedItem.Id, entry.EventId);

        Assert.Contains(typeName, entry.Message, StringComparison.Ordinal);
        Assert.Contains(itemName, entry.Message, StringComparison.Ordinal);
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
        var typeName = nameof(Object);

        // Act
        Logger.LogUpdatedSeedItem<object>(itemName);

        // Assert
        var entry = Assert.Single(LoggedMessages);

        Assert.Equal(EventIdContainer.UpdatedSeedItem.Level, entry.LogLevel);
        Assert.Equal(EventIdContainer.UpdatedSeedItem.Id, entry.EventId);

        Assert.Contains(typeName, entry.Message, StringComparison.Ordinal);
        Assert.Contains(itemName, entry.Message, StringComparison.Ordinal);
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
        var entry = Assert.Single(LoggedMessages);

        Assert.Equal(EventIdContainer.DatabaseSeedFailure.Level, entry.LogLevel);
        Assert.Equal(EventIdContainer.DatabaseSeedFailure.Id, entry.EventId);

        Assert.Contains(exception.Message, entry.FullLogMessage, StringComparison.Ordinal);
        Assert.Contains(methodName, entry.Message, StringComparison.Ordinal);
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