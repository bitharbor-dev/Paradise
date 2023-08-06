using Paradise.ApplicationLogic.Domain.MessageTemplates;
using Paradise.DataAccess.Repositories.Base;
using System.Globalization;

namespace Paradise.DataAccess.Repositories.Application;

/// <summary>
/// <see cref="EmailTemplate"/> repository interface.
/// </summary>
public interface IEmailTemplatesRepository : IRepository<EmailTemplate>
{
    #region Methods
    /// <summary>
    /// Finds an <see cref="EmailTemplate"/> with the given <paramref name="templateName"/>
    /// and <paramref name="culture"/>.
    /// If no <see cref="EmailTemplate"/> is found, then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="templateName">
    /// Template name.
    /// </param>
    /// <param name="culture">
    /// Template culture.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// The <see cref="EmailTemplate"/> found, or <see langword="null"/>.
    /// </returns>
    Task<EmailTemplate?> GetByNameAndCultureAsync(string templateName, CultureInfo? culture, CancellationToken cancellationToken = default);
    #endregion
}