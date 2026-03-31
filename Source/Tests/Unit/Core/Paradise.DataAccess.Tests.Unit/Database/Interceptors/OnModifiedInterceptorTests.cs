using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Paradise.DataAccess.Database.Interceptors;
using Paradise.DataAccess.Database.Interceptors.Base;
using Paradise.Domain.Base;
using Paradise.Tests.Miscellaneous.TestDoubles.Dummies.Core.Domain.Base;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Paradise.DataAccess.Tests.Unit.Database.Interceptors;

/// <summary>
/// <see cref="OnModifiedInterceptor"/> test class.
/// </summary>
public sealed partial class OnModifiedInterceptorTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="OnModifiedInterceptor.Intercept"/> method should
    /// set the <see cref="IDomainObject.Modified"/> to the same
    /// value which was passed into the method as transaction time.
    /// </summary>
    [Fact]
    public void Intercept()
    {
        // Arrange
        var currentTime = Test.UtcNow;

        var entity = new DummyEntity();
        var state = EntityState.Modified;

        var entry = new FakeEntityEntry(entity, state);
        var properties = new DbContextEventProperties
        {
            TransactionTime = currentTime
        };

        // Act
        Test.Target.Intercept(entry, properties);

        // Assert
        Assert.Equal(currentTime, entity.Modified);
    }

    /// <summary>
    /// The <see cref="OnModifiedInterceptor.Intercept"/> method should
    /// ignore the entity which does not implement the <see cref="IDomainObject"/>
    /// <see langword="interface"/> and make no interactions with such object.
    /// </summary>
    [Fact]
    public void Intercept_IgnoresNonDomainObject()
    {
        // Arrange
        var entity = new object();
        var state = EntityState.Modified;

        var entry = new FakeEntityEntry(entity, state);
        var properties = new DbContextEventProperties();

        // Act & Assert
        Test.Target.Intercept(entry, properties);
    }

    /// <summary>
    /// The <see cref="OnModifiedInterceptor.Intercept"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="EntityEntry"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void Intercept_ThrowsOnNullEntry()
    {
        // Arrange
        var entry = null as EntityEntry;
        var properties = new DbContextEventProperties();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Test.Target.Intercept(entry!, properties));
    }

    /// <summary>
    /// The <see cref="OnModifiedInterceptor.Intercept"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="DbContextEventProperties"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void Intercept_ThrowsOnNullProperties()
    {
        // Arrange
        var entity = new DummyEntity();
        var state = EntityState.Modified;

        var entry = new FakeEntityEntry(entity, state);
        var properties = null as DbContextEventProperties;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Test.Target.Intercept(entry, properties!));
    }

    /// <summary>
    /// The <see cref="OnModifiedInterceptor.EntityFilter"/> property should
    /// return <see langword="true"/> upon property invocation with the
    /// <see cref="EntityEntry"/> object passed in, with it's entity of type
    /// <see cref="IDomainObject"/> and state equal to <see cref="EntityState.Modified"/>.
    /// </summary>
    [Fact]
    public void EntityFilter_ReturnsTrue()
    {
        // Arrange
        var entry = new FakeEntityEntry(new DummyEntity(), EntityState.Modified);

        // Act
        var result = Test.Target.EntityFilter.Invoke(entry);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="OnModifiedInterceptor.EntityFilter"/> property should
    /// return <see langword="false"/> upon property invocation with the
    /// <see cref="EntityEntry"/> object passed in, with it's entity of type not equal
    /// <see cref="IDomainObject"/> and state equal to <see cref="EntityState.Modified"/>.
    /// </summary>
    [Fact]
    public void EntityFilter_ReturnsFalseOnNonDomainObject()
    {
        // Arrange
        var entry = new FakeEntityEntry(new object(), EntityState.Modified);

        // Act
        var result = Test.Target.EntityFilter.Invoke(entry);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="OnModifiedInterceptor.EntityFilter"/> property should
    /// return <see langword="false"/> upon property invocation with the
    /// <see cref="EntityEntry"/> object passed in, with it's entity of type
    /// <see cref="IDomainObject"/> and state not equal to <see cref="EntityState.Modified"/>.
    /// </summary>
    [Fact]
    public void EntityFilter_ReturnsFalseOnNonModifiedEntity()
    {
        // Arrange
        var entry = new FakeEntityEntry(new DummyEntity(), EntityState.Added);

        // Act
        var result = Test.Target.EntityFilter.Invoke(entry);

        // Assert
        Assert.False(result);
    }
    #endregion
}