using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;

namespace Paradise.WebApi.Swagger.DocumentFilters;

/// <summary>
/// Lowers all endpoints paths.
/// </summary>
public sealed class EndpointPathToLowercaseDocumentFilter : IDocumentFilter
{
    #region Constants
    /// <summary>
    /// URI segments separator.
    /// </summary>
    private const char PathSeparator = '/';
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(swaggerDoc);

        var paths = new OpenApiPaths();

        swaggerDoc
            .Paths
            .Select(item =>
            {
                var decomposedKey = item
                    .Key
                    .Split(PathSeparator)
                    .Where(segment => !string.IsNullOrEmpty(segment))
                    .Select(segment => char.ToLower(segment.First(), CultureInfo.InvariantCulture) + segment[1..]);

                var camelCaseKey = PathSeparator + string.Join(PathSeparator, decomposedKey);

                return new KeyValuePair<string, OpenApiPathItem>(camelCaseKey, item.Value);
            })
            .ToList()
            .ForEach(item => paths.Add(item.Key, item.Value));

        swaggerDoc.Paths = paths;
    }
    #endregion
}