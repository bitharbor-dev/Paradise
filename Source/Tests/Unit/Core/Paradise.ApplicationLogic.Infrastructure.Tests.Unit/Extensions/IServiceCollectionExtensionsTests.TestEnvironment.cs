using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Infrastructure.Extensions;
using Paradise.ApplicationLogic.Options.Models.DataAccess.Seed.Providers;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.DataAccess.Seed.Models.ApplicationLogic;
using Paradise.DataAccess.Seed.Models.Domain;
using Paradise.DataAccess.Seed.Providers.Implementation;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Extensions;

public sealed partial class IServiceCollectionExtensionsTests : IDisposable
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => Test.Dispose();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="IServiceCollectionExtensionsTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private DirectoryInfo? _seedDirectory;
        private readonly ConcurrentBag<string> _temporaryDirectories = [];
        #endregion

        #region Properties
        /// <summary>
        /// Application configuration which helps resolving the registered services.
        /// </summary>
        public OptionsContainer Options { get; } = new()
        {
            SmtpOptions = new()
            {
                Credentials = new("UserName", "endpoint=https://test.com/;accesskey=1234"),
                EnableSecureSocketsLayer = false,
                Host = "smtp.test",
                Port = 2525
            },
            IdentityOptions = new(),
            JsonSeedDataProviderOptions = new()
        };
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public void Dispose()
        {
            _seedDirectory?.Delete(true);

            foreach (var path in _temporaryDirectories)
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// Gets a temporary directory path, if a directory will be created
        /// by the returned path - it will be automatically deleted after the test run.
        /// </summary>
        /// <returns>
        /// A path to the temporary sub-folder in the current user's temporary folder.
        /// </returns>
        public string GetTemporaryDirectoryPath()
        {
            var tempPath = Path.GetTempPath();
            var directoryName = Guid.CreateVersion7().ToString("N");

            var fullPath = Path.Combine(tempPath, directoryName);

            _temporaryDirectories.Add(fullPath);

            return fullPath;
        }

        /// <summary>
        /// Builds a service provider using the current <see cref="Options"/> and the
        /// <see cref="IServiceCollectionExtensions.AddInfrastructure"/> registration method.
        /// </summary>
        /// <param name="environmentName">
        /// Current environment name.
        /// </param>
        /// <returns>
        /// A configured <see cref="IServiceProvider"/>.
        /// </returns>
        [SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance",
            Justification = "Intentional encapsulation.")]
        public IServiceProvider BuildInfrastructureServiceProvider(string environmentName)
        {
            PrepareTemporarySeedFiles();

            var configuration = BuildConfiguration();

            var services = new ServiceCollection()
                .AddInfrastructure(configuration, environmentName);

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Creates temporary JSON seed data files for application and domain data,
        /// and returns the corresponding in-memory model instances.
        /// </summary>
        /// <remarks>
        /// This method generates minimal test data containing a single email template, role, and user.
        /// The data is serialized to JSON and written to the temporary directory associated with
        /// <see cref="_seedDirectory"/> using the filenames defined in
        /// <see cref="JsonSeedDataProvider.ApplicationDataFileName"/> and
        /// <see cref="JsonSeedDataProvider.DomainDataFileName"/>.
        /// </remarks>
        private void PrepareTemporarySeedFiles()
        {
            _seedDirectory = Directory.CreateTempSubdirectory();
            Options.JsonSeedDataProviderOptions!.SeedDirectoryPath = _seedDirectory.FullName;

            SerializeToFile(new ApplicationDataSeedModel([]), _seedDirectory.FullName, JsonSeedDataProvider.ApplicationDataFileName);
            SerializeToFile(new DomainDataSeedModel([], []), _seedDirectory.FullName, JsonSeedDataProvider.DomainDataFileName);
        }

        /// <summary>
        /// Serializes the specified <paramref name="data"/> to the file.
        /// </summary>
        /// <param name="data">
        /// The data to serialize.
        /// </param>
        /// <param name="directory">
        /// File path (without file name).
        /// </param>
        /// <param name="fileName">
        /// The file name (without path) to create inside the <paramref name="directory"/>.
        /// </param>
        private static void SerializeToFile(object? data, string directory, string fileName)
        {
            var path = Path.Combine(directory, fileName);
            using var file = File.Create(path);

            JsonSerializer.Serialize(file, data);
        }

        /// <summary>
        /// Builds the <see cref="IConfiguration"/> instance representing
        /// the current <see cref="Options"/> instance.
        /// </summary>
        /// <returns>
        /// The <see cref="IConfiguration"/> representation of the <see cref="Options"/>.
        /// </returns>
        private IConfiguration BuildConfiguration()
        {
            using var configurationStream = new MemoryStream();
            JsonSerializer.Serialize(configurationStream, Options);

            configurationStream.Position = 0;

            return new ConfigurationBuilder()
                .AddJsonStream(configurationStream)
                .Build();
        }
        #endregion
    }

    /// <summary>
    /// Replicates the application options data structure for proper
    /// conversion into <see cref="IConfiguration"/> instances.
    /// </summary>
    private sealed class OptionsContainer
    {
        #region Properties
        /// <summary>
        /// Configurable SMTP options instance.
        /// </summary>
        public SmtpOptions? SmtpOptions { get; set; }

        /// <summary>
        /// Configurable Identity options instance.
        /// </summary>
        public IdentityOptions? IdentityOptions { get; set; }

        /// <summary>
        /// Configurable JSON seed data provider options instance.
        /// </summary>
        public JsonSeedDataProviderOptions? JsonSeedDataProviderOptions { get; set; }
        #endregion
    }
    #endregion
}