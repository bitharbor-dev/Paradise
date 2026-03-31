using Paradise.Domain.Base.Events.Base;
using System.Globalization;

namespace Paradise.Domain.Events.Identity.Users;

/// <summary>
/// A domain event that is occurring upon user's password reset request initiation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PasswordResetRequestedEvent"/> class.
/// </remarks>
/// <param name="occurredOn">
/// The timestamp indicating when the domain event occurred.
/// </param>
/// <param name="emailAddress">
/// The email address of the user whose password to be reset.
/// </param>
/// <param name="changePasswordToken">
/// The token used to reset the password.
/// </param>
/// <param name="userCulture">
/// The culture used during user's password reset request initiation.
/// </param>
public sealed class PasswordResetRequestedEvent(DateTimeOffset occurredOn,
                                                string emailAddress,
                                                string changePasswordToken,
                                                CultureInfo userCulture) : DomainEventBase(occurredOn)
{
    #region Properties
    /// <summary>
    /// The email address of the user whose password to be reset.
    /// </summary>
    public string EmailAddress { get; } = emailAddress;

    /// <summary>
    /// The token used to reset the password.
    /// </summary>
    public string ChangePasswordToken { get; } = changePasswordToken;

    /// <summary>
    /// The culture used during user's password reset request initiation.
    /// </summary>
    public CultureInfo UserCulture { get; } = userCulture;
    #endregion
}