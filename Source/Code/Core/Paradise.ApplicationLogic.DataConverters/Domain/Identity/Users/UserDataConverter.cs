using Paradise.DataAccess.Seed.Models.Domain.Identity.Users;
using Paradise.Domain.Identity.Users;
using Paradise.Models.Domain.Identity.Users;

namespace Paradise.ApplicationLogic.DataConverters.Domain.Identity.Users;

/// <summary>
/// Contains extension methods for <see cref="User"/> conversion operations.
/// </summary>
public static class UserDataConverter
{
    #region Public methods
    /// <summary>
    /// Converts the <see cref="UserRegistrationModel"/> into the <see cref="User"/>.
    /// </summary>
    /// <param name="model">
    /// The input <see cref="UserRegistrationModel"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="User"/> instance
    /// converted from the input <paramref name="model"/>.
    /// </returns>
    public static User ToEntity(this UserRegistrationModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var emailAddress = model.EmailAddress;
        var userName = model.UserName;
        var phoneNumber = model.PhoneNumber;

        return new(emailAddress, userName)
        {
            PhoneNumber = phoneNumber
        };
    }

    /// <summary>
    /// Converts the <see cref="SeedUserModel"/> into the <see cref="User"/>.
    /// </summary>
    /// <param name="model">
    /// The input <see cref="SeedUserModel"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="User"/> instance
    /// converted from the input <paramref name="model"/>.
    /// </returns>
    public static User ToEntity(this SeedUserModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var emailAddress = model.EmailAddress;
        var userName = model.UserName;
        var isEmailConfirmed = model.IsEmailConfirmed;

        return new(emailAddress, userName)
        {
            EmailConfirmed = isEmailConfirmed
        };
    }

    /// <summary>
    /// Converts the <see cref="User"/> into the <see cref="UserModel"/>.
    /// </summary>
    /// <param name="user">
    /// The input <see cref="User"/> to be converted.
    /// </param>
    /// <returns>
    /// A new <see cref="UserModel"/> instance
    /// converted from the input <paramref name="user"/>.
    /// </returns>
    public static UserModel ToModel(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var id = user.Id;
        var userName = user.UserName;
        var email = user.Email;
        var emailConfirmed = user.EmailConfirmed;
        var phoneNumber = user.PhoneNumber;
        var phoneNumberConfirmed = user.PhoneNumberConfirmed;
        var isPendingDeletion = user.IsPendingDeletion;
        var twoFactorEnabled = user.TwoFactorEnabled;
        var created = user.Created;

        return new(id, userName, email, emailConfirmed, phoneNumber, phoneNumberConfirmed, isPendingDeletion, twoFactorEnabled, created);
    }
    #endregion
}