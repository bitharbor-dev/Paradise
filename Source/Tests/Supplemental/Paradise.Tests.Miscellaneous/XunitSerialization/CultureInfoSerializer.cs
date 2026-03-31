using System.Globalization;
using Xunit.Sdk;

namespace Paradise.Tests.Miscellaneous.XunitSerialization;

/// <summary>
/// Provides xUnit theory data serialization
/// capabilities for <see cref="CultureInfo"/> instances.
/// </summary>
public sealed class CultureInfoSerializer : XunitSerializer<CultureInfo>
{
    #region Public methods
    /// <inheritdoc/>
    public override CultureInfo Deserialize(Type type, string serializedValue)
        => CultureInfo.GetCultureInfo(serializedValue);

    /// <inheritdoc/>
    public override string Serialize(CultureInfo value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Name;
    }
    #endregion
}