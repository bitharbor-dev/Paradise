using Paradise.Domain.Base.Events.Base;
using System.Globalization;

namespace Paradise.Domain.Events.Identity.Users;

/// <summary>
/// A domain event that is occurring upon user's email address reset request completion.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailAddressResetCompletedEvent"/> class.
/// </remarks>
/// <param name="occurredOn">
/// The timestamp indicating when the domain event occurred.
/// </param>
/// <param name="userName">
/// The user-name of the user whose email address was reset.
/// </param>
/// <param name="newEmailAddress">
/// The new email address.
/// </param>
/// <param name="oldEmailAddress">
/// The old email address.
/// </param>
/// <param name="userCulture">
/// The culture used during user's email address reset request completion.
/// </param>
public sealed class EmailAddressResetCompletedEvent(DateTimeOffset occurredOn,
                                                    string userName,
                                                    string newEmailAddress,
                                                    string oldEmailAddress,
                                                    CultureInfo userCulture) : DomainEventBase(occurredOn)
{
    #region Properties
    /// <summary>
    /// The user-name of the user whose email address was reset.
    /// </summary>
    public string UserName { get; } = userName;

    /// <summary>
    /// The new email address.
    /// </summary>
    public string NewEmailAddress { get; } = newEmailAddress;

    /// <summary>
    /// The old email address.
    /// </summary>
    public string OldEmailAddress { get; } = oldEmailAddress;

    /// <summary>
    /// The culture used during user's email address reset request completion.
    /// </summary>
    public CultureInfo UserCulture { get; } = userCulture;
    #endregion
}