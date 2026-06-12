using System.Text.Json.Serialization;

namespace Paradise.Tests.Extensibility.Json.Converters;

[JsonSerializable(typeof(JwtBearerOptionsConverter.JwtBearerOptionsModel))]
internal sealed partial class InternalJsonSerializerContext : JsonSerializerContext;