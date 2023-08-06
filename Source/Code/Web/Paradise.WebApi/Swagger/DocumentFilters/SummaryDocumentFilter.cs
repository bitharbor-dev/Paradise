using Microsoft.OpenApi.Models;
using Paradise.Common.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Paradise.WebApi.Swagger.DocumentFilters;

/// <summary>
/// Formats operations summaries.
/// </summary>
internal sealed partial class SummaryDocumentFilter : IDocumentFilter
{
    #region Constants
    private const string LangwordTag = "<see langword=\"";
    private const string ClosingTagBracket = "\" />";
    #endregion

    #region Fields
    [GeneratedRegex($"{LangwordTag}.+{ClosingTagBracket}", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex LangwordTagRegex();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        static OpenApiOperation Selector(KeyValuePair<OperationType, OpenApiOperation> keyValuePair)
            => keyValuePair.Value;

        var operations = swaggerDoc
            .Paths
            .SelectMany(path => path.Value.Operations.Select(Selector));

        TrimNamespaces(operations);

        TrimLangwordTags(operations);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Trims namespaces names in the given <paramref name="operations"/> sequence.
    /// </summary>
    /// <param name="operations">
    /// Operations, which summaries to be formatted.
    /// </param>
    private static void TrimNamespaces(IEnumerable<OpenApiOperation> operations)
    {
        var namespaces = GetApplicationNamespaces();

        foreach (var operation in operations)
        {
            foreach (var ns in namespaces)
            {
                if (operation.RequestBody?.Description is not null)
                {
                    operation.RequestBody.Description
                        = operation.RequestBody.Description.Replace(ns, string.Empty, StringComparison.OrdinalIgnoreCase);
                }

                if (operation.Summary is not null)
                    operation.Summary = operation.Summary.Replace(ns, string.Empty, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    /// <summary>
    /// Removes all "langword" tags in the given <paramref name="operations"/> sequence.
    /// </summary>
    /// <param name="operations">
    /// Operations, which summaries to be cleared of "langword" tags.
    /// </param>
    private static void TrimLangwordTags(IEnumerable<OpenApiOperation> operations)
    {
        static bool Predicate(OpenApiOperation operation)
            => operation.Summary is not null;

        foreach (var operation in operations.Where(Predicate))
            operation.Summary = TrimLangwordTag(operation.Summary);
    }

    /// <summary>
    /// Removes all "langword" tag occurrences and leaves only inner values.
    /// </summary>
    /// <param name="input">
    /// The input string to be trimmed.
    /// </param>
    /// <returns>
    /// The input string but without "langword" tags.
    /// </returns>
    private static string TrimLangwordTag(string input)
    {
        static string Replacement(Match match)
        {
            var value = match.ToString();

            value = value
                .Replace(LangwordTag, string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace(ClosingTagBracket, string.Empty, StringComparison.OrdinalIgnoreCase);

            return value;
        }

        return LangwordTagRegex().Replace(input, Replacement);
    }

    /// <summary>
    /// Gets the list of the application namespaces names.
    /// </summary>
    /// <returns>
    /// The list of the application namespaces names.
    /// </returns>
    private static IEnumerable<string> GetApplicationNamespaces()
    {
        var currentAssembly = Assembly.GetExecutingAssembly();

        var currentAssemblyTypes = currentAssembly.GetTypes();

        var referencedAssembliesTypes = currentAssembly
            .GetReferencedAssemblies()
            .Select(Assembly.Load)
            .SelectMany(assembly => assembly.GetTypes());

        var targetAssembliesTypes = currentAssemblyTypes
            .Concat(referencedAssembliesTypes)
            .Where(type => type.Namespace.IsNotNullOrWhiteSpace()
                        && type.Namespace.StartsWith(nameof(Paradise), StringComparison.OrdinalIgnoreCase));

        var namespaces = targetAssembliesTypes
            .Select(type => $"{type.Namespace}.")
            .Distinct()
            .OrderByDescending(ns => ns.Length);

        return namespaces;
    }
    #endregion
}