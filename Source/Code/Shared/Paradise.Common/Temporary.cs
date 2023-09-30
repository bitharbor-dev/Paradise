using System.Diagnostics.CodeAnalysis;

namespace Paradise.Common;

/// <summary>
/// Provides data for IDE0290 message suppression.
/// </summary>
public static class SuppressionOfIDE0290
{
    #region Constants
    /// <inheritdoc cref="SuppressMessageAttribute.Category"/>
    public const string Category = "Style";

    /// <inheritdoc cref="SuppressMessageAttribute.CheckId"/>
    public const string CheckId = "IDE0290:Use primary constructor";

    /// <inheritdoc cref="SuppressMessageAttribute.Justification"/>
    public const string Justification = "Primary constructors not working with constructor attributes.";
    #endregion
}