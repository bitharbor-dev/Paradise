using Paradise.DataAccess;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories;

/// <summary>
/// Fake <see cref="IInfrastructureUnitOfWork"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeInfrastructureUnitOfWork"/> class.
/// </remarks>
/// <param name="source">
/// Unit-of-work data source.
/// </param>
public sealed class FakeInfrastructureUnitOfWork(IDataSource source) : IInfrastructureUnitOfWork
{
    #region Properties
    /// <inheritdoc/>
    public IEmailTemplatesRepository EmailTemplatesRepository { get; } = new FakeEmailTemplatesRepository(source);
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => source.SaveChangesAsync(cancellationToken);
    #endregion
}