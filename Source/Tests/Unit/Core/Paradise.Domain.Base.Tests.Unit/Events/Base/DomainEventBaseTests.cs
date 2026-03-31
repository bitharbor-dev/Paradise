using Paradise.Domain.Base.Events;
using Paradise.Domain.Base.Events.Base;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Events;

namespace Paradise.Domain.Base.Tests.Unit.Events.Base;

/// <summary>
/// <see cref="DomainEventBase"/> test class.
/// </summary>
public sealed class DomainEventBaseTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="DomainEventBase"/> constructor should
    /// successfully create a new instance of the class and
    /// populate the <see cref="IDomainEvent.OccurredOn"/> property.
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // Arrange
        var occurredOn = DateTimeOffset.UnixEpoch;

        // Act
        var domainEvent = new TestDomainEvent(occurredOn);

        // Assert
        Assert.Equal(occurredOn, domainEvent.OccurredOn);
    }
    #endregion
}