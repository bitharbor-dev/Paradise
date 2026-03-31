using Paradise.Domain.Base.Events.Base;
using System.Globalization;

namespace Paradise.Domain.Events.Identity.Users;

/// <summary>
/// A domain event that is occurring upon user's email address reset request initiation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailAddressResetRequestedEvent"/> class.
/// </remarks>
/// <param name="occurredOn">
/// The timestamp indicating when the domain event occurred.
/// </param>
/// <param name="userName">
/// The user-name of the user whose email address to be reset.
/// </param>
/// <param name="changeEmailAddressToken">
/// The token used to reset the email address.
/// </param>
/// <param name="currentEmailAddress">
/// The email address of the user whose email address to be reset.
/// </param>
/// <param name="newEmailAddress">
/// The new email address.
/// </param>
/// <param name="userCulture">
/// The culture used during user's email address reset request initiation.
/// </param>
public sealed class EmailAddressResetRequestedEvent(DateTimeOffset occurredOn,
                                                    string userName,
                                                    string changeEmailAddressToken,
                                                    string currentEmailAddress,
                                                    string newEmailAddress,
                                                    CultureInfo userCulture) : DomainEventBase(occurredOn)
{
    #region Properties
    /// <summary>
    /// The user-name of the user whose email address to be reset.
    /// </summary>
    public string UserName { get; } = userName;

    /// <summary>
    /// The token used to reset the email address.
    /// </summary>
    public string ChangeEmailAddressToken { get; } = changeEmailAddressToken;

    /// <summary>
    /// The email address of the user whose email address to be reset.
    /// </summary>
    public string CurrentEmailAddress { get; } = currentEmailAddress;

    /// <summary>
    /// The new email address.
    /// </summary>
    public string NewEmailAddress { get; } = newEmailAddress;

    /// <summary>
    /// The culture used during user's email address reset request initiation.
    /// </summary>
    public CultureInfo UserCulture { get; } = userCulture;
    #endregion
}