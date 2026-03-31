using Paradise.DataAccess.Repositories.Attributes;
using Paradise.DataAccess.Repositories.Domain.Identity.Users;

namespace Paradise.DataAccess.Repositories.Implementation;

/// <summary>
/// Represents a unit of work for the domain data layer.
/// Coordinates persistence of changes for domain-related repositories.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DomainUnitOfWork"/> class.
/// </remarks>
/// <param name="source">
/// Data source.
/// </param>
/// <param name="userRefreshTokensRepository">
/// User refresh tokens repository.
/// </param>
internal sealed class DomainUnitOfWork([DomainContextKey] IDataSource source,
                                       IUserRefreshTokensRepository userRefreshTokensRepository) : IDomainUnitOfWork
{
    #region Properties
    /// <inheritdoc/>
    public IUserRefreshTokensRepository UserRefreshTokensRepository { get; } = userRefreshTokensRepository;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => source.SaveChangesAsync(cancellationToken);
    #endregion
}