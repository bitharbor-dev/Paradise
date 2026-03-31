using System.Text.Json;
using Xunit.Sdk;

namespace Paradise.Tests.Miscellaneous.XunitSerialization;

/// <summary>
/// Provides an ability to serialize and deserialize xUnit theory data
/// using the built-in <see cref="JsonSerializer"/> functionalities.
/// </summary>
public sealed class XunitJsonSerializer : IXunitSerializer
{
    #region Public methods
    /// <inheritdoc/>
    public object Deserialize(Type type, string serializedValue)
        => JsonSerializer.Deserialize(serializedValue, type)!;

    /// <inheritdoc/>
    public bool IsSerializable(Type type, object? value, out string failureReason)
    {
        failureReason = string.Empty;

        return true;
    }

    /// <inheritdoc/>
    public string Serialize(object value)
        => JsonSerializer.Serialize(value);
    #endregion
}