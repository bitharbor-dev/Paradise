using Microsoft.OpenApi.Models;
using Paradise.WebApi.OpenApi.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Paradise.WebApi.Swagger;

/// <summary>
/// Default <see cref="OpenApiDocument"/> filter.
/// </summary>
internal sealed class DefaultDocumentFilter : IDocumentFilter
{
    #region Public methods
    /// <inheritdoc/>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(swaggerDoc);

        swaggerDoc.TrimNamespaces();
        swaggerDoc.TrimLangwordTags();
        swaggerDoc.FormatPathsToCamelCase();
    }
    #endregion
}