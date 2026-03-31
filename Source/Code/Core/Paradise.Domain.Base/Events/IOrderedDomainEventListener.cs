namespace Paradise.Domain.Base.Events;

/// <summary>
/// Defines a listener that reacts to a specific type of domain event in an ordered manner.
/// </summary>
/// <typeparam name="TEvent">
/// The type of domain event this listener handles.
/// </typeparam>
public interface IOrderedDomainEventListener<TEvent> : IDomainEventListener<TEvent>
    where TEvent : IDomainEvent
{
    #region Properties
    /// <summary>
    /// Defines the processing order of this event listener.
    /// <para>
    /// Lower values indicate higher priority and are processed earlier.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If multiple listeners specify the same value, they will be processed concurrently.
    /// </remarks>
    int ProcessingOrder { get; }
    #endregion
}