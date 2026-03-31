using Paradise.ApplicationLogic.Infrastructure.DataProtection;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.ApplicationLogic.Infrastructure.DataProtection;

/// <summary>
/// Fake <see cref="IDataProtector"/> implementation.
/// </summary>
public sealed class StubDataProtector : IDataProtector
{
    #region Properties
    /// <summary>
    /// <see cref="GenerateRandomDigitCode"/> result.
    /// </summary>
    public string? GenerateRandomDigitCodeResult { get; set; }

    /// <summary>
    /// <see cref="Protect"/> result.
    /// </summary>
    public string? ProtectResult { get; set; }

    /// <summary>
    /// <see cref="TryUnprotect"/> result.
    /// </summary>
    public Tuple<bool, object?>? TryUnprotectResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public string GenerateRandomDigitCode(ushort length)
        => GenerateRandomDigitCodeResult ?? throw new NotImplementedException();

    /// <inheritdoc/>
    public string Protect<T>(T value)
        => ProtectResult ?? throw new NotImplementedException();

    /// <inheritdoc/>
    public bool TryUnprotect<T>(string? token, [NotNullWhen(true)] out T? value)
    {
        var tuple = TryUnprotectResult ?? throw new NotImplementedException();

        value = (T)tuple.Item2!;

        return tuple.Item1;
    }
    #endregion
}