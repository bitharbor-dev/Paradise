using Microsoft.Extensions.Time.Testing;
using Paradise.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity.Implementation;
using Paradise.DataAccess.Tests.Unit.Repositories.Base.Implementation;

namespace Paradise.DataAccess.Tests.Unit.Repositories.ApplicationLogic.Infrastructure.Domain.Identity.Implementation;

/// <summary>
/// <see cref="UserRefreshTokensRepository"/> test class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRefreshTokensRepositoryTests"/> class.
/// </remarks>
public sealed class UserRefreshTokensRepositoryTests()
    : RepositoryTests<UserRefreshTokensRepository, UserRefreshToken>(new FakeTimeProvider())
{
    #region Public methods
    /// <summary>
    /// The <see cref="UserRefreshTokensRepository.GetUserTokensAsync"/> method should
    /// return the list of refresh tokens which belong to user with the specified Id.
    /// </summary>
    [Fact]
    public async Task GetUserTokensAsync()
    {
        // Arrange
        var userId = Guid.AllBitsSet;
        var refreshToken = new UserRefreshToken(userId, DateTimeOffset.UnixEpoch);

        Source.Add(refreshToken);
        await Source.SaveChangesAsync(Token);

        // Act
        var result = await Repository.GetUserTokensAsync(userId, Token);

        // Assert
        Assert.Contains(result, token => token.Id == refreshToken.Id
                                      && token.OwnerId == refreshToken.OwnerId);
    }
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override UserRefreshToken GetTestEntity(Guid? id = null, DateTimeOffset? created = null, DateTimeOffset? modified = null)
    {
        var ownerId = Guid.NewGuid();
        var expiryDateUtc = DateTimeOffset.UnixEpoch;

        var userRefreshToken = new UserRefreshToken(ownerId, expiryDateUtc);

        var targetType = typeof(UserRefreshToken);

        if (id.HasValue)
        {
            var idProperty = targetType.GetProperty(nameof(UserRefreshToken.Id));
            idProperty!.SetValue(userRefreshToken, id.Value);
        }

        if (created.HasValue)
        {
            var createdProperty = targetType.GetProperty(nameof(UserRefreshToken.Created));
            createdProperty!.SetValue(userRefreshToken, created.Value);
        }

        if (modified.HasValue)
        {
            var modifiedProperty = targetType.GetProperty(nameof(UserRefreshToken.Modified));
            modifiedProperty!.SetValue(userRefreshToken, modified.Value);
        }

        return userRefreshToken;
    }
    #endregion
}