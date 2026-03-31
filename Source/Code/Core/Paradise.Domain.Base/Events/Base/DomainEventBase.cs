using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.Events.Base;

/// <summary>
/// Base <see cref="IDomainEvent"/> implementation.
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventBase"/> class.
    /// </summary>
    /// <param name="occurredOn">
    /// The timestamp indicating when the domain event occurred.
    /// </param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors can not be protected.")]
    protected DomainEventBase(DateTimeOffset occurredOn)
        => OccurredOn = occurredOn;
    #endregion

    #region Properties
    /// <inheritdoc/>
    public DateTimeOffset OccurredOn { get; }
    #endregion
}