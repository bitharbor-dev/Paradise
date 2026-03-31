using Microsoft.Extensions.Options;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Users;
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
/// <see cref="HttpClient"/> instance the <see cref="UsersApiClient"/>
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

        return GetAsync<IEnumerable<UserModel>>(route, true, cancellationToken);
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
            [UserIdParameter] = userId.ToString()
        });

        return GetAsync<UserModel>(route, true, cancellationToken);
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

        return PostAsync<UserModel>(route, false, model, cancellationToken);
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
    public Task<Result<UserModel>> ConfirmEmailAddressAsync(string identityToken, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(ConfirmEmailAddress, routeParameters: new()
        {
            [IdentityTokenParameter] = identityToken
        });

        return GetAsync<UserModel>(route, false, cancellationToken);
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

        return PostAsync(route, false, model, cancellationToken);
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

        return PatchAsync(route, false, model, cancellationToken);
    }

    /// <summary>
    /// Creates an email address reset request.
    /// </summary>
    /// <param name="model">
    /// The <see cref="UserResetEmailAddressRequestModel"/> to be used to
    /// create an email address reset request.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> instance containing errors data if any occurs.
    /// </returns>
    public Task<Result> CreateEmailAddressResetRequestAsync(
        UserResetEmailAddressRequestModel model, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(CreateEmailAddressResetRequest);

        return PutAsync(route, true, model, cancellationToken);
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
    public Task<Result> ResetEmailAddressAsync(string identityToken, CancellationToken cancellationToken = default)
    {
        var route = CreateRoute(ResetEmailAddress, routeParameters: new()
        {
            [IdentityTokenParameter] = identityToken
        });

        return GetAsync(route, false, cancellationToken);
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

        return PatchAsync<UserModel>(route, true, model, cancellationToken);
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

        return DeleteAsync(route, true, cancellationToken);
    }
    #endregion
}