using Paradise.DataAccess.Seed.Models.ApplicationLogic;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain;
using Paradise.DataAccess.Seed.Providers.Implementation;

namespace Paradise.DataAccess.Seed.Tests.Unit.Providers.Implementation;

/// <summary>
/// <see cref="JsonSeedDataProvider"/> test class..
/// </summary>
public sealed partial class JsonSeedDataProviderTests
{
    #region Fields
    private static readonly SeedRoleModel _seedRole
        = new("Name", false);

    private static readonly SeedUserModel _seedUser
        = new("EmailAddress", "UserName", "Password", false, []);

    private static readonly SeedEmailTemplateModel _seedEmailTemplate
        = new("TemplateName", null, "Subject", false, null, 0, null, 0, "TemplateText");

    private static readonly DomainDataSeedModel _domainSeedModel
        = new();

    private static readonly InfrastructureDataSeedModel _applicationSeedModel
        = new(emailTemplates: [_seedEmailTemplate], roles: [_seedRole], users: [_seedUser]);
    #endregion

    #region Properties
    /// <summary>
    /// Provides member data for <see cref="Constructor_ThrowsOnInvalidData"/> method.
    /// </summary>
    public static TheoryData<InfrastructureDataSeedModel?, DomainDataSeedModel?> Constructor_ThrowsOnInvalidData_MemberData { get; } = new()
    {
        { null,                     _domainSeedModel    },
        { _applicationSeedModel,    null                }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="JsonSeedDataProvider"/> constructor should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// seed data JSON file contains invalid JSON.
    /// </summary>
    /// <param name="applicationData">
    /// A nullable <see cref="InfrastructureDataSeedModel"/> instance (<see langword="null"/> is invalid).
    /// </param>
    /// <param name="domainData">
    /// A nullable <see cref="DomainDataSeedModel"/> instance (<see langword="null"/> is invalid).
    /// </param>
    [Theory, MemberData(nameof(Constructor_ThrowsOnInvalidData_MemberData))]
    public void Constructor_ThrowsOnInvalidData(InfrastructureDataSeedModel? applicationData, DomainDataSeedModel? domainData)
    {
        // Arrange
        Test.OverwriteApplicationData(applicationData);
        Test.OverwriteDomainData(domainData);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => new JsonSeedDataProvider(Test.SeedDataDirectory.FullName));
    }

    /// <summary>
    /// The <see cref="JsonSeedDataProvider.DomainData"/> property should
    /// return the <see cref="DomainDataSeedModel"/> populated with the JSON data.
    /// </summary>
    [Fact]
    public void DomainData()
    {
        // Arrange
        var provider = new JsonSeedDataProvider(Test.SeedDataDirectory.FullName);

        // Act
        var data = provider.DomainData;

        // Assert
        Assert.Equivalent(Test.PrepopulatedDomainData, data);
    }

    /// <summary>
    /// The <see cref="JsonSeedDataProvider.InfrastructureData"/> property should
    /// return the <see cref="InfrastructureDataSeedModel"/> populated with the JSON data.
    /// </summary>
    [Fact]
    public void InfrastructureData()
    {
        // Arrange
        var provider = new JsonSeedDataProvider(Test.SeedDataDirectory.FullName);

        // Act
        var data = provider.InfrastructureData;

        // Assert
        Assert.Equivalent(Test.PrepopulatedInfrastructureData, data);
    }
    #endregion
}