using Paradise.DataAccess.Seed.Models.Domain.Users;
using Paradise.Domain.Users;
using Paradise.Models.Domain.UserModels;

namespace Paradise.ApplicationLogic.DataConverters.Domain;

/// <summary>
/// Contains extension methods for "User" objects conversion operations.
/// </summary>
internal static class UserDataConverter
{
    #region Public methods
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
        => new(user.Email, user.UserName)
        {
            Id = user.Id,
            IsEmailConfirmed = user.EmailConfirmed,
            IsPendingDeletion = user.IsPendingDeletion,
            IsTwoFactorEnabled = user.TwoFactorEnabled,
            Phone = user.PhoneNumber,
            RegistrationDate = user.Created
        };

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
        => new(model.Email, model.UserName)
        {
            PhoneNumber = model.Phone
        };

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
    public static User ToEntity(this SeedUserModel model) =>
        new(model.Email, model.UserName)
        {
            EmailConfirmed = model.IsEmailConfirmed,
        };
    #endregion
}