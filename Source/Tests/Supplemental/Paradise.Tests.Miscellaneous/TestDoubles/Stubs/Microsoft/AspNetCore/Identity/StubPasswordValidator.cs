using Microsoft.AspNetCore.Identity;
using Paradise.Tests.Miscellaneous.TestImplementations.Microsoft.AspNetCore.Identity;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Microsoft.AspNetCore.Identity;

/// <summary>
/// Fake <see cref="IPasswordValidator{TUser}"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StubPasswordValidator"/> class.
/// </remarks>
/// <param name="result">
/// Predefined validation result.
/// </param>
public sealed class StubPasswordValidator(IdentityResult? result = null) : IPasswordValidator<TestUser>
{
    #region Fields
    private readonly IdentityResult _result = result ?? IdentityResult.Success;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<IdentityResult> ValidateAsync(UserManager<TestUser> manager, TestUser user, string? password)
        => Task.FromResult(_result);
    #endregion
}