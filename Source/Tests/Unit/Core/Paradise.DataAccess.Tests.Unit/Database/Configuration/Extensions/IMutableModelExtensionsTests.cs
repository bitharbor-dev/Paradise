using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Paradise.DataAccess.Database.Configuration.Extensions;
using Paradise.Domain.Base;
using Paradise.Tests.Doubles.Dummies.Core.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Tests.Unit.Database.Configuration.Extensions;

/// <summary>
/// <see cref="IMutableModelExtensions"/> test class.
/// </summary>
public sealed class IMutableModelExtensionsTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="IMutableModelExtensions.MarkPropertyAsReadOnly"/> method should
    /// set the after-save behavior of the property with the specified name to
    /// <see cref="PropertySaveBehavior.Throw"/> and leave other properties
    /// after-save behavior unchanged.
    /// </summary>
    [Fact, SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
    public void MarkColumnAsReadOnly()
    {
        // Arrange
        var model = new Model(null);

        var entity = model.AddEntityType(typeof(DummyEntity), false, ConfigurationSource.Explicit);

        var created = entity!.AddProperty(nameof(DummyEntity.Created),
                                          typeof(DateTimeOffset),
                                          ConfigurationSource.Explicit,
                                          ConfigurationSource.Explicit);

        var modified = entity.AddProperty(nameof(DummyEntity.Modified),
                                          typeof(DateTimeOffset),
                                          ConfigurationSource.Explicit,
                                          ConfigurationSource.Explicit);

        // Act
        model.MarkPropertyAsReadOnly(nameof(DummyEntity.Created));

        // Assert
        Assert.Equal(PropertySaveBehavior.Throw, created!.GetAfterSaveBehavior());
        Assert.Equal(PropertySaveBehavior.Save, modified!.GetAfterSaveBehavior());
    }

    /// <summary>
    /// The <see cref="IMutableModelExtensions.DisableValueGenerationFor"/> method should
    /// disable value generation for matching properties across all entities
    /// when no base type filter is specified.
    /// </summary>
    [Fact, SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
    public void DisableValueGenerationFor()
    {
        // Arrange
        var targetPropertyName = nameof(DummyEntity.Id);

        var model = new Model(null);

        var entity = model.AddEntityType(typeof(DummyEntity), false, ConfigurationSource.Explicit);

        var id = entity!.AddProperty(targetPropertyName,
                                     typeof(Guid),
                                     ConfigurationSource.Explicit,
                                     ConfigurationSource.Explicit);

        id!.ValueGenerated = ValueGenerated.OnAdd;

        // Act
        model.DisableValueGenerationFor(targetPropertyName);

        // Assert
        Assert.Equal(ValueGenerated.Never, id.ValueGenerated);
    }

    /// <summary>
    /// The <see cref="IMutableModelExtensions.DisableValueGenerationFor"/> method should
    /// disable value generation only for entities assignable to the specified base type.
    /// </summary>
    [Fact, SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
    public void DisableValueGenerationFor_AppliesFilter()
    {
        // Arrange
        var targetPropertyName = nameof(Entity.Id);

        var model = new Model(null);

        var affectedEntity = model.AddEntityType(typeof(DummyEntity), false, ConfigurationSource.Explicit);

        var id = affectedEntity!.AddProperty(targetPropertyName,
                                             typeof(Guid),
                                             ConfigurationSource.Explicit,
                                             ConfigurationSource.Explicit);

        id!.ValueGenerated = ValueGenerated.OnAdd;

        var unaffectedEntity = model.AddEntityType(typeof(AnotherDummyEntity), false, ConfigurationSource.Explicit);

        var anotherId = unaffectedEntity!.AddProperty(targetPropertyName,
                                                      typeof(Guid),
                                                      ConfigurationSource.Explicit,
                                                      ConfigurationSource.Explicit);

        anotherId!.ValueGenerated = ValueGenerated.OnAdd;

        // Act
        model.DisableValueGenerationFor(targetPropertyName, typeof(DummyEntity));

        // Assert
        Assert.Equal(ValueGenerated.Never, id.ValueGenerated);
        Assert.Equal(ValueGenerated.OnAdd, anotherId.ValueGenerated);
    }

    /// <summary>
    /// The <see cref="IMutableModelExtensions.DisableValueGenerationFor"/> method should
    /// ignore entities that do not contain the specified property.
    /// </summary>
    [Fact, SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
    public void DisableValueGenerationFor_IgnoresMissingProperties()
    {
        // Arrange
        var model = new Model(null);

        model.AddEntityType(typeof(DummyEntity), false, ConfigurationSource.Explicit);

        // Act
        var exception = Record.Exception(()
            => model.DisableValueGenerationFor("MissingProperty"));

        // Assert
        Assert.Null(exception);
    }

    /// <summary>
    /// The <see cref="IMutableModelExtensions.SetSchemaFor"/> method should
    /// assign the specified schema to the target entity type.
    /// </summary>
    [Fact, SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
    public void SetSchemaFor()
    {
        // Arrange
        var schema = "dbo";

        var model = new Model(null);

        var entity = model.AddEntityType(typeof(DummyEntity), false, ConfigurationSource.Explicit);

        entity?.SetSchema(schema);

        // Act
        model.SetSchemaFor<DummyEntity>("test");

        // Assert
        Assert.Equal("test", entity?.GetSchema());
    }

    /// <summary>
    /// The <see cref="IMutableModelExtensions.SetSchemaFor"/> method should
    /// ignore missing entity types.
    /// </summary>
    [Fact, SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
    public void SetSchemaFor_IgnoresMissingEntityType()
    {
        // Arrange

        var model = new Model(null);

        // Act
        var exception = Record.Exception(()
            => model.SetSchemaFor<DummyEntity>("custom"));

        // Assert
        Assert.Null(exception);
    }
    #endregion
}