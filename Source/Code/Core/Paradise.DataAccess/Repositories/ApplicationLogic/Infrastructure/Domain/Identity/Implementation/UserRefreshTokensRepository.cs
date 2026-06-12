using Microsoft.EntityFrameworkCore;
using Paradise.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Repositories.Base.Implementation;

namespace Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity.Implementation;

/// <summary>
/// <see cref="UserRefreshToken"/> repository class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRefreshTokensRepository"/> class.
/// </remarks>
/// <param name="source">
/// Repository data source.
/// </param>
public sealed class UserRefreshTokensRepository(IDataSource source)
    : Repository<UserRefreshToken>(source), IUserRefreshTokensRepository
{
    #region Public methods
    /// <inheritdoc/>
    public Task<List<UserRefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
        => GetQueryableEntities().Where(token => token.OwnerId == userId).ToListAsync(cancellationToken);
    #endregion
}