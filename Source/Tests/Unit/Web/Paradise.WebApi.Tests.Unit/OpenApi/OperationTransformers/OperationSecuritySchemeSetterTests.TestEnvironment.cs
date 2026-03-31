using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi;
using Paradise.Tests.Miscellaneous.Reflection;
using Paradise.WebApi.OpenApi.OperationTransformers;
using System.Reflection;
using System.Reflection.Emit;
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
    /// Provides setup and behavior check methods for the <see cref="OperationSecuritySchemeSetterTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Fields
        private readonly CustomAttributeBuilder _allowAnonymous;
        private readonly OpenApiSecurityScheme _sampleScheme;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _allowAnonymous = new CustomAttributeBuilder(
                typeof(AllowAnonymousAttribute).GetConstructor(Type.EmptyTypes)!, []);

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
        /// Creates a new instance of <see cref="OpenApiOperationTransformerContext"/> class
        /// with its action descriptor method set to
        /// dynamically generated <see cref="MethodInfo"/> instance.
        /// </summary>
        /// <param name="allowAnonymousOnMethod">
        /// Indicates whether <see cref="ControllerActionDescriptor.MethodInfo"/>
        /// should be decorated with <see cref="AllowAnonymousAttribute"/>.
        /// </param>
        /// <param name="allowAnonymousOnType">
        /// Indicates whether <see cref="ControllerActionDescriptor.MethodInfo"/> declaring type
        /// should be decorated with <see cref="AllowAnonymousAttribute"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="OpenApiOperationTransformerContext"/> instance with configured method information.
        /// </returns>
        public OpenApiOperationTransformerContext CreateContext(bool allowAnonymousOnMethod = false,
                                                                bool allowAnonymousOnType = false)
        {
            var typeAttributes = allowAnonymousOnType
                ? new[] { _allowAnonymous }
                : [];

            var methodAttributes = allowAnonymousOnMethod
                ? new[] { _allowAnonymous }
                : [];

            var methodInfo = MethodInfoFactory.CreateVoid(
                Guid.NewGuid().ToString("N"), "Test", typeAttributes, methodAttributes);

            return new()
            {
                ApplicationServices = null!,
                Description = new()
                {
                    ActionDescriptor = new ControllerActionDescriptor()
                    {
                        MethodInfo = methodInfo
                    }
                },
                DocumentName = string.Empty
            };
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