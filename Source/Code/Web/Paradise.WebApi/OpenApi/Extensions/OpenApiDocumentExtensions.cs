using Microsoft.OpenApi.Models;
using Paradise.Common.Extensions;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Paradise.WebApi.OpenApi.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="OpenApiDocument"/> <see langword="class"/>.
/// </summary>
internal static partial class OpenApiDocumentExtensions
{
    #region Constants
    /// <summary>
    /// First part of "langword" tag.
    /// </summary>
    private const string LangwordTagStart = "<see langword=\"";
    /// <summary>
    /// Second part of "langword" tag.
    /// </summary>
    private const string LangwordTagEnd = "\" />";
    /// <summary>
    /// URI segments separator.
    /// </summary>
    private const char PathSeparator = '/';
    #endregion

    #region Public methods
    /// <summary>
    /// Trims namespaces names in the given <paramref name="document"/>.
    /// </summary>
    /// <param name="document">
    /// The <see cref="OpenApiDocument"/>, which operation summaries to be formatted.
    /// </param>
    public static void TrimNamespaces(this OpenApiDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        static OpenApiOperation Selector(KeyValuePair<OperationType, OpenApiOperation> keyValuePair)
            => keyValuePair.Value;

        const string Replacement = "";

        var namespaces = GetApplicationNamespaces();

        var operations = document
            .Paths
            .SelectMany(path => path.Value.Operations.Select(Selector));

        foreach (var operation in operations)
        {
            foreach (var nameSpace in namespaces)
            {
                if (operation.RequestBody?.Description is not null)
                {
                    operation.RequestBody.Description = operation
                        .RequestBody
                        .Description
                        .Replace(nameSpace, Replacement, StringComparison.OrdinalIgnoreCase);
                }

                if (operation.Summary is not null)
                {
                    operation.Summary = operation
                        .Summary
                        .Replace(nameSpace, Replacement, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }

    /// <summary>
    /// Removes all "langword" tags in the given <paramref name="document"/>.
    /// </summary>
    /// <param name="document">
    /// The <see cref="OpenApiDocument"/>, which operation summaries to be cleared of "langword" tags.
    /// </param>
    public static void TrimLangwordTags(this OpenApiDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        static OpenApiOperation Selector(KeyValuePair<OperationType, OpenApiOperation> keyValuePair)
            => keyValuePair.Value;

        static bool Predicate(OpenApiOperation operation)
            => operation.Summary is not null;

        var operations = document
            .Paths
            .SelectMany(path => path.Value.Operations.Select(Selector));

        foreach (var operation in operations.Where(Predicate))
            operation.Summary = TrimLangwordTag(operation.Summary);
    }

    /// <summary>
    /// Formats all paths to the camel case in the given <paramref name="document"/>.
    /// </summary>
    /// <param name="document">
    /// The <see cref="OpenApiDocument"/>, which paths to be formatted.
    /// </param>
    public static void FormatPathsToCamelCase(this OpenApiDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var paths = new OpenApiPaths();

        document
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

        document.Paths = paths;
    }
    #endregion

    #region Private methods
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
        static string GetReplacementValue(Match match)
        {
            const string Replacement = "";

            var value = match.ToString();

            value = value
                .Replace(LangwordTagStart, Replacement, StringComparison.OrdinalIgnoreCase)
                .Replace(LangwordTagEnd, Replacement, StringComparison.OrdinalIgnoreCase);

            return value;
        }

        return GetLangwordTagRegex().Replace(input, GetReplacementValue);
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

    [GeneratedRegex($"{LangwordTagStart}.+{LangwordTagEnd}", RegexOptions.IgnoreCase)]
    private static partial Regex GetLangwordTagRegex();
    #endregion
}