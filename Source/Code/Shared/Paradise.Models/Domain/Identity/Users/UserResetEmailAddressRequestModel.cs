using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.Identity.Users;

/// <summary>
/// Reset email address request schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserResetEmailAddressRequestModel"/> class.
/// </remarks>
/// <param name="emailAddress">
/// User's new email address.
/// </param>
/// <param name="emailAddressConfirmation">
/// User's new email address confirmation.
/// </param>
[method: JsonConstructor]
public sealed class UserResetEmailAddressRequestModel(string emailAddress, string emailAddressConfirmation)
{
    #region Properties
    /// <summary>
    /// User's new email address.
    /// </summary>
    [EmailAddress, DataType(DataType.EmailAddress)]
    public string EmailAddress { get; } = emailAddress;

    /// <summary>
    /// User's new email address confirmation.
    /// </summary>
    [Compare(nameof(EmailAddress))]
    [EmailAddress, DataType(DataType.EmailAddress)]
    public string EmailAddressConfirmation { get; } = emailAddressConfirmation;
    #endregion
}