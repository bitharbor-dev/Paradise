using Microsoft.EntityFrameworkCore;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Repositories.Attributes;
using Paradise.DataAccess.Repositories.Base.Implementation;
using System.Globalization;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates.Implementation;

/// <summary>
/// <see cref="EmailTemplate"/> repository class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailTemplatesRepository"/> class.
/// </remarks>
/// <param name="source">
/// Repository data source.
/// </param>
public sealed class EmailTemplatesRepository([InfrastructureContextKey] IDataSource source)
    : Repository<EmailTemplate>(source), IEmailTemplatesRepository
{
    #region Public methods
    /// <inheritdoc/>
    public Task<EmailTemplate?> GetByNameAndCultureAsync(string templateName, CultureInfo? culture, CancellationToken cancellationToken = default)
    {
        Expression<Func<EmailTemplate, bool>> predicate =
            template => template.TemplateName == templateName && template.Culture == culture;

        return GetQueryableEntities().SingleOrDefaultAsync(predicate, cancellationToken);
    }
    #endregion
}