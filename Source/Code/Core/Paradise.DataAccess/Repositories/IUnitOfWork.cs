using Microsoft.EntityFrameworkCore;

namespace Paradise.DataAccess.Repositories;

/// <summary>
/// Defines the contract for coordinating the persistence of changes
/// made through repositories that share the same underlying <see cref="DbContext"/>.
/// </summary>
public interface IUnitOfWork
{
    #region Methods
    /// <summary>
    /// Persists all pending changes tracked by the underlying
    /// <see cref="DbContext"/> to the database as a single atomic operation.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// The task result contains the number of state entries written to the persistence storage.
    /// </returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    #endregion
}