using Microsoft.Extensions.Time.Testing;
using Paradise.DataAccess.Repositories.Implementation;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories.Domain.Identity.Users;

namespace Paradise.DataAccess.Tests.Unit.Repositories.Implementation;

/// <summary>
/// <see cref="DomainUnitOfWork"/> test class.
/// </summary>
public sealed class DomainUnitOfWorkTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="DomainUnitOfWork"/> constructor should
    /// successfully initialize a new instance of the class.
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // Arrange
        var dataSource = new FakeDataSource(new FakeTimeProvider());
        var userRefreshTokensRepository = new FakeUserRefreshTokensRepository(dataSource);

        // Act
        var unitOfWork = new DomainUnitOfWork(dataSource, userRefreshTokensRepository);

        // Assert
        Assert.Same(userRefreshTokensRepository, unitOfWork.UserRefreshTokensRepository);
    }

    /// <summary>
    /// The <see cref="DomainUnitOfWork.CommitAsync"/> method should
    /// persist all pending changes to the storage and return the number of changes made.
    /// </summary>
    [Fact]
    public async Task CommitAsync()
    {
        // Arrange
        var changesSaved = false;

        var dataSource = new FakeDataSource(new FakeTimeProvider());
        dataSource.ChangesSavedAsync += (_, _) => changesSaved = true;

        var userRefreshTokensRepository = new FakeUserRefreshTokensRepository(dataSource);

        var unitOfWork = new DomainUnitOfWork(dataSource, userRefreshTokensRepository);

        // Act
        var result = await unitOfWork.CommitAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, result);
        Assert.True(changesSaved);
    }
    #endregion
}