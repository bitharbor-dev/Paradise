using Paradise.Domain.Base.Events.Base;
using System.Globalization;

namespace Paradise.Domain.Events.Identity.Users;

/// <summary>
/// A domain event that is occurring upon user's two-factor authentication.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TwoFactorAuthenticationOccurringEvent"/> class.
/// </remarks>
/// <param name="occurredOn">
/// The timestamp indicating when the domain event occurred.
/// </param>
/// <param name="emailAddress">
/// The email address of the user who is performing authentication.
/// </param>
/// <param name="verificationCode">
/// Two-factor verification code.
/// </param>
/// <param name="userCulture">
/// The culture used during user's two-factor authentication.
/// </param>
public sealed class TwoFactorAuthenticationOccurringEvent(DateTimeOffset occurredOn,
                                                          string emailAddress,
                                                          string verificationCode,
                                                          CultureInfo userCulture) : DomainEventBase(occurredOn)
{
    #region Properties
    /// <summary>
    /// The email address of the user who is performing authentication.
    /// </summary>
    public string EmailAddress { get; } = emailAddress;

    /// <summary>
    /// Two-factor verification code.
    /// </summary>
    public string VerificationCode { get; } = verificationCode;

    /// <summary>
    /// The culture used during user's two-factor authentication.
    /// </summary>
    public CultureInfo UserCulture { get; } = userCulture;
    #endregion
}