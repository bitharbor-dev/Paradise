using Microsoft.EntityFrameworkCore;
using Paradise.DataAccess.Repositories.Attributes;
using Paradise.DataAccess.Repositories.Base.Implementation;
using Paradise.Domain.Identity.Users;

namespace Paradise.DataAccess.Repositories.Domain.Identity.Users.Implementation;

/// <summary>
/// <see cref="UserRefreshToken"/> repository class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRefreshTokensRepository"/> class.
/// </remarks>
/// <param name="source">
/// Repository data source.
/// </param>
public sealed class UserRefreshTokensRepository([DomainContextKey] IDataSource source)
    : Repository<UserRefreshToken>(source), IUserRefreshTokensRepository
{
    #region Public methods
    /// <inheritdoc/>
    public Task<List<UserRefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
        => GetQueryableEntities().Where(token => token.OwnerId == userId).ToListAsync(cancellationToken);
    #endregion
}