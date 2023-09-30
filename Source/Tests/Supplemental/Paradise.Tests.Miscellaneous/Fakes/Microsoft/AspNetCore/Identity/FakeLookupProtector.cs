using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Tests.Miscellaneous.Fakes.Microsoft.AspNetCore.Identity;

/// <summary>
/// Fake <see cref="ILookupProtector"/> implementation.
/// </summary>
internal sealed class FakeLookupProtector : ILookupProtector
{
    #region Public methods
    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(data))]
    public string? Protect(string keyId, string? data)
        => data is null ? null : $"{keyId}-{data}";

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(data))]
    public string? Unprotect(string keyId, string? data)
    {
        var placeholder = $"{keyId}-";
        var replacement = string.Empty;

        return data?.Replace(placeholder, replacement, StringComparison.OrdinalIgnoreCase);
    }
    #endregion
}