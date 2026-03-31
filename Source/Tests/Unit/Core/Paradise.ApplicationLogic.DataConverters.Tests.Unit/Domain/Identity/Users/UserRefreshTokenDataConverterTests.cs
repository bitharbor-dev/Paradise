using Paradise.ApplicationLogic.DataConverters.Domain.Identity.Users;
using Paradise.Domain.Identity.Users;
using Paradise.Models.Domain.Identity.Users;

namespace Paradise.ApplicationLogic.DataConverters.Tests.Unit.Domain.Identity.Users;

/// <summary>
/// <see cref="UserRefreshTokenDataConverter"/> test class.
/// </summary>
public sealed class UserRefreshTokenDataConverterTests
{
    #region Properties
    /// <inheritdoc cref="DateTimeOffset.UnixEpoch"/>
    public static DateTimeOffset UnixEpoch { get; } = DateTimeOffset.UnixEpoch;

    /// <summary>
    /// Predefined GUID value to be used for data arrangement.
    /// </summary>
    public static Guid Id { get; } = Guid.Parse("0198610a-ac67-7bf0-8d08-676de1492235");
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="UserRefreshTokenDataConverter.ToModel"/> method should
    /// return a new <see cref="UserRefreshTokenModel"/> instance populated
    /// with the data from the input <see cref="UserRefreshToken"/> object.
    /// </summary>
    [Fact]
    public void ToModel()
    {
        // Arrange
        var entity = new UserRefreshToken(ownerId: Id, expiryDateUtc: UnixEpoch)
        {
            IsActive = true
        };

        // Act
        var result = entity.ToModel();

        // Assert
        Assert.Equal(entity.Created, result.Created);
        Assert.Equal(entity.ExpiryDateUtc, result.ExpiryDateUtc);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.IsActive, result.IsActive);
        Assert.Equal(entity.OwnerId, result.OwnerId);
    }

    /// <summary>
    /// The <see cref="UserRefreshTokenDataConverter.ToModel"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="UserRefreshToken"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToModel_ThrowsOnNull()
    {
        // Arrange
        var entity = null as UserRefreshToken;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => entity!.ToModel());
    }
    #endregion
}