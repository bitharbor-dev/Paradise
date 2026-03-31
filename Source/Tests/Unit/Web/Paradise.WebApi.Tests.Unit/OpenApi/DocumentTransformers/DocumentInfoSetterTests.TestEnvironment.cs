using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi;
using Paradise.WebApi.OpenApi.DocumentTransformers;
using System.Text.Json;

namespace Paradise.WebApi.Tests.Unit.OpenApi.DocumentTransformers;

public sealed partial class DocumentInfoSetterTests
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
    /// Provides setup and behavior check methods for the <see cref="DocumentInfoSetterTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Properties
        /// <summary>
        /// The <see cref="OpenApiInfo"/> instance passed
        /// into transformer configuration.
        /// </summary>
        public OpenApiInfo Info { get; } = new()
        {
            Contact = new()
            {
                Email = "code@bitharbor.dev",
                Name = "Bit Harbor"
            },
            Description = "Description",
            License = new()
            {
                Identifier = "MIT",
                Name = "MIT"
            },
            Summary = "'OpenApiDocumentTransformer' unit tests.",
            Title = "Paradise"
        };
        #endregion

        #region Public methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentInfoSetter"/> class.
        /// </summary>
        /// <returns>
        /// System under test.
        /// </returns>
        public DocumentInfoSetter CreateTransformer()
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
                OpenApiInfo = Info
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