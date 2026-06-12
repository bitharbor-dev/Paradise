using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi;
using Paradise.WebApi.OpenApi.OperationTransformers;
using System.Text.Json;

namespace Paradise.WebApi.Tests.Unit.OpenApi.OperationTransformers;

public sealed partial class OperationSecuritySchemeSetterTests
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
    /// Provides setup and behavior-check methods for the <see cref="OperationSecuritySchemeSetterTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private readonly AllowAnonymousAttribute _allowAnonymous;
        private readonly OpenApiSecurityScheme _sampleScheme;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _allowAnonymous = new();

            _sampleScheme = new()
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Scheme = "bearer",
                Type = SecuritySchemeType.Http
            };

            var configuration = BuildConfiguration();

            Target = new(configuration);
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public OperationSecuritySchemeSetter Target { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets a copy of a <see cref="OpenApiSecurityScheme"/> used to configure
        /// the target transformer.
        /// </summary>
        /// <returns>
        /// A new <see cref="OpenApiSecurityScheme"/> instance.
        /// </returns>
        public OpenApiSecurityScheme GetConfiguredSecurityScheme() => new()
        {
            In = _sampleScheme.In,
            Name = _sampleScheme.Name,
            Scheme = _sampleScheme.Scheme,
            Type = _sampleScheme.Type
        };

        /// <summary>
        /// Creates a new instance of the <see cref="OpenApiOperationTransformerContext"/> class
        /// with its action descriptor metadata set to contain <see cref="AllowAnonymousAttribute"/>
        /// depending on the <paramref name="allowAnonymous"/> value.
        /// </summary>
        /// <param name="allowAnonymous">
        /// Indicates whether the output context targets the endpoint allowing anonymous requests.
        /// </param>
        /// <returns>
        /// A new <see cref="OpenApiOperationTransformerContext"/> instance with configured action descriptor.
        /// </returns>
        public OpenApiOperationTransformerContext CreateContext(bool allowAnonymous = false) => new()
        {
            ApplicationServices = null!,
            Description = new()
            {
                ActionDescriptor = new()
                {
                    EndpointMetadata = allowAnonymous ? [_allowAnonymous] : []
                }
            },
            DocumentName = string.Empty
        };
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
                OpenApiSecurityScheme = _sampleScheme
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