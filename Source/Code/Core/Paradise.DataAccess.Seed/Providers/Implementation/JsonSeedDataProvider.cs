using Paradise.DataAccess.Seed.Models.ApplicationLogic;
using Paradise.DataAccess.Seed.Models.Domain;
using Paradise.Localization.ExceptionHandling;
using Paradise.Primitives.Extensions;
using System.Text.Json;

namespace Paradise.DataAccess.Seed.Providers.Implementation;

/// <summary>
/// Reads the database seed data from the JSON file(s).
/// </summary>
public sealed class JsonSeedDataProvider : ISeedDataProvider
{
    #region Constants
    /// <summary>
    /// Default JSON file's directory name.
    /// </summary>
    public const string DefaultSeedFolder = "Data\\JSON";

    /// <summary>
    /// JSON file name to read the domain data from.
    /// </summary>
    public const string DomainDataFileName = "DomainData.json";

    /// <summary>
    /// JSON file name to read the infrastructure data from.
    /// </summary>
    public const string InfrastructureDataFileName = "InfrastructureData.json";
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSeedDataProvider"/> class.
    /// </summary>
    /// <param name="path">
    /// Seed data files directory path.
    /// </param>
    public JsonSeedDataProvider(string? path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var sanitizedPath = path.SanitizePathSeparators();

        var domainFilePath = Path.Combine(sanitizedPath, DomainDataFileName);
        var infrastructureFilePath = Path.Combine(sanitizedPath, InfrastructureDataFileName);

        DomainData = ReadFromJsonFile<DomainDataSeedModel>(domainFilePath);
        InfrastructureData = ReadFromJsonFile<InfrastructureDataSeedModel>(infrastructureFilePath);
    }
    #endregion

    #region Properties
    /// <inheritdoc/>
    public DomainDataSeedModel DomainData { get; }

    /// <inheritdoc/>
    public InfrastructureDataSeedModel InfrastructureData { get; }
    #endregion

    #region Private methods
    /// <summary>
    /// Reads and deserializes JSON data from the specified file
    /// into an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The target type to deserialize the JSON content into.
    /// </typeparam>
    /// <param name="path">
    /// The path to the JSON file.
    /// </param>
    /// <returns>
    /// An instance of <typeparamref name="T"/> populated with data from the JSON file.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the JSON content cannot be deserialized into <typeparamref name="T"/>.
    /// </exception>
    private static T ReadFromJsonFile<T>(string path)
    {
        using var stream = File.OpenRead(path);
        var data = JsonSerializer.Deserialize<T>(stream);

        if (data is null)
        {
            var message = ExceptionMessagesProvider.GetMessageFailedToDeserialize<T>();

            throw new InvalidOperationException(message);
        }

        return data;
    }
    #endregion
}