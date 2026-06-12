using Paradise.ApplicationLogic.Infrastructure.Services.MessageTemplates;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.WebApi.Infrastructure.Extensions;

namespace Paradise.WebApi.Endpoints.Handlers.Infrastructure.Domain.MessageTemplates;

/// <summary>
/// Contains email templates management actions.
/// </summary>
internal static class EmailTemplateHandlers
{
    #region Public methods
    /// <summary>
    /// Gets the list of email templates.
    /// </summary>
    /// <returns>
    /// <param name="service">
    /// Email template service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// An <see cref="IEnumerable{T}"/> of <see cref="EmailTemplateModel"/>
    /// containing information about the email templates.
    /// </returns>
    public static Task<IResult> GetAllAsync(IEmailTemplateService service, CancellationToken cancellationToken)
        => service.GetAllAsync(cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Gets the email template with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the email template to be found.
    /// </param>
    /// <param name="service">
    /// Email template service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="EmailTemplateModel"/> containing information about the email template found.
    /// </returns>
    public static Task<IResult> GetByIdAsync(Guid id, IEmailTemplateService service, CancellationToken cancellationToken)
        => service.GetByIdAsync(id, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Creates a new email template.
    /// </summary>
    /// <param name="model">
    /// The <see cref="EmailTemplateCreationModel"/> to be used to
    /// create a new email template.
    /// </param>
    /// <param name="service">
    /// Email template service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="EmailTemplateModel"/> containing information about the created email template.
    /// </returns>
    public static Task<IResult> CreateAsync(EmailTemplateCreationModel model, IEmailTemplateService service, CancellationToken cancellationToken)
        => service.CreateAsync(model, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Updates an email template.
    /// </summary>
    /// <param name="id">
    /// The Id of the email template to be updated.
    /// </param>
    /// <param name="model">
    /// The <see cref="EmailTemplateUpdateModel"/> to be used to
    /// update an email template.
    /// </param>
    /// <param name="service">
    /// Email template service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="EmailTemplateModel"/> containing information about the created email template.
    /// </returns>
    public static Task<IResult> UpdateAsync(Guid id, EmailTemplateUpdateModel model, IEmailTemplateService service, CancellationToken cancellationToken)
        => service.UpdateAsync(id, model, cancellationToken).AsHttpResultAsync();

    /// <summary>
    /// Deletes an email template.
    /// </summary>
    /// <param name="id">
    /// The Id of the email template to be deleted.
    /// </param>
    /// <param name="service">
    /// Email template service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> DeleteAsync(Guid id, IEmailTemplateService service, CancellationToken cancellationToken)
        => service.DeleteAsync(id, cancellationToken).AsHttpResultAsync();
    #endregion
}