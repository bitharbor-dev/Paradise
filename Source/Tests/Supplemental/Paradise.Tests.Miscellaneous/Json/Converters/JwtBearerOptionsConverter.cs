using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Paradise.Tests.Miscellaneous.Json.Converters;

/// <summary>
/// <see cref="JwtBearerOptions"/> JSON converter.
/// </summary>
/// <remarks>
/// This is a custom converter because
/// <see cref="JwtBearerOptions"/> contains delegates
/// and other non-serializable members,
/// so we need to explicitly ignore them and only
/// serialize/deserialize the relevant properties.
/// </remarks>
public sealed class JwtBearerOptionsConverter : JsonConverter<JwtBearerOptions?>
{
    #region Public methods
    /// <inheritdoc/>
    public override JwtBearerOptions? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Null)
            return null;

        var model = JsonSerializer.Deserialize<JwtBearerOptionsModel>(ref reader, options)
            ?? throw new JsonException();

        return model.GetOptions();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, JwtBearerOptions? value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var model = new JwtBearerOptionsModel(value);

        JsonSerializer.Serialize(writer, model, options);
    }
    #endregion

    #region Nested types
    /// <summary>
    /// Represents a serializable subset of <see cref="JwtBearerOptions"/>.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="JwtBearerOptionsModel"/> class.
    /// </remarks>
    /// <param name="authority">
    /// <inheritdoc cref="JwtBearerOptions.Authority"/>
    /// </param>
    /// <param name="audience">
    /// <inheritdoc cref="JwtBearerOptions.Audience"/>
    /// </param>
    /// <param name="requireHttpsMetadata">
    /// <inheritdoc cref="JwtBearerOptions.RequireHttpsMetadata"/>
    /// </param>
    /// <param name="saveToken">
    /// <inheritdoc cref="JwtBearerOptions.SaveToken"/>
    /// </param>
    /// <param name="tokenValidationParameters">
    /// <inheritdoc cref="JwtBearerOptions.TokenValidationParameters"/>
    /// </param>
    [method: JsonConstructor]
    private sealed class JwtBearerOptionsModel(string? authority,
                                               string? audience,
                                               bool requireHttpsMetadata,
                                               bool saveToken,
                                               TokenValidationParameters tokenValidationParameters)
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtBearerOptionsModel"/> class.
        /// </summary>
        /// <param name="options">
        /// The source <see cref="JwtBearerOptions"/> instance.
        /// </param>
        public JwtBearerOptionsModel(JwtBearerOptions options) : this(options.Authority,
                                                                      options.Audience,
                                                                      options.RequireHttpsMetadata,
                                                                      options.SaveToken,
                                                                      options.TokenValidationParameters)
        { }
        #endregion

        #region Properties
        ///<inheritdoc cref="JwtBearerOptions.Authority"/>
        [JsonPropertyName(nameof(JwtBearerOptions.Authority))]
        public string? Authority { get; set; } = authority;

        ///<inheritdoc cref="JwtBearerOptions.Audience"/>
        [JsonPropertyName(nameof(JwtBearerOptions.Audience))]
        public string? Audience { get; set; } = audience;

        ///<inheritdoc cref="JwtBearerOptions.RequireHttpsMetadata"/>
        [JsonPropertyName(nameof(JwtBearerOptions.RequireHttpsMetadata))]
        public bool RequireHttpsMetadata { get; set; } = requireHttpsMetadata;

        ///<inheritdoc cref="JwtBearerOptions.SaveToken"/>
        [JsonPropertyName(nameof(JwtBearerOptions.SaveToken))]
        public bool SaveToken { get; set; } = saveToken;

        ///<inheritdoc cref="JwtBearerOptions.TokenValidationParameters"/>
        [JsonPropertyName(nameof(JwtBearerOptions.TokenValidationParameters))]
        public TokenValidationParameters TokenValidationParameters { get; set; } = tokenValidationParameters;
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a <see cref="JwtBearerOptions"/> instance from the current model.
        /// </summary>
        /// <returns>
        /// A configured <see cref="JwtBearerOptions"/> instance.
        /// </returns>
        public JwtBearerOptions GetOptions() => new()
        {
            Authority = Authority,
            Audience = Audience,
            RequireHttpsMetadata = RequireHttpsMetadata,
            SaveToken = SaveToken,
            TokenValidationParameters = TokenValidationParameters
        };
        #endregion
    }
    #endregion
}