using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Paradise.Tests.Miscellaneous.Json.Converters;

/// <summary>
/// <see cref="CultureInfo"/> JSON converter.
/// </summary>
public sealed class CultureInfoConverter : JsonConverter<CultureInfo?>
{
    #region Public methods
    /// <inheritdoc />
    public override CultureInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var cultureName = reader.GetString();

        if (string.IsNullOrWhiteSpace(cultureName))
            return null;

        try
        {
            return new CultureInfo(cultureName);
        }
        catch (CultureNotFoundException ex)
        {
            throw new JsonException(string.Empty, ex);
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, CultureInfo? value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Name);
    }
    #endregion
}