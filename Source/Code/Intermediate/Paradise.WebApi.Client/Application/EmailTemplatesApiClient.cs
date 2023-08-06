using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Paradise.Models;
using Paradise.Models.Application.EmailTemplateModels;
using Paradise.Options.Models;
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
/// <param name="applicationOptions">
/// The accessor used to access the <see cref="ApplicationOptions"/>.
/// </param>
/// <param name="jsonSerializerOptions">
/// The accessor used to access the <see cref="JsonSerializerOptions"/>.
/// </param>
/// <param name="httpClient">
/// <see cref="HttpClient"/> instance the <see cref="EmailTemplatesApiClient"/>
/// will operate over.
/// </param>
/// <param name="schemeName">
/// The authentication scheme name for this client.
/// <para>
/// The default is <see cref="JwtBearerDefaults.AuthenticationScheme"/>.
/// </para>
/// </param>
public sealed class EmailTemplatesApiClient(IOptionsMonitor<ApplicationOptions> applicationOptions,
                                            IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions,
                                            HttpClient httpClient,
                                            string schemeName = JwtBearerDefaults.AuthenticationScheme)
    : ApiClientBase(applicationOptions, jsonSerializerOptions, httpClient, schemeName)
{
    #region Public methods
    /// <summary>
    /// Gets the list of email templates.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="EmailTemplateModel"/>
    /// containing information about the email templates.
    /// </returns>
    public Task<Result<IEnumerable<EmailTemplateModel>>> GetAllAsync(string accessToken)
    {
        var uri = CreateUri(GetAll);

        return GetAsync<IEnumerable<EmailTemplateModel>>(uri, accessToken);
    }

    /// <summary>
    /// Gets the email template with the given <paramref name="emailTemplateId"/>.
    /// </summary>
    /// <param name="emailTemplateId">
    /// The Id of the email template to be found.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailTemplateModel"/>
    /// containing information about the email template found.
    /// </returns>
    public Task<Result<EmailTemplateModel>> GetByIdAsync(Guid emailTemplateId, string accessToken)
    {
        var uri = CreateUri(GetById, routeParameters: new()
        {
            { EmailTemplateIdParameter, emailTemplateId }
        });

        return GetAsync<EmailTemplateModel>(uri, accessToken);
    }

    /// <summary>
    /// Creates a new email template.
    /// </summary>
    /// <param name="model">
    /// The <see cref="EmailTemplateCreationModel"/> to be used
    /// to create a new email template.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailTemplateModel"/>
    /// containing information about the created email template.
    /// </returns>
    public Task<Result<EmailTemplateModel>> CreateAsync(EmailTemplateCreationModel model, string accessToken)
    {
        var uri = CreateUri(Create);

        return PostAsync<EmailTemplateModel>(uri, model, accessToken);
    }

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
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="EmailTemplateModel"/>
    /// containing information about the created email template.
    /// </returns>
    public Task<Result<EmailTemplateModel>> UpdateAsync(Guid emailTemplateId, EmailTemplateUpdateModel model, string accessToken)
    {
        var uri = CreateUri(Update, routeParameters: new()
        {
            { EmailTemplateIdParameter, emailTemplateId }
        });

        return PatchAsync<EmailTemplateModel>(uri, model, accessToken);
    }

    /// <summary>
    /// Deletes an email template.
    /// </summary>
    /// <param name="emailTemplateId">
    /// The Id of the email template to be deleted.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> DeleteAsync(Guid emailTemplateId, string accessToken)
    {
        var uri = CreateUri(Delete, routeParameters: new()
        {
            { EmailTemplateIdParameter, emailTemplateId }
        });

        return DeleteAsync(uri, accessToken);
    }
    #endregion
}