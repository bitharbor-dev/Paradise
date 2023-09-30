namespace Paradise.DataAccess.Seed.Tests.Providers;

/// <summary>
/// Test class for the <see cref="JsonSeedDataProvider"/> methods.
/// </summary>
public sealed class JsonSeedDataProviderTests
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSeedDataProviderTests"/> class.
    /// </summary>
    public JsonSeedDataProviderTests()
    {
        using var applicationDataStream = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(new ApplicationDataSeedModel
        {
            EmailTemplates = [new("", "", "")]
        }));

        using var domainDataStream = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(new DomainDataSeedModel
        {
            Roles = [new("")],
            Users = [new("", "", "")]
        }));

        Provider = new(null, applicationDataStream, domainDataStream);
    }
    #endregion

    #region Properties
    /// <summary>
    /// A <see cref="JsonSeedDataProvider"/> instance to be tested.
    /// </summary>
    public JsonSeedDataProvider Provider { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="JsonSeedDataProvider.GetSeedEmailTemplates"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a list of email templates to be seeded into the database.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetSeedEmailTemplates()
    {
        // Arrange

        // Act
        var seedEmailTemplates = Provider.GetSeedEmailTemplates();

        // Assert
        Assert.NotEmpty(seedEmailTemplates);
    }

    /// <summary>
    /// <see cref="JsonSeedDataProvider.GetSeedRoles"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a list of roles to be seeded into the database.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetSeedRoles()
    {
        // Arrange

        // Act
        var seedRoles = Provider.GetSeedRoles();

        //Assert
        Assert.NotEmpty(seedRoles);
    }

    /// <summary>
    /// <see cref="JsonSeedDataProvider.GetSeedUsers"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns a list of users to be seeded into the database.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetSeedUsers()
    {
        // Arrange

        // Act
        var seedUsers = Provider.GetSeedUsers();

        // Assert
        Assert.NotEmpty(seedUsers);
    }
    #endregion
}