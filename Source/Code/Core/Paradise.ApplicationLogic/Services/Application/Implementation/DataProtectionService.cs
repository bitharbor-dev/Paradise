using Microsoft.AspNetCore.DataProtection;
using Paradise.Common.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;

namespace Paradise.ApplicationLogic.Services.Application.Implementation;

/// <summary>
/// Provides data protection functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DataProtectionService"/> class.
/// </remarks>
/// <param name="dataProtectionProvider">
/// The <see cref="IDataProtectionProvider"/> to be used to
/// protect and unprotect the data.
/// </param>
public sealed class DataProtectionService(IDataProtectionProvider dataProtectionProvider) : IDataProtectionService
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
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.General);
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public string Protect<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, _jsonSerializerOptions);

        return dataProtectionProvider.CreateProtector(DataProtectionPurpose).Protect(json);
    }

    /// <inheritdoc/>
    public bool TryUnprotect<T>(string? token, [NotNullWhen(true)] out T? value)
    {
        value = default;

        if (token.IsNullOrWhiteSpace())
            return false;

        try
        {
            var json = dataProtectionProvider.CreateProtector(DataProtectionPurpose).Unprotect(token);

            value = JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);

            return value is not null;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public string GenerateRandomDigitCode(ushort length)
        => RandomNumberGenerator.GetString(Digits, length);
    #endregion
}