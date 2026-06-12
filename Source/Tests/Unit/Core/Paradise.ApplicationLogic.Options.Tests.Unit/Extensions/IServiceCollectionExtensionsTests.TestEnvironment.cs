using Microsoft.Extensions.Configuration;
using Paradise.Tests.Fixtures.Common.Models;
using System.Text.Json;

namespace Paradise.ApplicationLogic.Options.Tests.Unit.Extensions;

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
        #region Properties
        /// <summary>
        /// An options instance used to build initial configuration,
        /// and to compare test resulting options instances against.
        /// </summary>
        public TestModel Options { get; } = new();
        #endregion

        #region Public methods
        /// <summary>
        /// Builds the <see cref="IConfiguration"/> instance representing
        /// the current <see cref="Options"/> instance.
        /// </summary>
        /// <param name="configurationSectionPath">
        /// A semicolon ':' delimited string representing
        /// the sections path to the target options section.
        /// </param>
        /// <returns>
        /// The <see cref="IConfiguration"/> representation of the <see cref="Options"/>.
        /// </returns>
        public IConfiguration BuildConfiguration(string? configurationSectionPath = null)
        {
            var configurationKey = configurationSectionPath is null
                ? nameof(TestModel)
                : $"{configurationSectionPath}:{nameof(TestModel)}";

            var configurationValues = new Dictionary<string, object>()
            {
                [configurationKey] = Options
            };

            using var configurationStream = new MemoryStream();
            JsonSerializer.Serialize(configurationStream, configurationValues);

            configurationStream.Position = 0;

            return new ConfigurationBuilder()
                .AddJsonStream(configurationStream)
                .Build();
        }
        #endregion
    }
    #endregion
}