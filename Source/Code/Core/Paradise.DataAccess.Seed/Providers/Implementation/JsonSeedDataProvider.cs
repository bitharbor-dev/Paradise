using Microsoft.Extensions.Options;
using Paradise.DataAccess.Seed.Models.Application;
using Paradise.DataAccess.Seed.Models.Application.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain;
using Paradise.DataAccess.Seed.Models.Domain.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Users;
using System.Text.Json;
using static System.IO.File;
using static System.IO.Path;
using static System.Text.Json.JsonSerializer;

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
    private const string DefaultSeedFolder = "SeedData\\Json";

    /// <summary>
    /// JSON file name to read the domain data from.
    /// </summary>
    private const string DomainDataFileName = "DomainData.json";

    /// <summary>
    /// JSON file name to read the application data from.
    /// </summary>
    private const string ApplicationDataFileName = "ApplicationData.json";
    #endregion

    #region Fields
    private readonly JsonSerializerOptions? _jsonSerializerOptions;
    private readonly ApplicationDataSeedModel _applicationData;
    private readonly DomainDataSeedModel _domainData;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSeedDataProvider"/> class.
    /// </summary>
    /// <param name="jsonSerializerOptions">
    /// The accessor used to access the <see cref="JsonSerializerOptions"/>.
    /// </param>
    /// <param name="applicationData">
    /// Application seed data input <see cref="Stream"/>.
    /// </param>
    /// <param name="domainData">
    /// Domain seed data input <see cref="Stream"/>.
    /// </param>
    public JsonSeedDataProvider(IOptions<JsonSerializerOptions>? jsonSerializerOptions, Stream applicationData, Stream domainData)
    {
        _jsonSerializerOptions = jsonSerializerOptions?.Value;

        _applicationData = Deserialize<ApplicationDataSeedModel>(applicationData, _jsonSerializerOptions)!;
        _domainData = Deserialize<DomainDataSeedModel>(domainData, _jsonSerializerOptions)!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSeedDataProvider"/> class.
    /// </summary>
    /// <param name="jsonSerializerOptions">
    /// The accessor used to access the <see cref="JsonSerializerOptions"/>.
    /// </param>
    /// <param name="seedFolder">
    /// Seed data files directory name.
    /// </param>
    public JsonSeedDataProvider(IOptions<JsonSerializerOptions>? jsonSerializerOptions, string seedFolder = DefaultSeedFolder)
    {
        var root = AppContext.BaseDirectory;
        var applicationFilePath = Combine(root, seedFolder.Replace('\\', DirectorySeparatorChar), ApplicationDataFileName);
        var domainFilePath = Combine(root, seedFolder.Replace('\\', DirectorySeparatorChar), DomainDataFileName);

        var applicationJson = ReadAllText(applicationFilePath);
        var domainJson = ReadAllText(domainFilePath);

        _jsonSerializerOptions = jsonSerializerOptions?.Value;

        _applicationData = Deserialize<ApplicationDataSeedModel>(applicationJson, _jsonSerializerOptions)!;
        _domainData = Deserialize<DomainDataSeedModel>(domainJson, _jsonSerializerOptions)!;
    }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IEnumerable<SeedEmailTemplateModel> GetSeedEmailTemplates()
        => _applicationData.EmailTemplates ?? Array.Empty<SeedEmailTemplateModel>();

    /// <inheritdoc/>
    public IEnumerable<SeedRoleModel> GetSeedRoles()
        => _domainData.Roles ?? Array.Empty<SeedRoleModel>();

    /// <inheritdoc/>
    public IEnumerable<SeedUserModel> GetSeedUsers()
        => _domainData.Users ?? Array.Empty<SeedUserModel>();
    #endregion
}