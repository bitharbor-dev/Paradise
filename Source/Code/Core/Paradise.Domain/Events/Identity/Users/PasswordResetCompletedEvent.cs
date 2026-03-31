using Paradise.Domain.Base.Events.Base;
using System.Globalization;

namespace Paradise.Domain.Events.Identity.Users;

/// <summary>
/// A domain event that is occurring upon user's password reset request completion.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PasswordResetCompletedEvent"/> class.
/// </remarks>
/// <param name="occurredOn">
/// The timestamp indicating when the domain event occurred.
/// </param>
/// <param name="emailAddress">
/// The email address of the user whose password was reset.
/// </param>
/// <param name="userCulture">
/// The culture used during user's password reset request completion.
/// </param>
public sealed class PasswordResetCompletedEvent(DateTimeOffset occurredOn,
                                                string emailAddress,
                                                CultureInfo userCulture) : DomainEventBase(occurredOn)
{
    #region Properties
    /// <summary>
    /// The email address of the user whose password was reset.
    /// </summary>
    public string EmailAddress { get; } = emailAddress;

    /// <summary>
    /// The culture used during user's password reset request completion.
    /// </summary>
    public CultureInfo UserCulture { get; } = userCulture;
    #endregion
}