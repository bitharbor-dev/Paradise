using Paradise.DataAccess;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Tests.Doubles.Fakes.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.Tests.Doubles.Fakes.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

namespace Paradise.Tests.Doubles.Fakes.DataAccess.Repositories;

/// <summary>
/// Fake <see cref="IUnitOfWork"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeUnitOfWork"/> class.
/// </remarks>
/// <param name="source">
/// Unit-of-work data source.
/// </param>
public sealed class FakeUnitOfWork(IDataSource source) : IUnitOfWork
{
    #region Properties
    /// <inheritdoc/>
    public IUserRefreshTokensRepository UserRefreshTokensRepository { get; } = new FakeUserRefreshTokensRepository(source);

    /// <inheritdoc/>
    public IEmailTemplatesRepository EmailTemplatesRepository { get; } = new FakeEmailTemplatesRepository(source);
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => source.SaveChangesAsync(cancellationToken);
    #endregion
}