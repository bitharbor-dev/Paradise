using Paradise.ApplicationLogic.DataConverters.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Common.Extensions;
using Paradise.DataAccess.Repositories;
using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using System.Globalization;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.ApplicationLogic.Infrastructure.Services.Implementation;

/// <summary>
/// Provides email templates management functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailTemplateService"/> class.
/// </remarks>
/// <param name="unitOfWork">
/// Unit of work.
/// </param>
internal sealed class EmailTemplateService(IInfrastructureUnitOfWork unitOfWork) : IEmailTemplateService
{
    #region Public methods
    /// <inheritdoc/>
    public async Task<Result<IEnumerable<EmailTemplateModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var emailTemplates = await unitOfWork
            .EmailTemplatesRepository
            .GetAllAsync(cancellationToken)
            .ConfigureAwait(false);

        var models = emailTemplates.Select(emailTemplate => emailTemplate.ToModel());

        return new(models, Success);
    }

    /// <inheritdoc/>
    public async Task<Result<EmailTemplateModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var emailTemplate = await unitOfWork
            .EmailTemplatesRepository
            .GetByIdAsync(id, cancellationToken)
            .ConfigureAwait(false);

        return emailTemplate is null
            ? new(Missing, MessageTemplateIdNotFound, id)
            : new(emailTemplate.ToModel(), Success);
    }

    public async Task<Result<EmailTemplateModel>> GetByNameAndCultureAsync(string templateName, CultureInfo? culture,
                                                                           CancellationToken cancellationToken = default)
    {
        var emailTemplate = await unitOfWork
            .EmailTemplatesRepository
            .GetByNameAndCultureAsync(templateName, culture, cancellationToken)
            .ConfigureAwait(false);

        return emailTemplate is null
            ? new(Missing, MessageTemplateNotFound, templateName, culture)
            : new(emailTemplate.ToModel(), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<EmailTemplateModel>> CreateAsync(EmailTemplateCreationModel model, CancellationToken cancellationToken = default)
    {
        if (model is null)
            return new(InvalidInput, InvalidModel, nameof(model));

        if (model.Subject.IsNullOrWhiteSpace())
            return new(InvalidInput, MessageTemplateEmptyText, nameof(model.Subject));

        if (model.TemplateName.IsNullOrWhiteSpace())
            return new(InvalidInput, MessageTemplateMissingName);

        if (model.TemplateText.IsNullOrWhiteSpace())
            return new(InvalidInput, MessageTemplateEmptyText, nameof(model.TemplateText));

        var emailTemplate = model.ToEntity();

        var existingTemplate = await unitOfWork
            .EmailTemplatesRepository
            .GetByNameAndCultureAsync(emailTemplate.TemplateName, emailTemplate.Culture, cancellationToken)
            .ConfigureAwait(false);

        if (existingTemplate is not null)
            return new(InvalidInput, MessageTemplateAlreadyExists, emailTemplate.TemplateName, emailTemplate.Culture?.LCID);

        unitOfWork.EmailTemplatesRepository.Add(emailTemplate);

        await unitOfWork.CommitAsync(cancellationToken)
            .ConfigureAwait(false);

        return new(emailTemplate.ToModel(), Created);
    }

    /// <inheritdoc/>
    public async Task<Result<EmailTemplateModel>> UpdateAsync(Guid id, EmailTemplateUpdateModel model, CancellationToken cancellationToken = default)
    {
        if (model is null)
            return new(InvalidInput, InvalidModel, nameof(model));

        var emailTemplate = await unitOfWork
            .EmailTemplatesRepository
            .GetByIdAsync(id, cancellationToken)
            .ConfigureAwait(false);

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
            await unitOfWork.CommitAsync(cancellationToken)
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
        unitOfWork.EmailTemplatesRepository.RemoveById(id);

        await unitOfWork.CommitAsync(cancellationToken)
            .ConfigureAwait(false);

        return new(Success);
    }
    #endregion
}