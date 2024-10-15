using Microsoft.Extensions.Options;
using Paradise.Models;
using Paradise.Models.Domain.UserModels;
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
/// <param name="jsonSerializerOptions">
/// The accessor used to access the <see cref="JsonSerializerOptions"/>.
/// </param>
/// <param name="httpClient">
/// <see cref="HttpClient"/> instance the <see cref="EmailTemplatesApiClient"/>
/// will operate over.
/// </param>
public sealed class UsersApiClient(IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions, HttpClient httpClient)
    : ApiClientBase(jsonSerializerOptions, httpClient)
{
    #region Public methods
    /// <summary>
    /// Gets the list of application users.
    /// </summary>
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
    public Task<Result<IEnumerable<UserModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(GetAll);

        return GetAsync<IEnumerable<UserModel>>(route, cancellationToken);
    }

    /// <summary>
    /// Gets the user with the given <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">
    /// The Id of the user to be found.
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
    public Task<Result<UserModel>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(GetById, routeParameters: new()
        {
            { UserIdParameter, userId }
        });

        return GetAsync<UserModel>(route, cancellationToken);
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
        var route = CreateRoute(Register);

        return PostAsync<UserModel>(route, model, cancellationToken);
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
        var route = CreateRoute(ConfirmEmail, routeParameters: new()
        {
            { IdentityTokenParameter, identityToken }
        });

        return GetAsync<UserModel>(route, cancellationToken);
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
        var route = CreateRoute(Login);

        return PostAsync<UserAuthorizationTokenModel>(route, model, cancellationToken);
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
        var route = CreateRoute(ConfirmLogin);

        return PutAsync<UserAuthorizationTokenModel>(route, model, cancellationToken);
    }

    /// <summary>
    /// Generates a new user authorization token
    /// using the access token.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> where
    /// <see cref="Result{TValue}.Value"/> is a <see cref="UserAuthorizationTokenModel"/>
    /// containing information about the user authorization token.
    /// </returns>
    public Task<Result<UserAuthorizationTokenModel>> RenewTokenAsync(CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(RenewToken);

        return GetAsync<UserAuthorizationTokenModel>(route, cancellationToken);
    }

    /// <summary>
    /// Invalidates the access token
    /// to make it unusable during the authentication process.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> LogoutAsync(CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(Logout);

        return DeleteAsync(route, cancellationToken);
    }

    /// <summary>
    /// Invalidates all user's refresh tokens
    /// to make them all unusable during the authentication process.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> LogoutEverywhereAsync(CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(LogoutEverywhere);

        return DeleteAsync(route, cancellationToken);
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
        var route = CreateRoute(CreatePasswordResetRequest);

        return PostAsync(route, model, cancellationToken);
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
        var route = CreateRoute(ResetPassword);

        return PatchAsync(route, model, cancellationToken);
    }

    /// <summary>
    /// Creates an email address reset request.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetEmailRequestModel"/> to be used to
    /// create an email address reset request.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> CreateEmailResetRequestAsync(UserResetEmailRequestModel model, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(CreateEmailResetRequest);

        return PutAsync(route, model, cancellationToken);
    }

    /// <summary>
    /// Resets the user's email address.
    /// </summary>
    /// <param name="identityToken">
    /// An encrypted string value to be used to
    /// reset the user's email address.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> ResetEmailAsync(string identityToken, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(ResetEmail, routeParameters: new()
        {
            { IdentityTokenParameter, identityToken }
        });

        return GetAsync(route, cancellationToken);
    }

    /// <summary>
    /// Updates the user.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserUpdateModel"/> to be used to
    /// update the user.
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
    public Task<Result<UserModel>> UpdateAsync(UserUpdateModel model, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(Update);

        return PatchAsync<UserModel>(route, model, cancellationToken);
    }

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> DeleteAsync(CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(Delete);

        return DeleteAsync(route, cancellationToken);
    }
    #endregion
}