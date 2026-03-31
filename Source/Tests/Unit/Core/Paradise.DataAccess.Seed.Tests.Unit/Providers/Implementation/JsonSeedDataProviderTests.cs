using Paradise.DataAccess.Seed.Models.ApplicationLogic;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Users;
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
        = new(roles: [_seedRole], users: [_seedUser]);

    private static readonly ApplicationDataSeedModel _applicationSeedModel
        = new(emailTemplates: [_seedEmailTemplate]);
    #endregion

    #region Properties
    /// <summary>
    /// Provides member data for <see cref="Constructor_ThrowsOnInvalidData"/> method.
    /// </summary>
    public static TheoryData<ApplicationDataSeedModel?, DomainDataSeedModel?> Constructor_ThrowsOnInvalidData_MemberData { get; } = new()
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
    /// A nullable <see cref="ApplicationDataSeedModel"/> instance (<see langword="null"/> is invalid).
    /// </param>
    /// <param name="domainData">
    /// A nullable <see cref="DomainDataSeedModel"/> instance (<see langword="null"/> is invalid).
    /// </param>
    [Theory, MemberData(nameof(Constructor_ThrowsOnInvalidData_MemberData))]
    public void Constructor_ThrowsOnInvalidData(ApplicationDataSeedModel? applicationData, DomainDataSeedModel? domainData)
    {
        // Arrange
        Test.OverwriteApplicationData(applicationData);
        Test.OverwriteDomainData(domainData);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => new JsonSeedDataProvider(Test.SeedDataDirectory.FullName));
    }

    /// <summary>
    /// The <see cref="JsonSeedDataProvider.GetSeedEmailTemplates"/> method should
    /// return the <see cref="IEnumerable{T}"/> of <see cref="SeedEmailTemplateModel"/>
    /// that represents the JSON data targeted by it's instance.
    /// </summary>
    [Fact]
    public void GetSeedEmailTemplates()
    {
        // Arrange
        var provider = new JsonSeedDataProvider(Test.SeedDataDirectory.FullName);

        // Act
        var seedEmailTemplates = provider.GetSeedEmailTemplates();

        // Assert
        Assert.Equivalent(Test.PrepopulatedApplicationData?.EmailTemplates, seedEmailTemplates, true);
    }

    /// <summary>
    /// The <see cref="JsonSeedDataProvider.GetSeedRoles"/> method should
    /// return the <see cref="IEnumerable{T}"/> of <see cref="SeedRoleModel"/>
    /// that represents the JSON data targeted by it's instance.
    /// </summary>
    [Fact]
    public void GetSeedRoles()
    {
        // Arrange
        var provider = new JsonSeedDataProvider(Test.SeedDataDirectory.FullName);

        // Act
        var seedRoles = provider.GetSeedRoles();

        //Assert
        Assert.Equivalent(Test.PrepopulatedDomainData?.Roles, seedRoles, true);
    }

    /// <summary>
    /// The <see cref="JsonSeedDataProvider.GetSeedUsers"/> method should
    /// return the <see cref="IEnumerable{T}"/> of <see cref="SeedUserModel"/>
    /// that represents the JSON data targeted by it's instance.
    /// </summary>
    [Fact]
    public void GetSeedUsers()
    {
        // Arrange
        var provider = new JsonSeedDataProvider(Test.SeedDataDirectory.FullName);

        // Act
        var seedUsers = provider.GetSeedUsers();

        // Assert
        Assert.Equivalent(Test.PrepopulatedDomainData?.Users, seedUsers, true);
    }
    #endregion
}