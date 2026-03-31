using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Paradise.Common.Tests.Unit.Extensions;

public sealed partial class IConfigurationExtensionsTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="IConfigurationExtensionsTests"/> class.
    /// </summary>
    private sealed class TestEnvironment
    {
        #region Properties
        /// <summary>
        /// A <see cref="BindingTarget"/> instance used to build
        /// the <see cref="IConfiguration"/> via the <see cref="GetConfiguration"/> method.
        /// </summary>
        public BindingTarget? ConfigurationInstance { get; set; }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates an <see cref="IConfiguration"/> instance containing a JSON
        /// representation of the current <see cref="ConfigurationInstance"/>.
        /// </summary>
        /// <param name="mimicUnknownConfiguration">
        /// When <see langword="true"/>, the returned configuration places the
        /// <see cref="ConfigurationInstance"/> under a section named
        /// <c>UnknownConfiguration</c>.
        /// When <see langword="false"/>, the configuration places it under
        /// the <c>BindingTarget</c> section, which matches the expected section
        /// name for successful binding in tests.
        /// </param>
        /// <returns>
        /// A fully built <see cref="IConfiguration"/> whose data originates
        /// from a JSON stream representing the selected configuration structure.
        /// </returns>
        public IConfiguration GetConfiguration(bool mimicUnknownConfiguration = false)
        {
            var configurationKey = mimicUnknownConfiguration
                ? "UnknownConfiguration"
                : nameof(BindingTarget);

            var configurationValues = new Dictionary<string, object?>
            {
                [configurationKey] = ConfigurationInstance
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

    /// <summary>
    /// A test-helper class that eases writing and isolates test methods.
    /// </summary>
    private sealed class BindingTarget
    {
        #region Properties
        /// <summary>
        /// Test string property.
        /// </summary>
        public string? StringValue { get; set; }

        /// <summary>
        /// Test integer property.
        /// </summary>
        public int IntegerValue { get; set; }

        /// <summary>
        /// Test complex property.
        /// </summary>
        public BindingTarget? Child { get; set; }
        #endregion
    }
    #endregion
}