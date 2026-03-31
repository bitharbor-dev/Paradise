using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Paradise.WebApi.OpenApi.DocumentTransformers;

/// <summary>
/// Sets the <see cref="OpenApiInfo"/> instance to the <see cref="OpenApiDocument"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DocumentPathsCamelCaseFormatter"/> class.
/// </remarks>
internal sealed class DocumentPathsCamelCaseFormatter : IOpenApiDocumentTransformer
{
    #region Constants
    /// <summary>
    /// URI segments separator.
    /// </summary>
    private const char PathSeparator = '/';
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        FormatPathsToCamelCase(document);

        return Task.CompletedTask;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Formats all paths to the camel case in the given <paramref name="document"/>.
    /// </summary>
    /// <param name="document">
    /// The <see cref="OpenApiDocument"/>, which paths to be formatted.
    /// </param>
    private static void FormatPathsToCamelCase(OpenApiDocument document)
    {
        static void LowercaseFirstCharacter(Span<char> span, string value)
        {
            value.AsSpan().CopyTo(span);
            span[0] = char.ToLowerInvariant(span[0]);
        }

        document.Paths = document.Paths.Aggregate(new OpenApiPaths(), (paths, pair) =>
        {
            var path = pair.Key;
            var item = pair.Value;

            var segments = path
                .Split(PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var decomposedPath = segments.Select(segment => string.Create(segment.Length, segment, LowercaseFirstCharacter));
            var camelCasePath = PathSeparator + string.Join(PathSeparator, decomposedPath);

            paths.Add(camelCasePath, item);

            return paths;
        });
    }
    #endregion
}