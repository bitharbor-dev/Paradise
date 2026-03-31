using Paradise.ApplicationLogic.DataConverters.Domain.Identity.Users;
using Paradise.Models.Domain.Identity.Users;
using System.Security.Claims;

namespace Paradise.ApplicationLogic.DataConverters.Tests.Unit.Domain.Identity.Users;

/// <summary>
/// <see cref="ClaimDataConverter"/> test class.
/// </summary>
public sealed class ClaimDataConverterTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="ClaimDataConverter.ToModel"/> method should
    /// return a new <see cref="UserClaimModel"/> instance populated
    /// with the data from the input <see cref="Claim"/> object.
    /// </summary>
    [Fact]
    public void ToModel()
    {
        // Arrange
        var claim = new Claim(type: "Type", value: "Value");

        // Act
        var result = claim.ToModel();

        // Assert
        Assert.Equal(claim.Type, result.Type);
        Assert.Equal(claim.Value, result.Value);
    }

    /// <summary>
    /// The <see cref="ClaimDataConverter.ToModel"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="Claim"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ToModel_ThrowsOnNull()
    {
        // Arrange
        var claim = null as Claim;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => claim!.ToModel());
    }
    #endregion
}