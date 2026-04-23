using Microsoft.Extensions.Time.Testing;
using Paradise.DataAccess.Repositories.Implementation;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;

namespace Paradise.DataAccess.Tests.Unit.Repositories.Implementation;

/// <summary>
/// <see cref="InfrastructureUnitOfWork"/> test class.
/// </summary>
public sealed class InfrastructureUnitOfWorkTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="InfrastructureUnitOfWork"/> constructor should
    /// successfully initialize a new instance of the class.
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // Arrange
        var dataSource = new FakeDataSource(new FakeTimeProvider());
        var eailTemplatesRepository = new FakeEmailTemplatesRepository(dataSource);

        // Act
        var unitOfWork = new InfrastructureUnitOfWork(dataSource, eailTemplatesRepository);

        // Assert
        Assert.Same(eailTemplatesRepository, unitOfWork.EmailTemplatesRepository);
    }

    /// <summary>
    /// The <see cref="InfrastructureUnitOfWork.CommitAsync"/> method should
    /// persist all pending changes to the storage and return the number of changes made.
    /// </summary>
    [Fact]
    public async Task CommitAsync()
    {
        // Arrange
        var changesSaved = false;

        var dataSource = new FakeDataSource(new FakeTimeProvider());
        dataSource.ChangesSavedAsync += (_, _) => changesSaved = true;

        var eailTemplatesRepository = new FakeEmailTemplatesRepository(dataSource);

        var unitOfWork = new InfrastructureUnitOfWork(dataSource, eailTemplatesRepository);

        // Act
        var result = await unitOfWork.CommitAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, result);
        Assert.True(changesSaved);
    }
    #endregion
}