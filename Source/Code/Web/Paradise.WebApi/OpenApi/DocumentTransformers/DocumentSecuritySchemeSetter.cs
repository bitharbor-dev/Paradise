using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Paradise.Common.Extensions;

namespace Paradise.WebApi.OpenApi.DocumentTransformers;

/// <summary>
/// Sets the <see cref="OpenApiSecurityScheme"/> instance to the <see cref="OpenApiDocument"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DocumentSecuritySchemeSetter"/> class.
/// </remarks>
/// <param name="configuration">
/// The <see cref="IConfiguration"/> containing the document data.
/// </param>
internal sealed class DocumentSecuritySchemeSetter(IConfiguration configuration) : IOpenApiDocumentTransformer
{
    #region Public methods
    /// <inheritdoc/>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        var scheme = configuration.GetRequiredInstance<OpenApiSecurityScheme>();

        ArgumentException.ThrowIfNullOrWhiteSpace(scheme.Scheme);

        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes.Add(scheme.Scheme, scheme);

        return Task.CompletedTask;
    }
    #endregion
}