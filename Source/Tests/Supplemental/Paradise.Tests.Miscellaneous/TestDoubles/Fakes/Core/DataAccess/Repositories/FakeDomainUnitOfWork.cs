using Paradise.DataAccess;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.Domain.Identity.Users;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories.Domain.Identity.Users;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories;

/// <summary>
/// Fake <see cref="IDomainUnitOfWork"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeDomainUnitOfWork"/> class.
/// </remarks>
/// <param name="source">
/// Unit-of-work data source.
/// </param>
public sealed class FakeDomainUnitOfWork(IDataSource source) : IDomainUnitOfWork
{
    #region Properties
    /// <inheritdoc/>
    public IUserRefreshTokensRepository UserRefreshTokensRepository { get; } = new FakeUserRefreshTokensRepository(source);
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => source.SaveChangesAsync(cancellationToken);
    #endregion
}