using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi;
using Paradise.WebApi.OpenApi.DocumentTransformers;
using System.Text.Json;

namespace Paradise.WebApi.Tests.Unit.OpenApi.DocumentTransformers;

public sealed partial class DocumentSecuritySchemeSetterTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="DocumentSecuritySchemeSetterTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Properties
        /// <summary>
        /// The <see cref="OpenApiSecurityScheme"/> instance passed
        /// into transformer configuration.
        /// </summary>
        public OpenApiSecurityScheme SecurityScheme { get; } = new()
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Scheme = "bearer",
            Type = SecuritySchemeType.Http
        };
        #endregion

        #region Public methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSecuritySchemeSetter"/> class.
        /// </summary>
        /// <returns>
        /// System under test.
        /// </returns>
        public DocumentSecuritySchemeSetter CreateTransformer()
        {
            var configuration = BuildConfiguration();

            return new(configuration);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Builds the <see cref="IConfiguration"/> instance containing the transformer configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="IConfiguration"/> to be used to configure the target transformer.
        /// </returns>
        private IConfiguration BuildConfiguration()
        {
            using var configurationStream = new MemoryStream();
            JsonSerializer.Serialize(configurationStream, new
            {
                OpenApiSecurityScheme = SecurityScheme
            });

            configurationStream.Position = 0;

            return new ConfigurationBuilder()
                .AddJsonStream(configurationStream)
                .Build();
        }
        #endregion
    }
    #endregion
}