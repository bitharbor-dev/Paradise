using Paradise.ApplicationLogic.Infrastructure.DataProtection;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.DataProtection;

/// <summary>
/// Fake <see cref="IDataProtector"/> implementation.
/// </summary>
public sealed class FakeDataProtector : IDataProtector
{
    #region Public methods
    /// <inheritdoc/>
    public string GenerateRandomDigitCode(ushort length)
        => RandomNumberGenerator.GetString("0123456789", length);

    /// <inheritdoc/>
    public string Protect<T>(T value)
        => JsonSerializer.Serialize(value);

    /// <inheritdoc/>
    public bool TryUnprotect<T>(string? token, [NotNullWhen(true)] out T? value)
    {
        value = default;

        if (token is null)
            return false;

        try
        {
            value = JsonSerializer.Deserialize<T>(token);

            return value is not null;
        }
        catch (Exception exception) when (exception is JsonException or NotSupportedException)
        {
            return false;
        }
    }
    #endregion
}