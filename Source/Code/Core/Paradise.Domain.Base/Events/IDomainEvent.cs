namespace Paradise.Domain.Base.Events;

/// <summary>
/// Represents a domain event that captures something
/// of significance which has occurred within the domain.
/// </summary>
public interface IDomainEvent
{
    #region Properties
    /// <summary>
    /// The timestamp indicating when the domain event occurred.
    /// </summary>
    DateTimeOffset OccurredOn { get; }
    #endregion
}