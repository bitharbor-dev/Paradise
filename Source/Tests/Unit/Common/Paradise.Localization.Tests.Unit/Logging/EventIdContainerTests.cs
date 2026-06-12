using Microsoft.Extensions.Logging;
using Paradise.Localization.Logging;

namespace Paradise.Localization.Tests.Unit.Logging;

/// <summary>
/// <see cref="EventIdContainer"/> test class.
/// </summary>
public sealed class EventIdContainerTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="EventIdContainer.AddedSeedItem"/> property should
    /// return the <see cref="LoggedEvent"/> with the event ID set to <c>52000</c>
    /// and logging level <see cref="LogLevel.Information"/>.
    /// </summary>
    [Fact]
    public void AddedSeedItem()
    {
        // Arrange
        var loggedEvent = EventIdContainer.AddedSeedItem;

        // Act
        var id = loggedEvent.Id;
        var level = loggedEvent.Level;

        // Assert
        Assert.Equal(52_000, id);
        Assert.Equal(LogLevel.Information, level);
    }

    /// <summary>
    /// The <see cref="EventIdContainer.DatabaseSeedFailure"/> property should
    /// return the <see cref="LoggedEvent"/> with the event ID set to <c>55001</c>
    /// and logging level <see cref="LogLevel.Critical"/>.
    /// </summary>
    [Fact]
    public void DatabaseSeedFailure()
    {
        // Arrange
        var loggedEvent = EventIdContainer.DatabaseSeedFailure;

        // Act
        var id = loggedEvent.Id;
        var level = loggedEvent.Level;

        // Assert
        Assert.Equal(55_001, id);
        Assert.Equal(LogLevel.Critical, level);
    }

    /// <summary>
    /// The <see cref="EventIdContainer.IdentityFailure"/> property should
    /// return the <see cref="LoggedEvent"/> with the event ID set to <c>55002</c>
    /// and logging level <see cref="LogLevel.Critical"/>.
    /// </summary>
    [Fact]
    public void IdentityFailure()
    {
        // Arrange
        var loggedEvent = EventIdContainer.IdentityFailure;

        // Act
        var id = loggedEvent.Id;
        var level = loggedEvent.Level;

        // Assert
        Assert.Equal(55_002, id);
        Assert.Equal(LogLevel.Critical, level);
    }

    /// <summary>
    /// The <see cref="EventIdContainer.UnhandledExceptionOccurred"/> property should
    /// return the <see cref="LoggedEvent"/> with the event ID set to <c>55000</c>
    /// and logging level <see cref="LogLevel.Critical"/>.
    /// </summary>
    [Fact]
    public void UnhandledExceptionOccurred()
    {
        // Arrange
        var loggedEvent = EventIdContainer.UnhandledExceptionOccurred;

        // Act
        var id = loggedEvent.Id;
        var level = loggedEvent.Level;

        // Assert
        Assert.Equal(55_000, id);
        Assert.Equal(LogLevel.Critical, level);
    }

    /// <summary>
    /// The <see cref="EventIdContainer.UpdatedSeedItem"/> property should
    /// return the <see cref="LoggedEvent"/> with the event ID set to <c>52001</c>
    /// and logging level <see cref="LogLevel.Information"/>.
    /// </summary>
    [Fact]
    public void UpdatedSeedItem()
    {
        // Arrange
        var loggedEvent = EventIdContainer.UpdatedSeedItem;

        // Act
        var id = loggedEvent.Id;
        var level = loggedEvent.Level;

        // Assert
        Assert.Equal(52_001, id);
        Assert.Equal(LogLevel.Information, level);
    }
    #endregion
}