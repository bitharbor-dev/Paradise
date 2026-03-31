using Paradise.ApplicationLogic.DataConverters.Domain.Identity.Roles;
using Paradise.DataAccess.Seed.Models.Domain.Identity.Roles;
using Paradise.Domain.Identity.Roles;
using Paradise.Models.Domain.Identity.Roles;

namespace Paradise.ApplicationLogic.DataConverters.Tests.Unit.Domain.Identity.Roles;

/// <summary>
/// <see cref="RoleDataConverter"/> test class.
/// </summary>
public sealed class RoleDataConverterTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="RoleDataConverter.ToEntity(SeedRoleModel)"/> method should
    /// return a new <see cref="Role"/> instance populated
    /// with the data from the input <see cref="SeedRoleModel"/> object.
    /// </summary>
    [Fact]
    public void ToEntity_FromSeedModel()
    {
        // Arrange
        var model = new SeedRoleModel(
            name: "Name",
            isDefault: true);

        // Act
        var result = model.ToEntity();

        // Assert
        Assert.Equal(model.Name, result.Name);
        Assert.Equal(model.IsDefault, result.IsDefault);
    }

    /// <summary>
    /// The <see cref="RoleDataConverter.ToEntity(SeedRoleModel)"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="SeedRoleModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToEntity_FromSeedModel_ThrowsOnNull()
    {
        // Arrange
        var model = null as SeedRoleModel;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => model!.ToEntity());
    }

    /// <summary>
    /// The <see cref="RoleDataConverter.ToEntity(RoleCreationModel)"/> method should
    /// return a new <see cref="Role"/> instance populated
    /// with the data from the input <see cref="RoleCreationModel"/> object.
    /// </summary>
    [Fact]
    public void ToEntity_FromCreationModel()
    {
        // Arrange
        var model = new RoleCreationModel(
            name: "Name",
            isDefault: true);

        // Act
        var result = model.ToEntity();

        // Assert
        Assert.Equal(model.Name, result.Name);
        Assert.Equal(model.IsDefault, result.IsDefault);
    }

    /// <summary>
    /// The <see cref="RoleDataConverter.ToEntity(RoleCreationModel)"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="RoleCreationModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToEntity_FromCreationModel_ThrowsOnNull()
    {
        // Arrange
        var model = null as RoleCreationModel;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => model!.ToEntity());
    }

    /// <summary>
    /// The <see cref="RoleDataConverter.ToModel"/> method should
    /// return a new <see cref="RoleModel"/> instance populated
    /// with the data from the input <see cref="Role"/> object.
    /// </summary>
    [Fact]
    public void ToModel()
    {
        // Arrange
        var entity = new Role(name: "Name", isDefault: true);

        // Act
        var result = entity.ToModel();

        // Assert
        Assert.Equal(entity.Name, result.Name);
        Assert.Equal(entity.IsDefault, result.IsDefault);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Created, result.Created);
        Assert.Equal(entity.Modified, result.Modified);
    }

    /// <summary>
    /// The <see cref="RoleDataConverter.ToModel"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="Role"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToModel_ThrowsOnNull()
    {
        // Arrange
        var entity = null as Role;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => entity!.ToModel());
    }
    #endregion
}