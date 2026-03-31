using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories.Base;
using System.Globalization;
using System.Linq.Expressions;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

/// <summary>
/// Fake <see cref="IEmailTemplatesRepository"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeEmailTemplatesRepository"/> class.
/// </remarks>
/// <param name="source">
/// Repository data source.
/// </param>
public sealed class FakeEmailTemplatesRepository(IDataSource source) : FakeRepositoryBase<EmailTemplate>(source), IEmailTemplatesRepository
{
    #region Public methods
    /// <inheritdoc/>
    public Task<EmailTemplate?> GetByNameAndCultureAsync(string templateName, CultureInfo? culture, CancellationToken cancellationToken = default)
    {
        Expression<Func<EmailTemplate, bool>> predicate = culture is null
            ? template => template.TemplateName == templateName && template.Culture == null
            : template => template.TemplateName == templateName && template.Culture != null && template.Culture.LCID == culture.LCID;

        return Task.FromResult(_source.GetQueryable<EmailTemplate>().SingleOrDefault(predicate));
    }
    #endregion
}