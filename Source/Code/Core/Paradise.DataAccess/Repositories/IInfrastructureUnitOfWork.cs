using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

namespace Paradise.DataAccess.Repositories;

/// <inheritdoc/>
public interface IInfrastructureUnitOfWork : IUnitOfWork
{
    #region Properties
    /// <summary>
    /// Email templates repository.
    /// </summary>
    IEmailTemplatesRepository EmailTemplatesRepository { get; }
    #endregion
}