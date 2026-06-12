using Microsoft.Extensions.Time.Testing;
using Paradise.DataAccess.Repositories.Implementation;
using Paradise.Tests.Doubles.Fakes.DataAccess;
using Paradise.Tests.Doubles.Fakes.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.Tests.Doubles.Fakes.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

namespace Paradise.DataAccess.Tests.Unit.Repositories.Implementation;

/// <summary>
/// <see cref="UnitOfWork"/> test class.
/// </summary>
public sealed class UnitOfWorkTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="UnitOfWork"/> constructor should
    /// successfully initialize a new instance of the class.
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // Arrange
        var dataSource = new FakeDataSource(new FakeTimeProvider());
        var userRefreshTokensRepository = new FakeUserRefreshTokensRepository(dataSource);
        var emailTemplatesRepository = new FakeEmailTemplatesRepository(dataSource);

        // Act
        var unitOfWork = new UnitOfWork(dataSource, userRefreshTokensRepository, emailTemplatesRepository);

        // Assert
        Assert.Same(userRefreshTokensRepository, unitOfWork.UserRefreshTokensRepository);
        Assert.Same(emailTemplatesRepository, unitOfWork.EmailTemplatesRepository);
    }

    /// <summary>
    /// The <see cref="UnitOfWork.CommitAsync"/> method should
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
        var emailTemplatesRepository = new FakeEmailTemplatesRepository(dataSource);

        var unitOfWork = new UnitOfWork(dataSource, userRefreshTokensRepository, emailTemplatesRepository);

        // Act
        var result = await unitOfWork.CommitAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, result);
        Assert.True(changesSaved);
    }
    #endregion
}