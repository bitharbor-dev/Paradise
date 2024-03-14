using System.Diagnostics.CodeAnalysis;

namespace Paradise.ApplicationLogic.Services.Application;

/// <summary>
/// Provides data protection functionalities.
/// </summary>
public interface IDataProtectionService
{
    #region Methods
    /// <summary>
    /// Generates an encrypted token that contains the given <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Value type.
    /// </typeparam>
    /// <param name="value">
    /// The <typeparamref name="T"/> to encrypted.
    /// </param>
    /// <returns>
    /// Encrypted token that contains the given <paramref name="value"/>.
    /// </returns>
    string Protect<T>(T value);

    /// <summary>
    /// Parses the given <paramref name="token"/> back into original value.
    /// </summary>
    /// <param name="token">
    /// String value to parse.
    /// </param>
    /// <param name="value">
    /// Parsed value.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="token"/> was parsed successfully,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    bool TryUnprotect<T>(string? token, [NotNullWhen(true)] out T? value);

    /// <summary>
    /// Generates a <see cref="string"/> containing
    /// the random digital code with the length of <paramref name="length"/>.
    /// </summary>
    /// <param name="length">
    /// Code length.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> containing
    /// the random digital code with the length of <paramref name="length"/>.
    /// </returns>
    string GenerateRandomDigitCode(ushort length);
    #endregion
}