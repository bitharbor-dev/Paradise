using Paradise.Models;
using Paradise.Models.Application.EmailTemplateModels;

namespace Paradise.ApplicationLogic.Services.Application;

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
    /// Gets the email template with the given <paramref name="emailTemplateId"/>.
    /// </summary>
    /// <param name="emailTemplateId">
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
    Task<Result<EmailTemplateModel>> GetByIdAsync(Guid emailTemplateId, CancellationToken cancellationToken = default);

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
    /// <param name="emailTemplateId">
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
    Task<Result<EmailTemplateModel>> UpdateAsync(Guid emailTemplateId, EmailTemplateUpdateModel model,
                                                 CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an email template.
    /// </summary>
    /// <param name="emailTemplateId">
    /// The Id of the email template to be deleted.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    Task<Result> DeleteAsync(Guid emailTemplateId, CancellationToken cancellationToken = default);
    #endregion
}