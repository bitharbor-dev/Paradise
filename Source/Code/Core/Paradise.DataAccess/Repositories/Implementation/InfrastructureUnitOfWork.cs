using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Repositories.Attributes;

namespace Paradise.DataAccess.Repositories.Implementation;

/// <summary>
/// Represents a unit of work for the infrastructure data layer.
/// Coordinates persistence of changes for infrastructure-related repositories.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="InfrastructureUnitOfWork"/> class.
/// </remarks>
/// <param name="source">
/// Data source.
/// </param>
/// <param name="emailTemplatesRepository">
/// Email templates repository.
/// </param>
internal sealed class InfrastructureUnitOfWork([InfrastructureContextKey] IDataSource source,
                                               IEmailTemplatesRepository emailTemplatesRepository) : IInfrastructureUnitOfWork
{
    #region Properties
    /// <inheritdoc/>
    public IEmailTemplatesRepository EmailTemplatesRepository { get; } = emailTemplatesRepository;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => source.SaveChangesAsync(cancellationToken);
    #endregion
}