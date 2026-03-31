using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Mvc.ModelBinding;

/// <summary>
/// Fake <see cref="IValueProvider"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeValueProvider"/> class.
/// </remarks>
/// <param name="values">
/// A dictionary containing the key-value representation if the binding target.
/// </param>
public sealed class FakeValueProvider(Dictionary<string, StringValues> values) : IValueProvider
{
    #region Public methods
    /// <inheritdoc/>
    public bool ContainsPrefix(string prefix)
        => values.Keys.Any(key => key.StartsWith(prefix, StringComparison.Ordinal));

    /// <inheritdoc/>
    public ValueProviderResult GetValue(string key)
    {
        return values.TryGetValue(key, out var value)
            ? new ValueProviderResult(value, CultureInfo.InvariantCulture)
            : ValueProviderResult.None;
    }
    #endregion
}