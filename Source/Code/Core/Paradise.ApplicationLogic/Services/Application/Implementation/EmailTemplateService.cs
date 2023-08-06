using Paradise.ApplicationLogic.DataConverters.Application;
using Paradise.ApplicationLogic.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Extensions;
using Paradise.DataAccess.Repositories.Application;
using Paradise.Models;
using Paradise.Models.Application.EmailTemplateModels;
using static Paradise.ApplicationLogic.Exceptions.ResultException;
using static Paradise.Models.ErrorCode;
using static System.Net.HttpStatusCode;

namespace Paradise.ApplicationLogic.Services.Application.Implementation;

/// <summary>
/// Provides email templates management functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailTemplateService"/> class.
/// </remarks>
/// <param name="emailTemplatesRepository">
/// Email templates repository.
/// </param>
public sealed class EmailTemplateService(IEmailTemplatesRepository emailTemplatesRepository) : IEmailTemplateService
{
    #region Fields
    private readonly IEmailTemplatesRepository _emailTemplatesRepository = emailTemplatesRepository;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task<Result<IEnumerable<EmailTemplateModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var emailTemplates = await _emailTemplatesRepository.GetAllAsync(cancellationToken);

        var models = emailTemplates.Select(emailTemplate => emailTemplate.ToModel());

        return new(models, OK);
    }

    /// <inheritdoc/>
    public async Task<Result<EmailTemplateModel>> GetByIdAsync(Guid emailTemplateId, CancellationToken cancellationToken = default)
    {
        var emailTemplate = await _emailTemplatesRepository.GetByIdAsync(emailTemplateId, cancellationToken);

        emailTemplate.ThrowIfNull(NotFound, MessageTemplateIdNotFound, emailTemplateId);

        return new(emailTemplate.ToModel(), OK);
    }

    /// <inheritdoc/>
    public async Task<Result<EmailTemplateModel>> CreateAsync(EmailTemplateCreationModel model, CancellationToken cancellationToken = default)
    {
        model.Subject.ThrowIfEmptyOrWhiteSpace(BadRequest, MessageTemplateEmptyText);
        model.TemplateName.ThrowIfEmptyOrWhiteSpace(BadRequest, MessageTemplateMissingName);
        model.TemplateText.ThrowIfEmptyOrWhiteSpace(BadRequest, MessageTemplateEmptyText);

        var emailTemplate = model.ToEntity();

        ValidateEmailTemplateFormat(emailTemplate);

        var existingTemplate = await _emailTemplatesRepository.GetByNameAndCultureAsync(emailTemplate.TemplateName, emailTemplate.Culture, cancellationToken);

        existingTemplate.ThrowIfNotNull(UnprocessableEntity, MessageTemplateAlreadyExists, emailTemplate.TemplateName, emailTemplate.Culture?.LCID);

        _emailTemplatesRepository.Add(emailTemplate);
        await _emailTemplatesRepository.CommitAsync(cancellationToken);

        return new(emailTemplate.ToModel(), Created);
    }

    /// <inheritdoc/>
    public async Task<Result<EmailTemplateModel>> UpdateAsync(Guid emailTemplateId, EmailTemplateUpdateModel model, CancellationToken cancellationToken = default)
    {
        var emailTemplate = await _emailTemplatesRepository.GetByIdAsync(emailTemplateId, cancellationToken);

        emailTemplate.ThrowIfNull(NotFound, MessageTemplateIdNotFound, emailTemplateId);

        var changesExists = emailTemplate.Update(model.IsBodyHtml,
                                                 model.PlaceholderName,
                                                 model.PlaceholdersNumber,
                                                 model.Subject,
                                                 model.SubjectPlaceholderName,
                                                 model.SubjectPlaceholdersNumber,
                                                 model.TemplateText);

        var statusCode = OK;

        if (changesExists)
        {
            ValidateEmailTemplateFormat(emailTemplate);

            await _emailTemplatesRepository.CommitAsync(cancellationToken);
        }
        else
        {
            statusCode = Accepted;
        }

        return new(emailTemplate.ToModel(), statusCode);
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid emailTemplateId, CancellationToken cancellationToken = default)
    {
        _emailTemplatesRepository.RemoveById(emailTemplateId);
        await _emailTemplatesRepository.CommitAsync(cancellationToken);

        return OK;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Validates the <see cref="EmailTemplate"/> body and subject format.
    /// </summary>
    /// <param name="emailTemplate">
    /// The <see cref="EmailTemplate"/> which body and subject to be validated.
    /// </param>
    private static void ValidateEmailTemplateFormat(EmailTemplate emailTemplate)
    {
        var subjectParameters = Enumerable
            .Range(0, emailTemplate.SubjectPlaceholdersNumber)
            .Select(i => (object?)i)
            .ToList();

        var bodyParameters = Enumerable
            .Range(0, emailTemplate.PlaceholdersNumber)
            .Select(i => (object?)i)
            .ToList();

        try
        {
            emailTemplate.GetFormattedSubject(subjectParameters);
            emailTemplate.GetFormattedText(bodyParameters);
        }
        catch (FormatException e)
        {
            Throw(BadRequest, MessageTemplateMissingPlaceholder, e.Message);
        }
    }
    #endregion
}