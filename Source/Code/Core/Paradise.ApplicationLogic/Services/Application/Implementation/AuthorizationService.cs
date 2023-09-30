using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Paradise.ApplicationLogic.Extensions;
using Paradise.ApplicationLogic.Identity;
using Paradise.DataAccess.Repositories.Domain;
using Paradise.Domain.Users;
using Paradise.Models;
using Paradise.Models.Extensions;
using Paradise.Options.Models;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using static Paradise.Models.ErrorCode;
using static System.Net.HttpStatusCode;

namespace Paradise.ApplicationLogic.Services.Application.Implementation;

/// <summary>
/// Provides authorization functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the<see cref="AuthorizationService"/> class.
/// </remarks>
/// <param name="logger">
/// Logger.
/// </param>
/// <param name="applicationOptions">
/// The accessor used to access the <see cref="ApplicationOptions"/>.
/// </param>
/// <param name="jwtBearerOptions">
/// The accessor used to access the <see cref="JwtBearerOptions"/>.
/// </param>
/// <param name="jsonSerializerOptions">
/// The accessor used to access the <see cref="JsonSerializerOptions"/>.
/// </param>
/// <param name="userManager">
/// User manager.
/// </param>
/// <param name="userRefreshTokensRepository">
/// User refresh tokens repository.
/// </param>
/// <param name="jsonWebTokenService">
/// JWT service.
/// </param>
public sealed class AuthorizationService(ILogger<AuthorizationService> logger,
                                         IOptions<ApplicationOptions> applicationOptions,
                                         IOptions<JwtBearerOptions> jwtBearerOptions,
                                         IOptions<JsonSerializerOptions>? jsonSerializerOptions,
                                         UserManager userManager,
                                         IUserRefreshTokensRepository userRefreshTokensRepository,
                                         IJsonWebTokenService jsonWebTokenService)
    : IAuthorizationService
{
    #region Fields
    private readonly ApplicationOptions _applicationOptions = applicationOptions.Value;
    private readonly JwtBearerOptions _jwtBearerOptions = jwtBearerOptions.Value;
    private readonly JsonSerializerOptions? _jsonSerializerOptions = jsonSerializerOptions?.Value;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async Task OnAuthenticationFailedAsync(HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        try
        {
            await WriteErrorResultAsync(response, Unauthorized, UnauthorizedUser);
        }
        catch (Exception e)
        {
            await WriteExceptionResultAsync(response, e);
        }
    }

    /// <inheritdoc/>
    public async Task OnForbiddenAsync(HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        try
        {
            await WriteErrorResultAsync(response, Forbidden, AccessForbidden);
        }
        catch (Exception e)
        {
            await WriteExceptionResultAsync(response, e);
        }
    }

    /// <inheritdoc/>
    public async Task OnTokenValidatedAsync(HttpResponse response, ClaimsPrincipal? principal,
                                            SecurityToken securityToken, Action<string> failureDelegate)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(securityToken);
        ArgumentNullException.ThrowIfNull(failureDelegate);

        try
        {
            if (!TryGetRefreshTokenId(securityToken, out var refreshTokenId))
            {
                var error = InvalidToken;

                await WriteErrorResultAsync(response, Unauthorized, error);

                failureDelegate(error.GetFormattedErrorDescription());
                return;
            }

            var refreshToken = await userRefreshTokensRepository.GetByIdAsync(refreshTokenId);

            var tokenIsOutdated = refreshToken is null
                || refreshToken.IsOutdated(_applicationOptions.Authentication.RefreshTokenLifetime);

            if (tokenIsOutdated)
            {
                var error = OutdatedToken;
                var errorDescription = error.GetFormattedErrorDescription();

                await WriteErrorResultAsync(response, Unauthorized, error);

                failureDelegate(errorDescription);
                return;
            }

            if (principal is null)
            {
                var error = UnauthorizedUser;
                var errorDescription = error.GetFormattedErrorDescription();

                await WriteErrorResultAsync(response, Unauthorized, error);

                failureDelegate(errorDescription);
                return;
            }

            var user = await userManager.GetUserAsync(principal);

            if (user is null)
            {
                var error = TokenOwnerNotExists;
                var errorDescription = error.GetFormattedErrorDescription();

                await WriteErrorResultAsync(response, Unauthorized, error);

                failureDelegate(errorDescription);
                return;
            }

            var roles = await userManager.GetRolesAsync(user);
            var rolesAreMatching = PrincipalRolesAreMatching(principal, roles);

            if (!rolesAreMatching)
            {
                var error = OutdatedToken;
                var errorDescription = error.GetFormattedErrorDescription();

                await WriteErrorResultAsync(response, Unauthorized, error);

                failureDelegate(errorDescription);
                return;
            }
        }
        catch (Exception e)
        {
            await WriteExceptionResultAsync(response, e);
        }
    }

    /// <inheritdoc/>
    public async Task OnChallengeAsync(HttpResponse response, Action handleResponseDelegate)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(handleResponseDelegate);

        try
        {
            if (response.HasStarted)
            {
                handleResponseDelegate();
                return;
            }

            if (!TryGetAccessToken(response, out var token))
            {
                await WriteErrorResultAsync(response, Unauthorized, UnauthorizedUser);

                handleResponseDelegate();
            }

            if (!jsonWebTokenService.ValidateFormat(token))
            {
                await WriteErrorResultAsync(response, Unauthorized, InvalidToken);

                handleResponseDelegate();
            }
        }
        catch (Exception e)
        {
            await WriteExceptionResultAsync(response, e);
        }
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Writes the given error information
    /// as a <see cref="HttpResponse"/> content.
    /// </summary>
    /// <param name="response">
    /// Response object to write in.
    /// </param>
    /// <param name="statusCode">
    /// Response status code.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    private async Task WriteErrorResultAsync(HttpResponse response, HttpStatusCode statusCode,
                                             ErrorCode errorCode, params string[] args)
    {
        if (response.HasStarted)
            return;

        var result = new Result();
        result.AddError(statusCode, errorCode, args);

        await result.WriteResponseContentAsync(response, _jsonSerializerOptions);

        await response.CompleteAsync();
    }

    /// <summary>
    /// Writes the given <paramref name="exception"/>
    /// as a <see cref="HttpResponse"/> content.
    /// </summary>
    /// <param name="response">
    /// Response object to write in.
    /// </param>
    /// <param name="exception">
    /// The <see cref="Exception"/> to be written.
    /// </param>
    private async Task WriteExceptionResultAsync(HttpResponse response, Exception exception)
    {
        logger.LogUnhandledException(exception);

        if (response.HasStarted)
            return;

        var result = new Result();
        result.AddException(exception);

        await result.WriteResponseContentAsync(response, _jsonSerializerOptions);

        await response.CompleteAsync();
    }

    /// <summary>
    /// Gets the <see cref="bool"/> value indicating whether the
    /// <paramref name="principal"/>'s set of "Role" claims
    /// is matching the given <paramref name="roles"/>.
    /// </summary>
    /// <param name="principal">
    /// The <see cref="ClaimsPrincipal"/> which claims to be inspected.
    /// </param>
    /// <param name="roles">
    /// Set of roles to be matched.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the given <paramref name="principal"/>
    /// have a set of "Role" <see cref="Claim"/>s
    /// which are matching the given <paramref name="roles"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private bool PrincipalRolesAreMatching(ClaimsPrincipal principal, IEnumerable<string> roles)
    {
        var roleClaimType = _jwtBearerOptions.TokenValidationParameters.RoleClaimType;
        var principalRoles = principal.FindValues(roleClaimType).Order();

        return principalRoles.SequenceEqual(roles.Order());
    }

    /// <summary>
    /// Attempts to get the <see cref="UserRefreshToken"/> Id
    /// from the given <paramref name="securityToken"/>.
    /// </summary>
    /// <param name="securityToken">
    /// The <see cref="SecurityToken"/> from which the
    /// <see cref="UserRefreshToken"/> Id to be retrieved.
    /// </param>
    /// <param name="tokenId">
    /// <see cref="UserRefreshToken"/> Id.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="UserRefreshToken"/> Id
    /// retrieved successfully, otherwise - <see langword="false"/>.
    /// </returns>
    private static bool TryGetRefreshTokenId(SecurityToken securityToken, out Guid tokenId)
    {
        if (securityToken is not JwtSecurityToken jwtSecurityToken)
        {
            tokenId = default;
            return false;
        }

        return Guid.TryParse(jwtSecurityToken.Id, out tokenId);
    }

    /// <summary>
    /// Attempts to get the access token from the given response instance.
    /// </summary>
    /// <param name="response">
    /// The <see cref="HttpResponse"/> to get the access token from.
    /// </param>
    /// <param name="accessToken">
    /// Access token.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the token was retrieved successfully,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private static bool TryGetAccessToken(HttpResponse response, [NotNullWhen(true)] out string? accessToken)
    {
        var headers = response.HttpContext.Request.Headers;
        var result = headers.TryGetValue(HeaderNames.Authorization, out var value);

        accessToken = value.FirstOrDefault();

        return result;
    }
    #endregion
}