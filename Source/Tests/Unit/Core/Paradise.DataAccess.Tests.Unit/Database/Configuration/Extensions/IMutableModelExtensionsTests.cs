using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Paradise.DataAccess.Database.Configuration.Extensions;
using Paradise.Tests.Miscellaneous.TestDoubles.Dummies.Core.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.DataAccess.Tests.Unit.Database.Configuration.Extensions;

/// <summary>
/// <see cref="IMutableModelExtensions"/> test class.
/// </summary>
public sealed class IMutableModelExtensionsTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="IMutableModelExtensions.MarkColumnAsReadOnly"/> method should
    /// set the after-save behavior of the property with the specified name to
    /// <see cref="PropertySaveBehavior.Throw"/> and leave other properties
    /// after-save behavior unchanged.
    /// </summary>
    [Fact]
    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required for unit testing.")]
    public void MarkColumnAsReadOnly()
    {
        // Arrange
        var model = new Model(null);

        var entity = model.AddEntityType(typeof(DummyEntity),
                                         false,
                                         ConfigurationSource.Explicit);

        var createdProperty = entity!.AddProperty(nameof(DummyEntity.Created),
                                                  typeof(DateTimeOffset),
                                                  ConfigurationSource.Explicit,
                                                  ConfigurationSource.Explicit);

        var modifiedProperty = entity.AddProperty(nameof(DummyEntity.Modified),
                                                 typeof(DateTimeOffset),
                                                 ConfigurationSource.Explicit,
                                                 ConfigurationSource.Explicit);

        // Act
        model.MarkColumnAsReadOnly(nameof(DummyEntity.Created));

        // Assert
        Assert.Equal(PropertySaveBehavior.Throw, createdProperty!.GetAfterSaveBehavior());
        Assert.Equal(PropertySaveBehavior.Save, modifiedProperty!.GetAfterSaveBehavior());
    }
    #endregion
}