using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Paradise.Models;
using Paradise.Models.Domain.UserModels;
using Paradise.Options.Models;
using Paradise.WebApi.Client.Application;
using Paradise.WebApi.Client.Base;
using System.Text.Json;
using static Paradise.Common.Web.ParameterNames;
using static Paradise.Common.Web.UserRoutes;

namespace Paradise.WebApi.Client.Domain;

/// <summary>
/// Contains all web API requests to the UsersController.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UsersApiClient"/> class.
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
public sealed class UsersApiClient(IOptionsMonitor<ApplicationOptions> applicationOptions,
                                   IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions,
                                   HttpClient httpClient,
                                   string schemeName = JwtBearerDefaults.AuthenticationScheme)
    : ApiClientBase(applicationOptions, jsonSerializerOptions, httpClient, schemeName)
{
    #region Public methods
    /// <summary>
    /// Gets the list of application users.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is an <see cref="IEnumerable{T}"/>
    /// of <see cref="UserModel"/>
    /// containing information about the application users.
    /// </returns>
    public Task<Result<IEnumerable<UserModel>>> GetAllAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(GetAll);

        return GetAsync<IEnumerable<UserModel>>(uri, accessToken, cancellationToken);
    }

    /// <summary>
    /// Gets the user with the given <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user to be found.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the user found.
    /// </returns>
    public Task<Result<UserModel>> GetByIdAsync(Guid userId, string accessToken, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(GetById, routeParameters: new()
        {
            { UserIdParameter, userId }
        });

        return GetAsync<UserModel>(uri, accessToken, cancellationToken);
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserRegistrationModel"/> to be used to
    /// register a new user.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the created user.
    /// </returns>
    public Task<Result<UserModel>> RegisterAsync(UserRegistrationModel model, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(Register);

        return PostAsync<UserModel>(uri, model, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Confirms the user's email address.
    /// </summary>
    /// <param name="identityToken">
    /// An encrypted string value to be used to
    /// confirm the user's email address.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the updated user.
    /// </returns>
    public Task<Result<UserModel>> ConfirmEmailAsync(string identityToken, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(ConfirmEmail, routeParameters: new()
        {
            { IdentityTokenParameter, identityToken }
        });

        return GetAsync<UserModel>(uri, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Generates a new user authorization token or
    /// two-factor authentication token in case it is enabled for the user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserLoginModel"/> to be used to
    /// validate login data and generate an access token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserAuthorizationTokenModel"/>
    /// containing information about the user authorization token or
    /// two-factor authentication token in case it is enabled for the user.
    /// </returns>
    public Task<Result<UserAuthorizationTokenModel>> LoginAsync(UserLoginModel model, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(Login);

        return PostAsync<UserAuthorizationTokenModel>(uri, model, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Generates a new user authorization token
    /// for the user with two-factor authentication enabled.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserTwoFactorAuthenticationModel"/> to be used to
    /// validate the login data and generate an access token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserAuthorizationTokenModel"/>
    /// containing information about the user authorization token.
    /// </returns>
    public Task<Result<UserAuthorizationTokenModel>> ConfirmLoginAsync(UserTwoFactorAuthenticationModel model, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(ConfirmLogin);

        return PutAsync<UserAuthorizationTokenModel>(uri, model, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Generates a new user authorization token
    /// using the given <paramref name="accessToken"/>.
    /// </summary>
    /// <param name="accessToken">
    /// User authorization token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserAuthorizationTokenModel"/>
    /// containing information about the user authorization token.
    /// </returns>
    public Task<Result<UserAuthorizationTokenModel>> RenewTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(RenewToken, routeParameters: new()
        {
            { AccessTokenParameter, accessToken }
        });

        return GetAsync<UserAuthorizationTokenModel>(uri, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Invalidates the given <paramref name="accessToken"/>
    /// to make it unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token to be invalidated.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> LogoutAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(Logout);

        return DeleteAsync(uri, accessToken, cancellationToken);
    }

    /// <summary>
    /// Invalidates all user's refresh tokens
    /// to make them all unusable during the authentication process.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token to be invalidated.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> LogoutEverywhereAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(LogoutEverywhere);

        return DeleteAsync(uri, accessToken, cancellationToken);
    }

    /// <summary>
    /// Creates a password reset request.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetPasswordRequestModel"/> to be used to
    /// create a password reset request.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> CreatePasswordResetRequestAsync(UserResetPasswordRequestModel model, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(CreatePasswordResetRequest);

        return PostAsync(uri, model, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Resets the user's password.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetPasswordModel"/> to be used to
    /// reset the user's password.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> ResetPasswordAsync(UserResetPasswordModel model, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(ResetPassword);

        return PatchAsync(uri, model, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Creates an email address reset request.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetEmailRequestModel"/> to be used to
    /// create an email address reset request.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> CreateEmailResetRequestAsync(UserResetEmailRequestModel model, string accessToken, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(CreateEmailResetRequest);

        return PutAsync(uri, model, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Resets the user's email address.
    /// </summary>
    /// <param name="identityToken">
    /// An encrypted string value to be used to
    /// reset the user's email address.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> ResetEmailAsync(string identityToken, string accessToken, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(ResetEmail, routeParameters: new()
        {
            { IdentityTokenParameter, identityToken }
        });

        return GetAsync(uri, accessToken, cancellationToken);
    }

    /// <summary>
    /// Updates the user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserUpdateModel"/> to be used to
    /// update the user.
    /// </param>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserModel"/>
    /// containing information about the updated user.
    /// </returns>
    public Task<Result<UserModel>> UpdateAsync(UserUpdateModel model, string accessToken, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(Update);

        return PatchAsync<UserModel>(uri, model, accessToken, cancellationToken);
    }

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="accessToken">
    /// Authorization token.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> DeleteAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(Delete);

        return DeleteAsync(uri, accessToken, cancellationToken);
    }
    #endregion
}