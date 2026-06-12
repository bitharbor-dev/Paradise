using Microsoft.AspNetCore.Identity;
using Paradise.Tests.Fixtures.Microsoft.AspNetCore.Identity;

namespace Paradise.Tests.Doubles.Fakes.Microsoft.AspNetCore.Identity;

/// <summary>
/// Fake <see cref="IUserTwoFactorTokenProvider{TUser}"/> implementation.
/// </summary>
public sealed class FakeUserTwoFactorTokenProvider : IUserTwoFactorTokenProvider<TestUser>
{
    #region Constants
    /// <summary>
    /// Token provided by default.
    /// </summary>
    public const string DefaultToken = "DefaultToken";
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<string> GenerateAsync(string purpose, UserManager<TestUser> manager, TestUser user)
        => Task.FromResult(DefaultToken);

    /// <inheritdoc/>
    public Task<bool> ValidateAsync(string purpose, string token, UserManager<TestUser> manager, TestUser user)
        => Task.FromResult(token is DefaultToken);

    /// <inheritdoc/>
    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TestUser> manager, TestUser user)
        => Task.FromResult(true);
    #endregion
}