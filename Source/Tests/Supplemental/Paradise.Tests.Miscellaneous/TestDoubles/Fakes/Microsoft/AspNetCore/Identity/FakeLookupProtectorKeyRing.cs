using Microsoft.AspNetCore.Identity;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Identity;

/// <summary>
/// Fake <see cref="ILookupProtectorKeyRing"/> implementation.
/// </summary>
public sealed class FakeLookupProtectorKeyRing : ILookupProtectorKeyRing
{
    #region Fields
    private readonly IDictionary<string, string> _keys = new Dictionary<string, string>
    {
        [Guid.NewGuid().ToString()] = Guid.NewGuid().ToString()
    };
    #endregion

    #region Properties
    /// <inheritdoc/>
    public string CurrentKeyId
        => _keys.LastOrDefault().Key ?? string.Empty;
    #endregion

    #region Indexes
    /// <inheritdoc/>
    public string this[string keyId]
        => _keys[keyId];
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IEnumerable<string> GetAllKeyIds()
        => _keys.Keys.ToList().AsReadOnly();
    #endregion
}