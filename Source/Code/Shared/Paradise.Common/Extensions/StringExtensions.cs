using System.Diagnostics.CodeAnalysis;

namespace Paradise.Common.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="string"/> <see langword="class"/>.
/// </summary>
public static class StringExtensions
{
    #region Public methods
    /// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
        => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Indicates whether a specified string is not <see langword="null"/>, empty,
    /// or consists only of white-space characters.
    /// </summary>
    /// <param name="value">
    /// The <see cref="string"/> to test.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="value"/> parameter is not
    /// <see langword="null"/> or <see cref="string.Empty"/>, or if <paramref name="value"/>
    /// does not consist exclusively of white-space characters.
    /// </returns>
    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? value)
        => !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Replaces all '/' and '\' characters with <see cref="Path.DirectorySeparatorChar"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="string"/> to be sanitized.
    /// </param>
    /// <returns>
    /// A new <see cref="string"/> value equivalent to the given <paramref name="value"/>,
    /// except that all instances of '/' and '\' are replaced with <see cref="Path.DirectorySeparatorChar"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(value))]
    public static string? SanitizePathSeparators(this string? value)
        => value?.Replace('\\', Path.DirectorySeparatorChar)?.Replace('/', Path.DirectorySeparatorChar);
    #endregion
}