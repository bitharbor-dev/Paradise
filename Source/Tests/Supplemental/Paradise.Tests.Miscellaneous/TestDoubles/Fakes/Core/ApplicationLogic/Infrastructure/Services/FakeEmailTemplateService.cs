using Paradise.ApplicationLogic.DataConverters.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Infrastructure.Services;
using Paradise.DataAccess;
using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using System.Globalization;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.Services;

/// <summary>
/// Fake <see cref="IEmailTemplateService"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeEmailTemplateService"/> class.
/// </remarks>
/// <param name="source">
/// An <see cref="IDataSource"/> instance used to
/// arrange data and validate test results.
/// </param>
public sealed class FakeEmailTemplateService(IDataSource source) : IEmailTemplateService
{
    #region Properties
    /// <summary>
    /// <see cref="CreateAsync"/> result.
    /// </summary>
    public Func<Task<Result<EmailTemplateModel>>>? CreateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="UpdateAsync"/> result.
    /// </summary>
    public Func<Task<Result<EmailTemplateModel>>>? UpdateAsyncResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<Result<IEnumerable<EmailTemplateModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var models = source
            .GetQueryable<EmailTemplate>()
            .Select(emailTemplate => emailTemplate.ToModel());

        return Task.FromResult(new Result<IEnumerable<EmailTemplateModel>>(models, Success));
    }

    /// <inheritdoc/>
    public Task<Result<EmailTemplateModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var emailTemplate = source
            .GetQueryable<EmailTemplate>()
            .FirstOrDefault(template => template.Id == id);

        var result = emailTemplate is null
            ? new Result<EmailTemplateModel>(Missing, MessageTemplateIdNotFound, id)
            : new(emailTemplate.ToModel(), Success);

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<Result<EmailTemplateModel>> GetByNameAndCultureAsync(string templateName, CultureInfo? culture,
                                                                     CancellationToken cancellationToken = default)
    {
        var emailTemplate = source
            .GetQueryable<EmailTemplate>()
            .FirstOrDefault(template => template.TemplateName == templateName && template.Culture == culture);

        var result = emailTemplate is null
            ? new Result<EmailTemplateModel>(Missing, MessageTemplateNotFound, templateName, culture)
            : new(emailTemplate.ToModel(), Success);

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public async Task<Result<EmailTemplateModel>> CreateAsync(EmailTemplateCreationModel model, CancellationToken cancellationToken = default)
    {
        if (CreateAsyncResult is not null)
        {
            return await CreateAsyncResult()
                .ConfigureAwait(false);
        }

        var emailTemplate = model.ToEntity();

        source.Add(emailTemplate);

        await source.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return new(emailTemplate.ToModel(), Created);
    }

    /// <inheritdoc/>
    public async Task<Result<EmailTemplateModel>> UpdateAsync(Guid id, EmailTemplateUpdateModel model,
                                                              CancellationToken cancellationToken = default)
    {
        if (UpdateAsyncResult is not null)
        {
            return await UpdateAsyncResult()
                .ConfigureAwait(false);
        }

        if (model is null)
            return new(InvalidInput, InvalidModel, nameof(model));

        var emailTemplate = source
            .GetQueryable<EmailTemplate>()
            .FirstOrDefault(template => template.Id == id);

        if (emailTemplate is null)
            return new(Missing, MessageTemplateIdNotFound, id);

        var changesExists = emailTemplate.Update(model.IsBodyHtml,
                                                 model.PlaceholderName,
                                                 model.PlaceholdersNumber,
                                                 model.Subject,
                                                 model.SubjectPlaceholderName,
                                                 model.SubjectPlaceholdersNumber,
                                                 model.TemplateText);

        var status = Success;

        if (changesExists)
        {
            await source.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            status = Received;
        }

        return new(emailTemplate.ToModel(), status);
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var emailTemplate = source
            .GetQueryable<EmailTemplate>()
            .FirstOrDefault(template => template.Id == id);

        if (emailTemplate is not null)
        {
            source.Remove(emailTemplate);

            await source.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        return new(Success);
    }
    #endregion
}