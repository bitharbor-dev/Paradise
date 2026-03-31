using Paradise.Domain.Base.Events.Base;
using System.Globalization;

namespace Paradise.Domain.Events.Identity.Users;

/// <summary>
/// A domain event that is occurring upon successful user registration.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRegisteredEvent"/> class.
/// </remarks>
/// <param name="occurredOn">
/// The timestamp indicating when the domain event occurred.
/// </param>
/// <param name="emailAddress">
/// The email address of the newly registered user.
/// </param>
/// <param name="emailConfirmationToken">
/// The token used to confirm the email address.
/// </param>
/// <param name="userCulture">
/// The culture used during user's registration.
/// </param>
public sealed class UserRegisteredEvent(DateTimeOffset occurredOn,
                                        string emailAddress,
                                        string emailConfirmationToken,
                                        CultureInfo userCulture) : DomainEventBase(occurredOn)
{
    #region Properties
    /// <summary>
    /// The email address of the newly registered user.
    /// </summary>
    public string EmailAddress { get; } = emailAddress;

    /// <summary>
    /// The token used to confirm the email address.
    /// </summary>
    public string EmailConfirmationToken { get; } = emailConfirmationToken;

    /// <summary>
    /// The culture used during user's registration.
    /// </summary>
    public CultureInfo UserCulture { get; } = userCulture;
    #endregion
}