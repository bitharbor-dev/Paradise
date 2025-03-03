using Microsoft.OpenApi.Models;
using Paradise.WebApi.OpenApi.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Paradise.WebApi.Swagger;

/// <summary>
/// Default Swagger document filter.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultOperationFilter"/> class.
/// </remarks>
/// <param name="scheme">
/// Scheme to be appended.
/// </param>
internal sealed class DefaultOperationFilter(OpenApiSecurityScheme scheme) : IOperationFilter
{
    #region Public methods
    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        operation.AddSecurityScheme(context.ApiDescription.ActionDescriptor, scheme);
    }
    #endregion
}