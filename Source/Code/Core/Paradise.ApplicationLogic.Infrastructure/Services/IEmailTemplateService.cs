using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using System.Globalization;

namespace Paradise.ApplicationLogic.Infrastructure.Services;

/// <summary>
/// Provides email templates management functionalities.
/// </summary>
public interface IEmailTemplateService
{
    #region Methods
    /// <summary>
    /// Gets the list of email templates.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="EmailTemplateModel"/>
    /// containing information about the email templates.
    /// </returns>
    Task<Result<IEnumerable<EmailTemplateModel>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the email template with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the email template to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailTemplateModel"/>
    /// containing information about the email template found.
    /// </returns>
    Task<Result<EmailTemplateModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the email template with the given <paramref name="templateName"/> and <paramref name="culture"/>.
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
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailTemplateModel"/>
    /// containing information about the email template found.
    /// </returns>
    Task<Result<EmailTemplateModel>> GetByNameAndCultureAsync(string templateName, CultureInfo? culture,
                                                              CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new email template.
    /// </summary>
    /// <param name="model">
    /// The <see cref="EmailTemplateCreationModel"/> to be used
    /// to create a new email template.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailTemplateModel"/>
    /// containing information about the created email template.
    /// </returns>
    Task<Result<EmailTemplateModel>> CreateAsync(EmailTemplateCreationModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an email template.
    /// </summary>
    /// <param name="id">
    /// The Id of the email template to be updated.
    /// </param>
    /// <param name="model">
    /// The <see cref="EmailTemplateUpdateModel"/> to be used
    /// to update an email template.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailTemplateModel"/>
    /// containing information about the created email template.
    /// </returns>
    Task<Result<EmailTemplateModel>> UpdateAsync(Guid id, EmailTemplateUpdateModel model,
                                                 CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an email template.
    /// </summary>
    /// <param name="id">
    /// The Id of the email template to be deleted.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    #endregion
}