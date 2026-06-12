using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

namespace Paradise.DataAccess.Repositories.Implementation;

/// <summary>
/// Represents a unit of work for the data layer.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UnitOfWork"/> class.
/// </remarks>
/// <param name="source">
/// Data source.
/// </param>
/// <param name="userRefreshTokensRepository">
/// User refresh tokens repository.
/// </param>
/// <param name="emailTemplatesRepository">
/// Email templates repository.
/// </param>
internal sealed class UnitOfWork(IDataSource source,
                                 IUserRefreshTokensRepository userRefreshTokensRepository,
                                 IEmailTemplatesRepository emailTemplatesRepository) : IUnitOfWork
{
    #region Properties
    /// <inheritdoc/>
    public IUserRefreshTokensRepository UserRefreshTokensRepository { get; } = userRefreshTokensRepository;

    /// <inheritdoc/>
    public IEmailTemplatesRepository EmailTemplatesRepository { get; } = emailTemplatesRepository;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => source.SaveChangesAsync(cancellationToken);
    #endregion
}