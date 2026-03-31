using Paradise.Domain.Base.Events.Base;

namespace Paradise.Domain.Events.Identity.Users;

/// <summary>
/// A domain event that is occurring upon successful user's email address confirmation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailAddressConfirmedEvent"/> class.
/// </remarks>
/// <param name="occurredOn">
/// The timestamp indicating when the domain event occurred.
/// </param>
/// <param name="userId">
/// The Id of the user whose email address was confirmed.
/// </param>
public sealed class EmailAddressConfirmedEvent(DateTimeOffset occurredOn,
                                               Guid userId) : DomainEventBase(occurredOn)
{
    #region Properties
    /// <summary>
    /// The Id of the user whose email address was confirmed.
    /// </summary>
    public Guid UserId { get; } = userId;
    #endregion
}