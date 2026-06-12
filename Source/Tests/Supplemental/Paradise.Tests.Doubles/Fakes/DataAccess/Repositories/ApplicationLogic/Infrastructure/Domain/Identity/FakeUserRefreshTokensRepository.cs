using Paradise.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.Tests.Doubles.Fakes.DataAccess.Repositories.Base;

namespace Paradise.Tests.Doubles.Fakes.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity;

/// <summary>
/// Fake <see cref="IUserRefreshTokensRepository"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeUserRefreshTokensRepository"/> class.
/// </remarks>
/// <param name="source">
/// Repository data source.
/// </param>
public sealed class FakeUserRefreshTokensRepository(IDataSource source)
    : FakeRepositoryBase<UserRefreshToken>(source), IUserRefreshTokensRepository
{
    #region Public methods
    /// <inheritdoc/>
    public Task<List<UserRefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult(_source.GetQueryable<UserRefreshToken>().Where(token => token.OwnerId == userId).ToList());
    #endregion
}