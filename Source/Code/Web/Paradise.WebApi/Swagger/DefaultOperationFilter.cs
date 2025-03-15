using Microsoft.OpenApi.Models;
using Paradise.WebApi.OpenApi.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Paradise.WebApi.Swagger;

/// <summary>
/// Default <see cref="OpenApiOperation"/> filter.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultOperationFilter"/> class.
/// </remarks>
/// <param name="scheme">
/// Scheme to be appended.
/// </param>
/// <param name="scopes">
/// The list of scope names required for <paramref name="scheme"/> execution.
/// <para>
/// If security scheme is not "<c>oauth2</c>" or "<c>openIdConnect</c>" - the array MUST be empty.
/// </para>
/// </param>
internal sealed class DefaultOperationFilter(OpenApiSecurityScheme scheme, IList<string>? scopes = null) : IOperationFilter
{
    #region Public methods
    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        operation.AddSecurityScheme(context.ApiDescription.ActionDescriptor, scheme, scopes ?? []);
    }
    #endregion
}