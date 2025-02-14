using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paradise.ApplicationLogic.Services.Application;
using Paradise.Common.Web;
using Paradise.Models;
using Paradise.Models.Application.EmailTemplateModels;
using Paradise.WebApi.Controllers.Base;
using Paradise.WebApi.Filters.Annotation;
using System.ComponentModel.DataAnnotations;
using System.Net;
using static Paradise.Common.Web.ParameterNames;

namespace Paradise.WebApi.Controllers.Application;

/// <summary>
/// Contains roles management actions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailTemplatesController"/> class.
/// </remarks>
/// <param name="emailTemplateService">
/// Email template service.
/// </param>
[Authorize(Roles = "Administrator")]
public sealed class EmailTemplatesController(IEmailTemplateService emailTemplateService) : ApiControllerBase
{
    #region Public methods
    /// <summary>
    /// Gets the list of email templates.
    /// </summary>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="EmailTemplateModel"/>
    /// containing information about the email templates.
    /// </returns>
    [HttpGet(EmailTemplateRoutes.GetAll)]
    [ResultResponse<IEnumerable<EmailTemplateModel>>(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.Forbidden)]
    public async Task<IActionResult> GetAll()
        => await emailTemplateService.GetAllAsync().ConfigureAwait(false);

    /// <summary>
    /// Gets the email template with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The Id of the email template to be found.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailTemplateModel"/>
    /// containing information about the email template found.
    /// </returns>
    [HttpGet(EmailTemplateRoutes.GetById)]
    [ResultResponse<EmailTemplateModel>(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.Forbidden)]
    [ResultResponse(HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetById([FromRoute(Name = IdParameter)] Guid id)
        => await emailTemplateService.GetByIdAsync(id).ConfigureAwait(false);

    /// <summary>
    /// Creates a new email template.
    /// </summary>
    /// <param name="model">
    /// The <see cref="EmailTemplateCreationModel"/> to be used to
    /// create a new email template.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailTemplateModel"/>
    /// containing information about the created email template.
    /// </returns>
    [HttpPost(EmailTemplateRoutes.Create)]
    [ResultResponse<EmailTemplateModel>(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.BadRequest)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.Forbidden)]
    [ResultResponse(HttpStatusCode.UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody, Required] EmailTemplateCreationModel model)
        => await emailTemplateService.CreateAsync(model).ConfigureAwait(false);

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
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailTemplateModel"/>
    /// containing information about the created email template.
    /// </returns>
    [HttpPatch(EmailTemplateRoutes.Update)]
    [ResultResponse<EmailTemplateModel>(HttpStatusCode.OK)]
    [ResultResponse<EmailTemplateModel>(HttpStatusCode.Accepted)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.Forbidden)]
    [ResultResponse(HttpStatusCode.NotFound)]
    public async Task<IActionResult> Update([FromRoute(Name = IdParameter)] Guid id, [FromBody, Required] EmailTemplateUpdateModel model)
        => await emailTemplateService.UpdateAsync(id, model).ConfigureAwait(false);

    /// <summary>
    /// Deletes an email template.
    /// </summary>
    /// <param name="id">
    /// The Id of the email template to be deleted.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    [HttpDelete(EmailTemplateRoutes.Delete)]
    [ResultResponse(HttpStatusCode.OK)]
    [ResultResponse(HttpStatusCode.Unauthorized)]
    [ResultResponse(HttpStatusCode.Forbidden)]
    public async Task<IActionResult> Delete([FromRoute(Name = IdParameter)] Guid id)
        => await emailTemplateService.DeleteAsync(id).ConfigureAwait(false);
    #endregion
}