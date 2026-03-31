namespace Paradise.DataAccess.Seed.Tests.Unit.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

public sealed partial class SeedEmailTemplateModelTests : IDisposable
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
    /// Provides setup and behavior check methods for the <see cref="SeedEmailTemplateModelTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly DirectoryInfo _tempDirectory;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SeedEmailTemplateModelTests"/> class.
        /// </summary>
        public TestEnvironment()
            => _tempDirectory = Directory.CreateTempSubdirectory();
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public void Dispose()
            => _tempDirectory.Delete(true);

        /// <summary>
        /// Writes the specified content to a temporary file within the test's temporary directory.
        /// </summary>
        /// <param name="content">
        /// The text content to write to the temporary file.
        /// </param>
        /// <returns>
        /// The full path of the temporary file that was created.
        /// </returns>
        /// <remarks>
        /// The file name is derived from the current test's unique ID.
        /// </remarks>
        public string WriteTemporaryFile(string content)
        {
            var path = Path.Combine(_tempDirectory.FullName, TestContext.Current.Test!.UniqueID);
            File.WriteAllText(path, content);

            return path;
        }
        #endregion
    }
    #endregion
}