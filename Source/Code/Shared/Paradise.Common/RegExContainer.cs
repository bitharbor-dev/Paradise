using System.Diagnostics.CodeAnalysis;

namespace Paradise.Common;

/// <summary>
/// Regular expressions container class.
/// </summary>
public static class RegExContainer
{
    #region Constants
    /// <summary>
    /// Allows only alphabet uppercase and lowercase characters, without whitespace.
    /// </summary>
    [StringSyntax("Regex")]
    public const string OnlyAlphabetCharacters = "^[a-zA-Z]*$";
    #endregion
}