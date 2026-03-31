using Microsoft.Extensions.Options;
using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.WebApi.Client.Base;
using System.Text.Json;
using static Paradise.Common.Web.EmailTemplateRoutes;
using static Paradise.Common.Web.ParameterNames;

namespace Paradise.WebApi.Client.Application;

/// <summary>
/// Contains all web API requests to the EmailTemplatesController.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailTemplatesApiClient"/> class.
/// </remarks>
/// <param name="jsonSerializerOptions">
/// The accessor used to access the <see cref="JsonSerializerOptions"/>.
/// </param>
/// <param name="httpClient">
/// <see cref="HttpClient"/> instance the <see cref="EmailTemplatesApiClient"/>
/// will operate over.
/// </param>
public sealed class EmailTemplatesApiClient(IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions, HttpClient httpClient)
    : ApiClientBase(jsonSerializerOptions, httpClient)
{
    #region Public methods
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
    public Task<Result<IEnumerable<EmailTemplateModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(GetAll);

        return GetAsync<IEnumerable<EmailTemplateModel>>(route, true, cancellationToken);
    }

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
    public Task<Result<EmailTemplateModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(GetById, routeParameters: new()
        {
            [IdParameter] = id.ToString()
        });

        return GetAsync<EmailTemplateModel>(route, true, cancellationToken);
    }

    /// <summary>
    /// Creates a new email template.
    /// </summary>
    /// <param name="model">
    /// The <see cref="EmailTemplateCreationModel"/> to be used to
    /// create a new email template.
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
    public Task<Result<EmailTemplateModel>> CreateAsync(EmailTemplateCreationModel model, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(Create);

        return PostAsync<EmailTemplateModel>(route, true, model, cancellationToken);
    }

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
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailTemplateModel"/>
    /// containing information about the created email template.
    /// </returns>
    public Task<Result<EmailTemplateModel>> UpdateAsync(
        Guid id, EmailTemplateUpdateModel model, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(Update, routeParameters: new()
        {
            [IdParameter] = id.ToString()
        });

        return PatchAsync<EmailTemplateModel>(route, true, model, cancellationToken);
    }

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
    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(Delete, routeParameters: new()
        {
            [IdParameter] = id.ToString()
        });

        return DeleteAsync(route, true, cancellationToken);
    }
    #endregion
}