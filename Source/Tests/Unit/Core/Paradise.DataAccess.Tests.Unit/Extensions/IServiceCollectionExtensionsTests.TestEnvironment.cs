using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paradise.DataAccess.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Paradise.DataAccess.Tests.Unit.Extensions;

public sealed partial class IServiceCollectionExtensionsTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="IServiceCollectionExtensionsTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            using var configurationStream = new MemoryStream();
            JsonSerializer.Serialize(configurationStream, new
            {
                ConnectionStrings = new
                {
                    DomainConnectionString = "",
                    InfrastructureConnectionString = ""
                }
            });

            configurationStream.Position = 0;

            Configuration = new ConfigurationBuilder()
                .AddJsonStream(configurationStream)
                .Build();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Application configuration to resolve the registered services.
        /// </summary>
        public IConfiguration Configuration { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Builds a service provider using the current <see cref="Configuration"/> and the
        /// <see cref="IServiceCollectionExtensions.AddDataAccess"/> registration method,
        /// replacing certain registrations with fakes where necessary for testability.
        /// </summary>
        /// <returns>
        /// A configured <see cref="IServiceProvider"/>.
        /// </returns>
        [SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance",
            Justification = "Intentional encapsulation.")]
        public IServiceProvider BuildDataAccessServiceProvider()
        {
            var services = new ServiceCollection()
                .AddDataAccess(Configuration);

            return services.BuildServiceProvider(new ServiceProviderOptions()
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        }
        #endregion
    }
    #endregion
}