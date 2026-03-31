using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Paradise.Common.Extensions;

namespace Paradise.WebApi.OpenApi.DocumentTransformers;

/// <summary>
/// Sets the <see cref="OpenApiInfo"/> instance to the <see cref="OpenApiDocument"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DocumentInfoSetter"/> class.
/// </remarks>
/// <param name="configuration">
/// The <see cref="IConfiguration"/> containing the document data.
/// </param>
internal sealed class DocumentInfoSetter(IConfiguration configuration) : IOpenApiDocumentTransformer
{
    #region Public methods
    /// <inheritdoc/>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        document.Info = configuration.GetRequiredInstance<OpenApiInfo>();

        return Task.CompletedTask;
    }
    #endregion
}