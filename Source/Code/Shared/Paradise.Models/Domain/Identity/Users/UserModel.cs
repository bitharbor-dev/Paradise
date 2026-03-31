using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.Identity.Users;

/// <summary>
/// Represents an application user.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserModel"/> class.
/// </remarks>
/// <param name="id">
/// User Id.
/// </param>
/// <param name="userName">
/// User's user-name.
/// </param>
/// <param name="emailAddress">
/// User's email address.
/// </param>
/// <param name="isEmailAddressConfirmed">
/// Indicates whether the user's email address has been confirmed.
/// </param>
/// <param name="phoneNumber">
/// User's phone number.
/// </param>
/// <param name="isPhoneNumberConfirmed">
/// Indicates whether the user's phone number has been confirmed.
/// </param>
/// <param name="isPendingDeletion">
/// Indicates whether the user is pending deletion.
/// </param>
/// <param name="isTwoFactorEnabled">
/// Indicates whether two-factor authentication is enabled for the user.
/// </param>
/// <param name="registrationDate">
/// User registration date.
/// </param>
[method: JsonConstructor]
public sealed class UserModel(Guid id, string userName,
                              string emailAddress, bool isEmailAddressConfirmed,
                              string? phoneNumber, bool isPhoneNumberConfirmed,
                              bool isPendingDeletion, bool isTwoFactorEnabled,
                              DateTimeOffset registrationDate)
{
    #region Properties
    /// <summary>
    /// User Id.
    /// </summary>
    public Guid Id { get; } = id;

    /// <summary>
    /// User's user-name.
    /// </summary>
    [DataType(DataType.Text)]
    public string UserName { get; } = userName;

    /// <summary>
    /// User's email address.
    /// </summary>
    [EmailAddress, DataType(DataType.EmailAddress)]
    public string EmailAddress { get; } = emailAddress;

    /// <summary>
    /// Indicates whether the user's email address has been confirmed.
    /// </summary>
    public bool IsEmailAddressConfirmed { get; } = isEmailAddressConfirmed;

    /// <summary>
    /// User's phone number.
    /// </summary>
    [Phone, DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; } = phoneNumber;

    /// <summary>
    /// Indicates whether the user's phone number has been confirmed.
    /// </summary>
    public bool IsPhoneNumberConfirmed { get; } = isPhoneNumberConfirmed;

    /// <summary>
    /// Indicates whether the user is pending deletion.
    /// </summary>
    public bool IsPendingDeletion { get; } = isPendingDeletion;

    /// <summary>
    /// Indicates whether two-factor authentication is enabled for the user.
    /// </summary>
    public bool IsTwoFactorEnabled { get; } = isTwoFactorEnabled;

    /// <summary>
    /// User registration date.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTimeOffset RegistrationDate { get; } = registrationDate;
    #endregion
}