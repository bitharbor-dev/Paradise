using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Paradise.Models;
using Paradise.Models.Domain.RoleModels;
using Paradise.Options.Models;
using Paradise.WebApi.Client.Application;
using Paradise.WebApi.Client.Base;
using System.Text.Json;
using static Paradise.Common.Web.ParameterNames;
using static Paradise.Common.Web.RoleRoutes;

namespace Paradise.WebApi.Client.Domain;

/// <summary>
/// Contains all web API requests to the RolesController.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RolesApiClient"/> class.
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
public sealed class RolesApiClient(IOptionsMonitor<ApplicationOptions> applicationOptions,
                                   IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions,
                                   HttpClient httpClient,
                                   string schemeName = JwtBearerDefaults.AuthenticationScheme)
    : ApiClientBase(applicationOptions, jsonSerializerOptions, httpClient, schemeName)
{
    #region Public methods
    /// <summary>
    /// Gets the list of application roles.
    /// </summary>
    /// <param name="isDefault">
    /// Indicates whether the default, not default or all
    /// roles should be retrieved.
    /// <list type="bullet">
    /// <item>
    /// <see langword="null"/> - all roles.
    /// </item>
    /// <item>
    /// <see langword="true"/> - default roles.
    /// </item>
    /// <item>
    /// <see langword="false"/> - not default roles.
    /// </item>
    /// </list>
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles.
    /// </returns>
    public Task<Result<IEnumerable<RoleModel>>> GetAllAsync(bool? isDefault, string accessToken)
    {
        var queryParameters = isDefault.HasValue
            ? new Dictionary<string, object?>
            {
                { IsDefaultParameter, isDefault.Value }
            }
            : null;

        var uri = CreateUri(GetAll, queryParameters: queryParameters);

        return GetAsync<IEnumerable<RoleModel>>(uri, accessToken);
    }

    /// <summary>
    /// Gets the role with the given <paramref name="roleId"/>.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be found.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="RoleModel"/>
    /// containing information about the role found.
    /// </returns>
    public Task<Result<RoleModel>> GetByIdAsync(Guid roleId, string accessToken)
    {
        var uri = CreateUri(GetById, routeParameters: new()
        {
            { RoleIdParameter, roleId }
        });

        return GetAsync<RoleModel>(uri, accessToken);
    }

    /// <summary>
    /// Gets the list of application roles, which belongs
    /// to the user with the given <paramref name="userId"/>,
    /// or the current user if no value was provided.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user whose roles to be found.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>,
    /// or the current user if no value was provided.
    /// </returns>
    public Task<Result<IEnumerable<RoleModel>>> GetUserRolesAsync(Guid? userId, string accessToken)
    {
        var uri = CreateUri(GetUserRoles, routeParameters: new()
        {
            { UserIdParameter, userId }
        });

        return GetAsync<IEnumerable<RoleModel>>(uri, accessToken);
    }

    /// <summary>
    /// Creates a new application role.
    /// </summary>
    /// <param name="model">
    /// The <see cref="RoleCreationModel"/> to be used to
    /// create a new application role.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="RoleModel"/>
    /// containing information about the created role.
    /// </returns>
    public Task<Result<RoleModel>> CreateAsync(RoleCreationModel model, string accessToken)
    {
        var uri = CreateUri(Create);

        return PostAsync<RoleModel>(uri, model, accessToken);
    }

    /// <summary>
    /// Updates an application role.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be updated.
    /// </param>
    /// <param name="model">
    /// The <see cref="RoleUpdateModel"/> to be used to
    /// update an application role.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="RoleModel"/>
    /// containing information about the updated role.
    /// </returns>
    public Task<Result<RoleModel>> UpdateAsync(Guid roleId, RoleUpdateModel model, string accessToken)
    {
        var uri = CreateUri(Update, routeParameters: new()
        {
            { RoleIdParameter, roleId }
        });

        return PatchAsync<RoleModel>(uri, model, accessToken);
    }

    /// <summary>
    /// Deletes an application role.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be deleted.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles.
    /// </returns>
    public Task<Result<IEnumerable<RoleModel>>> DeleteAsync(Guid roleId, string accessToken)
    {
        var uri = CreateUri(Delete, routeParameters: new()
        {
            { RoleIdParameter, roleId }
        });

        return DeleteAsync<IEnumerable<RoleModel>>(uri, accessToken);
    }

    /// <summary>
    /// Assigns an application role to a user.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be assigned.
    /// </param>
    /// <param name="userId">
    /// The Id of the user to whom to assign the role.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>.
    /// </returns>
    public Task<Result<IEnumerable<RoleModel>>> AssignAsync(Guid roleId, Guid userId, string accessToken)
    {
        var uri = CreateUri(Assign, routeParameters: new()
        {
            { RoleIdParameter, roleId },
            { UserIdParameter, userId }
        });

        return PatchAsync<IEnumerable<RoleModel>>(uri, null, accessToken);
    }

    /// <summary>
    /// Unassigns an application role from a user.
    /// </summary>
    /// <param name="roleId">
    /// The Id of the role to be unassigned.
    /// </param>
    /// <param name="userId">
    /// The Id of the user from whom to unassign the role.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="RoleModel"/>
    /// containing information about the application roles, which belong
    /// to the user with the given <paramref name="userId"/>.
    /// </returns>
    public Task<Result<IEnumerable<RoleModel>>> UnassignAsync(Guid roleId, Guid userId, string accessToken)
    {
        var uri = CreateUri(Unassign, routeParameters: new()
        {
            { RoleIdParameter, roleId },
            { UserIdParameter, userId }
        });

        return DeleteAsync<IEnumerable<RoleModel>>(uri, accessToken);
    }
    #endregion
}