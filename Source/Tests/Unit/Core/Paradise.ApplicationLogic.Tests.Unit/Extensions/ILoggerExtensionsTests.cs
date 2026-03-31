using Microsoft.Extensions.Logging;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Models;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;

namespace Paradise.ApplicationLogic.Tests.Unit.Extensions;

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

    /// <summary>
    /// Provides member data for <see cref="LogResultErrors_SkipsEmptyErrors"/> method.
    /// </summary>
    public static TheoryData<bool[], bool?> LogResultErrors_SkipsEmptyErrors_MemberData { get; } = new()
    {
        { Array.Empty<bool>(),  null    },
        { new[] { true },       false   },
        { new[] { false },      true    }
    };
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => _logger.MessageLogged -= OnMessageLogged;

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogResultErrors"/> method should
    /// concatenate the input errors and log the concatenated string
    /// with the corresponding <see cref="LogLevel"/>.
    /// </summary>
    [Fact]
    public void LogResultErrors_LogsAllErrors()
    {
        // Arrange
        var description1 = "Error 1";
        var description2 = "Error 2";

        var errors = new[]
        {
            new ApplicationError(description1, true),
            new ApplicationError(description2, false)
        };

        // Act
        Logger.LogResultErrors(errors, null, null);

        // Assert
        var loggedMessage = Assert.Single(LoggedMessages);
        Assert.Contains(description1, loggedMessage.RawMessage, StringComparison.Ordinal);
        Assert.Contains(description2, loggedMessage.RawMessage, StringComparison.Ordinal);
    }

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogResultErrors"/> method should
    /// filter out the non-critical input errors,
    /// concatenate filtered errors and log the concatenated string
    /// with the <see cref="LogLevel.Critical"/> level.
    /// </summary>
    [Fact]
    public void LogResultErrors_LogsCriticalErrors()
    {
        // Arrange
        var description1 = "Error 1";
        var description2 = "Error 2";

        var errors = new[]
        {
            new ApplicationError(description1, true),
            new ApplicationError(description2, false)
        };

        // Act
        Logger.LogResultErrors(errors, null, true);

        // Assert
        var loggedMessage = Assert.Single(LoggedMessages);
        Assert.Equal(LogLevel.Critical, loggedMessage.LogLevel);
        Assert.Contains(description1, loggedMessage.RawMessage, StringComparison.Ordinal);
        Assert.DoesNotContain(description2, loggedMessage.RawMessage, StringComparison.Ordinal);
    }

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogResultErrors"/> method should
    /// filter out the critical input errors,
    /// concatenate filtered errors and log the concatenated string
    /// with the <see cref="LogLevel.Error"/> level.
    /// </summary>
    [Fact]
    public void LogResultErrors_LogsNonCriticalErrors()
    {
        // Arrange
        var description1 = "Error 1";
        var description2 = "Error 2";

        var errors = new[]
        {
            new ApplicationError(description1, true),
            new ApplicationError(description2, false)
        };

        // Act
        Logger.LogResultErrors(errors, null, false);

        // Assert
        var loggedMessage = Assert.Single(LoggedMessages);
        Assert.Equal(LogLevel.Error, loggedMessage.LogLevel);
        Assert.DoesNotContain(description1, loggedMessage.RawMessage, StringComparison.Ordinal);
        Assert.Contains(description2, loggedMessage.RawMessage, StringComparison.Ordinal);
    }

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogResultErrors"/> method should
    /// pass the input exception to the target logger instance.
    /// </summary>
    [Fact]
    public void LogResultErrors_LogsException()
    {
        // Arrange
        var errors = new[]
        {
            new ApplicationError(string.Empty, false)
        };

        var exception = new InvalidOperationException();

        // Act
        Logger.LogResultErrors(errors, exception);

        // Assert
        var loggedMessage = Assert.Single(LoggedMessages);
        Assert.Same(exception, loggedMessage.Exception);
    }

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogResultErrors"/> method should
    /// perform no logging if the initial input errors list is empty
    /// or filtered errors list is empty.
    /// </summary>
    /// <param name="isCriticalFlags">
    /// A projection of the list of errors to be logged.
    /// </param>
    /// <param name="logOnlyCritical">
    /// Defines what errors have to be included in the log:
    /// <list type="bullet">
    /// <item>
    /// <see langword="true"/> - critical only.
    /// </item>
    /// <item>
    /// <see langword="false"/> - non critical only.
    /// </item>
    /// <item>
    /// <see langword="null"/> - all errors.
    /// </item>
    /// </list>
    /// </param>
    [Theory, MemberData(nameof(LogResultErrors_SkipsEmptyErrors_MemberData))]
    public void LogResultErrors_SkipsEmptyErrors(bool[] isCriticalFlags, bool? logOnlyCritical)
    {
        // Arrange
        var errors = isCriticalFlags.Select(flag => new ApplicationError(string.Empty, flag));

        // Act
        Logger.LogResultErrors(errors, null, logOnlyCritical);

        // Assert
        Assert.Empty(LoggedMessages);
    }

    /// <summary>
    /// The <see cref="ILoggerExtensions.LogResultErrors"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// list of <see cref="ApplicationError"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void LogResultErrors_ThrowsOnNull()
    {
        // Arrange
        var errors = null as IEnumerable<ApplicationError>;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Logger.LogResultErrors(errors!, null));
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