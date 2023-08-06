using Microsoft.AspNetCore.Identity;
using Paradise.Domain.Users;

namespace Paradise.Tests.Miscellaneous.Fakes.Microsoft.AspNetCore.Identity;

/// <summary>
/// Fake <see cref="IUserTwoFactorTokenProvider{TUser}"/> of <see cref="User"/> implementation.
/// </summary>
public sealed class FakeUserTwoFactorTokenProvider : IUserTwoFactorTokenProvider<User>
{
    #region Constants
    /// <summary>
    /// Token provided by default.
    /// </summary>
    public const string DefaultToken = "DefaultToken";
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<string> GenerateAsync(string purpose, UserManager<User> manager, User user)
        => Task.FromResult(DefaultToken);

    /// <inheritdoc/>
    public Task<bool> ValidateAsync(string purpose, string token, UserManager<User> manager, User user)
        => Task.FromResult(token is DefaultToken);

    /// <inheritdoc/>
    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> manager, User user)
        => Task.FromResult(true);
    #endregion
}