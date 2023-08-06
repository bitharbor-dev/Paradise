using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paradise.Models;
using Paradise.WebApi.Filters.Validation;

namespace Paradise.WebApi.Controllers.Base;

/// <summary>
/// Base API controller class.
/// </summary>
[ApiController, RequireHttps, ValidateModel]
[Consumes(Result.ContentType), Produces(Result.ContentType)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public abstract class ApiControllerBase : ControllerBase
{
    #region Constants
    /// <summary>
    /// Request authorization header name.
    /// </summary>
    public const string AuthorizationHeaderName = "Authorization";
    #endregion
}