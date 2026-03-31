using Microsoft.AspNetCore.DataProtection;
using Paradise.Common.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;
using IProtector = Microsoft.AspNetCore.DataProtection.IDataProtector;

namespace Paradise.ApplicationLogic.Infrastructure.DataProtection.Implementation;

/// <summary>
/// Provides data protection functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DataProtector"/> class.
/// </remarks>
/// <param name="dataProtectionProvider">
/// The <see cref="IDataProtectionProvider"/> to be used to
/// protect and unprotect the data.
/// </param>
internal sealed class DataProtector(IDataProtectionProvider dataProtectionProvider) : IDataProtector
{
    #region Constants
    /// <summary>
    /// Default data protection purpose.
    /// </summary>
    public const string DataProtectionPurpose = "DataProtection";

    /// <summary>
    /// The "All digit" string used to generate random digit codes.
    /// </summary>
    private const string Digits = "0123456789";
    #endregion

    #region Fields
    private readonly IProtector _protector = dataProtectionProvider.CreateProtector(DataProtectionPurpose);
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public string Protect<T>(T value)
    {
        var json = JsonSerializer.Serialize(value);

        return _protector.Protect(json);
    }

    /// <inheritdoc/>
    public bool TryUnprotect<T>(string? token, [NotNullWhen(true)] out T? value)
    {
        value = default;

        if (token.IsNullOrWhiteSpace())
            return false;

        try
        {
            var json = _protector.Unprotect(token);

            value = JsonSerializer.Deserialize<T>(json);

            return value is not null;
        }
        catch (Exception exception) when (exception is JsonException or NotSupportedException or CryptographicException)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public string GenerateRandomDigitCode(ushort length)
        => RandomNumberGenerator.GetString(Digits, length);
    #endregion
}