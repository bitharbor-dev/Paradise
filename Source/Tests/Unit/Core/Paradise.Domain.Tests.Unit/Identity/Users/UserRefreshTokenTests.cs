using Paradise.Domain.Base;
using Paradise.Domain.Base.Exceptions;
using Paradise.Domain.Identity.Users;

namespace Paradise.Domain.Tests.Unit.Identity.Users;

/// <summary>
/// <see cref="UserRefreshToken"/> test class.
/// </summary>
public sealed class UserRefreshTokenTests
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
    /// The <see cref="UserRefreshToken.GetEqualityComponents"/> method should
    /// return <see cref="ValueObject.Id"/>, <see cref="ValueObject.Created"/>
    /// and <see cref="UserRefreshToken.OwnerId"/> values.
    /// </summary>
    [Fact]
    public void GetEqualityComponents()
    {
        // Arrange
        var entity = new UserRefreshToken(ownerId: Id, expiryDateUtc: UnixEpoch);

        // Act
        var result = entity.GetEqualityComponents();

        // Assert
        Assert.Contains(entity.Id, result);
        Assert.Contains(entity.Created, result);
        Assert.Contains(entity.OwnerId, result);

        Assert.Equal(3, result.Count());
    }

    /// <summary>
    /// The <see cref="UserRefreshToken.ValidateState"/> method should
    /// not throw any exception for the <see cref="UserRefreshToken"/> instance
    /// which <see cref="UserRefreshToken.OwnerId"/> is not equal to <see cref="Guid.Empty"/>.
    /// </summary>
    [Fact]
    public void ValidateState()
    {
        // Arrange
        var entity = new UserRefreshToken(
            ownerId: Id,
            expiryDateUtc: UnixEpoch);

        // Act & Assert
        entity.ValidateState();
    }

    /// <summary>
    /// The <see cref="UserRefreshToken.ValidateState"/> method should
    /// throw the <see cref="DomainStateException{TEntity}"/> for the
    /// <see cref="UserRefreshToken"/> instance
    /// which <see cref="UserRefreshToken.OwnerId"/> is equal to <see cref="Guid.Empty"/>.
    /// </summary>
    [Fact]
    public void ValidateState_ThrowsOnEmptyOwnerId()
    {
        // Arrange
        var entity = new UserRefreshToken(
            ownerId: Guid.Empty,
            expiryDateUtc: UnixEpoch);

        // Act & Assert
        Assert.Throws<DomainStateException<UserRefreshToken>>(entity.ValidateState);
    }
    #endregion
}