using Paradise.DataAccess.Seed.Models.ApplicationLogic;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain;
using Paradise.DataAccess.Seed.Providers.Implementation;
using System.Text.Json;

namespace Paradise.DataAccess.Seed.Tests.Unit.Providers.Implementation;

public sealed partial class JsonSeedDataProviderTests : IDisposable
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
    /// Provides setup and behavior check methods for the <see cref="JsonSeedDataProviderTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            SeedDataDirectory = Directory.CreateTempSubdirectory();

            PrepareTemporarySeedFiles();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Prepopulated infrastructure data.
        /// </summary>
        public InfrastructureDataSeedModel? PrepopulatedInfrastructureData { get; private set; }

        /// <summary>
        /// Prepopulated domain data.
        /// </summary>
        public DomainDataSeedModel? PrepopulatedDomainData { get; private set; }

        /// <summary>
        /// Path to a directory containing the seed data to read.
        /// </summary>
        public DirectoryInfo SeedDataDirectory { get; }
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public void Dispose()
            => SeedDataDirectory.Delete(true);

        /// <summary>
        /// Overwrites the prepopulated application seed data
        /// with the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">
        /// The new <see cref="InfrastructureDataSeedModel"/>.
        /// </param>
        public void OverwriteApplicationData(InfrastructureDataSeedModel? model)
        {
            SerializeToFile(model, SeedDataDirectory.FullName, JsonSeedDataProvider.InfrastructureDataFileName);

            PrepopulatedInfrastructureData = model;
        }

        /// <summary>
        /// Overwrites the prepopulated domain seed data
        /// with the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">
        /// The new <see cref="DomainDataSeedModel"/>.
        /// </param>
        public void OverwriteDomainData(DomainDataSeedModel? model)
        {
            SerializeToFile(model, SeedDataDirectory.FullName, JsonSeedDataProvider.DomainDataFileName);

            PrepopulatedDomainData = model;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Creates a default <see cref="InfrastructureDataSeedModel"/> containing a single
        /// <see cref="SeedEmailTemplateModel"/> used for testing.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="InfrastructureDataSeedModel"/> pre-populated with one email template.
        /// </returns>
        private static InfrastructureDataSeedModel GetDefaultApplicationSeedModel()
        {
            var emailTemplate = new SeedEmailTemplateModel(
                templateName: "TemplateName",
                cultureId: null,
                subject: "Subject",
                isBodyHtml: false,
                placeholderName: null,
                placeholdersNumber: 0,
                subjectPlaceholderName: null,
                subjectPlaceholdersNumber: 0,
                templateText: "TemplateText",
                templateTextSourcePath: null);

            var role = new SeedRoleModel(
                name: "Name",
                isDefault: false);

            var user = new SeedUserModel(
                emailAddress: "EmailAddress",
                userName: "UserName",
                password: "Password",
                isEmailConfirmed: false,
                roles: []);

            return new([emailTemplate], [role], [user]);
        }

        /// <summary>
        /// Creates a default <see cref="DomainDataSeedModel"/> containing a single
        /// <see cref="SeedRoleModel"/> and a single <see cref="SeedUserModel"/> used for testing.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="DomainDataSeedModel"/> with one role and one user.
        /// </returns>
        private static DomainDataSeedModel GetDefaultDomainSeedModel()
            => new();

        /// <summary>
        /// Creates temporary JSON seed data files for application and domain data,
        /// and returns the corresponding in-memory model instances.
        /// </summary>
        /// <remarks>
        /// This method generates minimal test data containing a single email template, role, and user.
        /// The data is serialized to JSON and written to the temporary directory associated with
        /// <see cref="SeedDataDirectory"/> using the filenames defined in
        /// <see cref="JsonSeedDataProvider.InfrastructureDataFileName"/> and
        /// <see cref="JsonSeedDataProvider.DomainDataFileName"/>.
        /// </remarks>
        private void PrepareTemporarySeedFiles()
        {
            PrepopulatedInfrastructureData = GetDefaultApplicationSeedModel();
            PrepopulatedDomainData = GetDefaultDomainSeedModel();

            SerializeToFile(PrepopulatedInfrastructureData, SeedDataDirectory.FullName, JsonSeedDataProvider.InfrastructureDataFileName);
            SerializeToFile(PrepopulatedDomainData, SeedDataDirectory.FullName, JsonSeedDataProvider.DomainDataFileName);
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
        #endregion
    }
    #endregion
}