using Microsoft.AspNetCore.Mvc;
using Paradise.ApplicationLogic.Infrastructure.Services.MessageTemplates;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Primitives.Web;
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
    /// The service.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// An <see cref="IEnumerable{T}"/> of <see cref="EmailTemplateModel"/>
    /// containing information about the email templates.
    /// </returns>
    public static Task<IResult> GetAllAsync([FromServices] IEmailTemplateService service,
                                            CancellationToken cancellationToken)
    {
        var result = service.GetAllAsync(cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Gets the email template with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="id">
    /// The Id of the email template to be found.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="EmailTemplateModel"/> containing information about the email template found.
    /// </returns>
    public static Task<IResult> GetByIdAsync([FromServices] IEmailTemplateService service,
                                             [FromRoute(Name = ParameterNames.IdParameter)] Guid id,
                                             CancellationToken cancellationToken)
    {
        var result = service.GetByIdAsync(id, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Creates a new email template.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="model">
    /// The <see cref="EmailTemplateCreationModel"/> to be used to
    /// create a new email template.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="EmailTemplateModel"/> containing information about the created email template.
    /// </returns>
    public static Task<IResult> CreateAsync([FromServices] IEmailTemplateService service,
                                            [FromBody] EmailTemplateCreationModel model,
                                            CancellationToken cancellationToken)
    {
        var result = service.CreateAsync(model, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Updates an email template.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="model">
    /// The <see cref="EmailTemplateUpdateModel"/> to be used to
    /// update an email template.
    /// </param>
    /// <param name="id">
    /// The Id of the email template to be updated.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="EmailTemplateModel"/> containing information about the created email template.
    /// </returns>
    public static Task<IResult> UpdateAsync([FromServices] IEmailTemplateService service,
                                            [FromBody] EmailTemplateUpdateModel model,
                                            [FromRoute(Name = ParameterNames.IdParameter)] Guid id,
                                            CancellationToken cancellationToken)
    {
        var result = service.UpdateAsync(id, model, cancellationToken);

        return result.AsHttpResultAsync();
    }

    /// <summary>
    /// Deletes an email template.
    /// </summary>
    /// <param name="service">
    /// The service.
    /// </param>
    /// <param name="id">
    /// The Id of the email template to be deleted.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance containing errors data if any occurs.
    /// </returns>
    public static Task<IResult> DeleteAsync([FromServices] IEmailTemplateService service,
                                            [FromRoute(Name = ParameterNames.IdParameter)] Guid id,
                                            CancellationToken cancellationToken)
    {
        var result = service.DeleteAsync(id, cancellationToken);

        return result.AsHttpResultAsync();
    }
    #endregion
}