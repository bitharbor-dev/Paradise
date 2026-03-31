using Microsoft.EntityFrameworkCore;
using Paradise.DataAccess.Database.Interceptors.Base;

namespace Paradise.DataAccess.Tests.Unit.Database.Interceptors.Base;

/// <summary>
/// <see cref="BaseSaveChangesInterceptor"/> test class.
/// </summary>
public sealed partial class BaseSaveChangesInterceptorTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="BaseSaveChangesInterceptor.SavingChanges"/> method should
    /// intercept the state of the entity being saved.
    /// </summary>
    [Fact]
    public void SavingChanges_WithEntries_NoFilter()
    {
        // Arrange
        var entityEntry = Test.CreateLinkedEntry();

        // Act
        Test.Target.SavingChanges(Test.GetEventData(), new());

        // Assert
        Assert.Contains(entityEntry, Test.InterceptedEntries, Test.Comparer);
    }

    /// <summary>
    /// The <see cref="BaseSaveChangesInterceptor.SavingChanges"/> method should
    /// intercept the state of the entity being saved
    /// which satisfies the condition in the entity filter
    /// configured on the interceptor instance.
    /// </summary>
    [Fact]
    public void SavingChanges_WithEntries_Filter()
    {
        // Arrange
        var entityEntry = Test.CreateLinkedEntry();
        entityEntry.State = EntityState.Modified;

        Test.SetEntityFilter(entry => entry.State is EntityState.Modified);

        // Act
        Test.Target.SavingChanges(Test.GetEventData(), new());

        // Assert
        Assert.Contains(entityEntry, Test.InterceptedEntries, Test.Comparer);
    }

    /// <summary>
    /// The <see cref="BaseSaveChangesInterceptor.SavingChanges"/> method should
    /// not intercept the state of the entity being saved
    /// which does not satisfy the condition in the entity filter
    /// configured on the interceptor instance.
    /// </summary>
    [Fact]
    public void SavingChanges_WithEntries_FilterNotMatching()
    {
        // Arrange
        var entityEntry = Test.CreateLinkedEntry();
        entityEntry.State = EntityState.Added;

        Test.SetEntityFilter(entry => entry.State is EntityState.Modified);

        // Act
        Test.Target.SavingChanges(Test.GetEventData(), new());

        // Assert
        Assert.DoesNotContain(entityEntry, Test.InterceptedEntries, Test.Comparer);
    }

    /// <summary>
    /// The <see cref="BaseSaveChangesInterceptor.SavingChanges"/> method should
    /// not intercept the state of the entities as no entities are currently being saved.
    /// </summary>
    [Fact]
    public void SavingChanges_WithoutEntries_NoFilter()
    {
        // Arrange

        // Act
        Test.Target.SavingChanges(Test.GetEventData(), new());

        // Assert
        Assert.Empty(Test.InterceptedEntries);
    }

    /// <summary>
    /// The <see cref="BaseSaveChangesInterceptor.SavingChanges"/> method should
    /// fail to intercept the state of the entities being saved if the internal
    /// <see cref="DbContext"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void SavingChanges_FailsOnNullDbContext()
    {
        // Arrange
        var entityEntry = Test.CreateLinkedEntry();

        // Act
        Test.Target.SavingChanges(Test.GetEventData(passNullDbContext: true), new());

        // Assert
        Assert.DoesNotContain(entityEntry, Test.InterceptedEntries, Test.Comparer);
    }

    /// <summary>
    /// The <see cref="BaseSaveChangesInterceptor.SavingChangesAsync"/> method should
    /// intercept the state of the entity being saved.
    /// </summary>
    [Fact]
    public async Task SavingChangesAsync_WithEntries_NoFilter()
    {
        // Arrange
        var entityEntry = Test.CreateLinkedEntry();

        // Act
        await Test.Target.SavingChangesAsync(Test.GetEventData(), new(), Token);

        // Assert
        Assert.Contains(entityEntry, Test.InterceptedEntries, Test.Comparer);
    }

    /// <summary>
    /// The <see cref="BaseSaveChangesInterceptor.SavingChangesAsync"/> method should
    /// intercept the state of the entity being saved
    /// which satisfies the condition in the entity filter
    /// configured on the interceptor instance.
    /// </summary>
    [Fact]
    public async Task SavingChangesAsync_WithEntries_Filter()
    {
        // Arrange
        var entityEntry = Test.CreateLinkedEntry();
        entityEntry.State = EntityState.Modified;

        Test.SetEntityFilter(entry => entry.State is EntityState.Modified);

        // Act
        await Test.Target.SavingChangesAsync(Test.GetEventData(), new(), Token);

        // Assert
        Assert.Contains(entityEntry, Test.InterceptedEntries, Test.Comparer);
    }

    /// <summary>
    /// The <see cref="BaseSaveChangesInterceptor.SavingChangesAsync"/> method should
    /// not intercept the state of the entity being saved
    /// which does not satisfy the condition in the entity filter
    /// configured on the interceptor instance.
    /// </summary>
    [Fact]
    public async Task SavingChangesAsync_WithEntries_FilterNotMatching()
    {
        // Arrange
        var entityEntry = Test.CreateLinkedEntry();
        entityEntry.State = EntityState.Added;

        Test.SetEntityFilter(entry => entry.State is EntityState.Modified);

        // Act
        await Test.Target.SavingChangesAsync(Test.GetEventData(), new(), Token);

        // Assert
        Assert.DoesNotContain(entityEntry, Test.InterceptedEntries, Test.Comparer);
    }

    /// <summary>
    /// The <see cref="BaseSaveChangesInterceptor.SavingChangesAsync"/> method should
    /// not intercept the state of the entities as no entities are currently being saved.
    /// </summary>
    [Fact]
    public async Task SavingChangesAsync_WithoutEntries_NoFilter()
    {
        // Arrange

        // Act
        await Test.Target.SavingChangesAsync(Test.GetEventData(), new(), Token);

        // Assert
        Assert.Empty(Test.InterceptedEntries);
    }

    /// <summary>
    /// The <see cref="BaseSaveChangesInterceptor.SavingChangesAsync"/> method should
    /// fail to intercept the state of the entities being saved if the internal
    /// <see cref="DbContext"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task SavingChangesAsync_FailsOnNullDbContext()
    {
        // Arrange
        Test.CreateLinkedEntry();

        // Act
        await Test.Target.SavingChangesAsync(Test.GetEventData(passNullDbContext: true), new(), Token);

        // Assert
        Assert.Empty(Test.InterceptedEntries);
    }
    #endregion
}