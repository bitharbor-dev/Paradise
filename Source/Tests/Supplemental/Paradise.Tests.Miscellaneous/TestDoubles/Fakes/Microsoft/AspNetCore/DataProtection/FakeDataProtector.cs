using Microsoft.AspNetCore.DataProtection;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.DataProtection;

/// <summary>
/// Fake <see cref="IDataProtector"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeDataProtector"/> class.
/// </remarks>
/// <param name="purpose">
/// The purpose of the current <see cref="IDataProtector"/> instance.
/// </param>
public sealed class FakeDataProtector(string purpose = FakeDataProtector.DefaultPurpose) : IDataProtector
{
    #region Constants
    /// <summary>
    /// Default purpose of this type of <see cref="IDataProtector"/>.
    /// </summary>
    private const string DefaultPurpose = "Testing";
    #endregion

    #region Properties
    /// <summary>
    /// The purpose of the current <see cref="IDataProtector"/> instance.
    /// </summary>
    public string Purpose { get; } = purpose;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IDataProtector CreateProtector(string purpose)
        => new FakeDataProtector(purpose);

    /// <inheritdoc/>
    public byte[] Protect(byte[] plaintext)
        => plaintext;

    /// <inheritdoc/>
    public byte[] Unprotect(byte[] protectedData)
        => protectedData;
    #endregion
}