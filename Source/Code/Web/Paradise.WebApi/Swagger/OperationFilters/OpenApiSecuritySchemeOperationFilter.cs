using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Paradise.WebApi.Swagger.OperationFilters;

/// <summary>
/// Appends the authorization schema to the operation description.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OpenApiSecuritySchemeOperationFilter"/> class.
/// </remarks>
/// <param name="scheme">
/// Scheme to be appended.
/// </param>
internal sealed class OpenApiSecuritySchemeOperationFilter(OpenApiSecurityScheme scheme) : IOperationFilter
{
    #region Public methods
    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authRequired = !context.ApiDescription
            .CustomAttributes()
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (authRequired)
            operation.Security.Add(new() { { scheme, [] } });
    }
    #endregion
}