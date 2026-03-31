using Paradise.DataAccess.Repositories.Domain.Identity.Users;

namespace Paradise.DataAccess.Repositories;

/// <inheritdoc/>
public interface IDomainUnitOfWork : IUnitOfWork
{
    #region Properties
    /// <summary>
    /// User refresh tokens repository.
    /// </summary>
    IUserRefreshTokensRepository UserRefreshTokensRepository { get; }
    #endregion
}