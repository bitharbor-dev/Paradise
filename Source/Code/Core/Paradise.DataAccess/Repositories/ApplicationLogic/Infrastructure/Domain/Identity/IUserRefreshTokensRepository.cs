using Paradise.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Repositories.Base;

namespace Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity;

/// <summary>
/// <see cref="UserRefreshToken"/> repository interface.
/// </summary>
public interface IUserRefreshTokensRepository : IRepository<UserRefreshToken>
{
    #region Methods
    /// <summary>
    /// Gets the list of <see cref="UserRefreshToken"/> which belong
    /// to the <see cref="User"/> with the given <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">
    /// The Id of the <see cref="User"/> whose tokens to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The list of <see cref="UserRefreshToken"/> which belong
    /// to the <see cref="User"/> with the given <paramref name="userId"/>.
    /// </returns>
    Task<List<UserRefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    #endregion
}