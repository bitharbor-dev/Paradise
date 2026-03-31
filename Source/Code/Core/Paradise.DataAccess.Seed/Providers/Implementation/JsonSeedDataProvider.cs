using Paradise.Common.Extensions;
using Paradise.DataAccess.Seed.Models.ApplicationLogic;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Users;
using Paradise.Localization.ExceptionHandling;
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
    /// JSON file name to read the application data from.
    /// </summary>
    public const string ApplicationDataFileName = "ApplicationData.json";
    #endregion

    #region Fields
    private readonly ApplicationDataSeedModel _applicationData;
    private readonly DomainDataSeedModel _domainData;
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

        var applicationFilePath = Path.Combine(sanitizedPath, ApplicationDataFileName);
        var domainFilePath = Path.Combine(sanitizedPath, DomainDataFileName);

        using var applicationFileStream = File.OpenRead(applicationFilePath);
        using var domainFileStream = File.OpenRead(domainFilePath);

        var applicationData = JsonSerializer.Deserialize<ApplicationDataSeedModel>(applicationFileStream);
        var domainData = JsonSerializer.Deserialize<DomainDataSeedModel>(domainFileStream);

        if (applicationData is null)
        {
            var message = ExceptionMessages.GetMessageFailedToDeserialize<ApplicationDataSeedModel>();

            throw new InvalidOperationException(message);
        }

        if (domainData is null)
        {
            var message = ExceptionMessages.GetMessageFailedToDeserialize<DomainDataSeedModel>();

            throw new InvalidOperationException(message);
        }

        _applicationData = applicationData;
        _domainData = domainData;
    }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public IEnumerable<SeedEmailTemplateModel> GetSeedEmailTemplates()
        => _applicationData.EmailTemplates;

    /// <inheritdoc/>
    public IEnumerable<SeedRoleModel> GetSeedRoles()
        => _domainData.Roles;

    /// <inheritdoc/>
    public IEnumerable<SeedUserModel> GetSeedUsers()
        => _domainData.Users;
    #endregion
}