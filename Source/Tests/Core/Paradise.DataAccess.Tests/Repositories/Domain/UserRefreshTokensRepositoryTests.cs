using Paradise.DataAccess.Repositories.Domain.Implementation;
using Paradise.DataAccess.Tests.Repositories.Base;
using Paradise.Domain.Users;

namespace Paradise.DataAccess.Tests.Repositories.Domain;

/// <summary>
/// Test class for the <see cref="UserRefreshTokensRepository"/>.
/// </summary>
public sealed class UserRefreshTokensRepositoryTests
    : RepositoryTests<UserRefreshTokensRepository, UserRefreshToken>
{
    #region Protected methods
    /// <inheritdoc/>
    protected override UserRefreshToken GetTestEntity(Guid? id = null, DateTime? created = null, DateTime? modified = null)
    {
        var userRefreshToken = new UserRefreshToken(Guid.Empty);

        if (id.HasValue)
            userRefreshToken.Id = id.Value;

        if (created.HasValue)
            userRefreshToken.Created = created.Value;

        if (modified.HasValue)
            userRefreshToken.Modified = modified.Value;

        return userRefreshToken;
    }
    #endregion
}