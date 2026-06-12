using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Paradise.Primitives.Extensions;

namespace Paradise.WebApi.OpenApi.OperationTransformers;

/// <summary>
/// Default <see cref="IOpenApiOperationTransformer"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OperationSecuritySchemeSetter"/> class.
/// </remarks>
/// <param name="configuration">
/// The <see cref="IConfiguration"/> containing the operation data.
/// </param>
internal sealed class OperationSecuritySchemeSetter(IConfiguration configuration) : IOpenApiOperationTransformer
{
    #region Public methods
    /// <inheritdoc/>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(context);

        var scheme = configuration.GetRequiredInstance<OpenApiSecurityScheme>();

        var descriptor = context.Description.ActionDescriptor;
        var schemeReference = new OpenApiSecuritySchemeReference(scheme.Scheme!, context.Document);

        AddSecurityScheme(operation, descriptor, schemeReference);

        return Task.CompletedTask;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Adds the given scheme <paramref name="reference"/> to the input <paramref name="operation"/>
    /// using the data from <paramref name="descriptor"/>.
    /// </summary>
    /// <param name="operation">
    /// The <see cref="OpenApiOperation"/> to which to add the given <paramref name="reference"/>.
    /// </param>
    /// <param name="descriptor">
    /// The <see cref="ActionDescriptor"/> instance containing the <paramref name="operation"/> information.
    /// </param>
    /// <param name="reference">
    /// The <see cref="OpenApiSecuritySchemeReference"/> to add.
    /// </param>
    /// <param name="scopes">
    /// The list of scope names required for scheme execution.
    /// <para>
    /// If referenced security scheme is not "<c>oauth2</c>" or "<c>openIdConnect</c>" - the array MUST be empty.
    /// </para>
    /// </param>
    private static void AddSecurityScheme(OpenApiOperation operation, ActionDescriptor descriptor,
                                          OpenApiSecuritySchemeReference reference, params List<string> scopes)
    {
        if (!descriptor.EndpointMetadata.Any(item => item is AllowAnonymousAttribute))
        {
            operation.Security ??= [];
            operation.Security.Add(new()
            {
                [reference] = scopes
            });
        }
    }
    #endregion
}