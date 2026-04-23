using Paradise.WebApi.Extensions;
using System.Security.Claims;

namespace Paradise.WebApi.Tests.Unit.Extensions;

/// <summary>
/// <see cref="ClaimsPrincipalExtensions"/> test class.
/// </summary>
public sealed class ClaimsPrincipalExtensionsTests
{
    #region Constants
    private const string IdClaimType = "UniqueIdentifier";
    private const string RoleClaimType = "Role";
    #endregion

    #region Properties
    /// <summary>
    /// Provides member data for <see cref="GetGuidClaim_ReturnsEmptyOnEmptyValue"/> method.
    /// </summary>
    public static TheoryData<string> GetGuidClaim_ReturnsEmptyOnEmptyValue_MemberData { get; } = new()
    {
        { string.Empty  },
        { " "           }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="ClaimsPrincipalExtensions.GetGuidClaim"/> method should
    /// return <see cref="Guid"/> value if
    /// a claim with the specified type exists and it's value is a valid UUID string.
    /// </summary>
    [Fact]
    public void GetGuidClaim()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id = new Claim(IdClaimType, guid.ToString());

        var principal = CreatePrincipal(id);

        // Act
        var result = principal.GetGuidClaim(IdClaimType);

        // Assert
        Assert.Equal(guid, result);
    }

    /// <summary>
    /// The <see cref="ClaimsPrincipalExtensions.GetGuidClaim"/> method should
    /// return <see cref="Guid.Empty"/> if
    /// no claim with the specified type exists.
    /// </summary>
    [Fact]
    public void GetGuidClaim_ReturnsEmptyOnMissingClaim()
    {
        // Arrange
        var principal = CreatePrincipal();

        // Act
        var result = principal.GetGuidClaim(IdClaimType);

        // Assert
        Assert.Equal(Guid.Empty, result);
    }

    /// <summary>
    /// The <see cref="ClaimsPrincipalExtensions.GetGuidClaim"/> method should
    /// return <see cref="Guid.Empty"/> if
    /// a claim with the specified type exists and it's value
    /// is equal to <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    [Theory, MemberData(nameof(GetGuidClaim_ReturnsEmptyOnEmptyValue_MemberData))]
    public void GetGuidClaim_ReturnsEmptyOnEmptyValue(string value)
    {
        // Arrange
        var id = new Claim(IdClaimType, value);

        var principal = CreatePrincipal(id);

        // Act
        var result = principal.GetGuidClaim(IdClaimType);

        // Assert
        Assert.Equal(Guid.Empty, result);
    }

    /// <summary>
    /// The <see cref="ClaimsPrincipalExtensions.GetGuidClaim"/> method should
    /// throw the <see cref="FormatException"/> if the
    /// claim value is not a valid UUID string.
    /// </summary>
    [Fact]
    public void GetGuidClaim_ThrowsOnInvalidGuidFormat()
    {
        // Arrange
        var invalidGuid = "invalidUuid";
        var id = new Claim(IdClaimType, invalidGuid);

        var principal = CreatePrincipal(id);

        // Act & Assert
        Assert.Throws<FormatException>(()
            => principal.GetGuidClaim(IdClaimType));
    }

    /// <summary>
    /// The <see cref="ClaimsPrincipalExtensions.GetGuidClaim"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="ClaimsPrincipal"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void GetGuidClaim_ThrowsOnNull()
    {
        // Arrange
        var principal = null as ClaimsPrincipal;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => principal!.GetGuidClaim(IdClaimType));
    }

    /// <summary>
    /// The <see cref="ClaimsPrincipalExtensions.FindValues"/> method should
    /// return all claim values for given <see cref="Claim.Type"/>.
    /// </summary>
    [Fact]
    public void FindValues()
    {
        // Arrange
        var id = new Claim(IdClaimType, Guid.NewGuid().ToString());
        var role1 = new Claim(RoleClaimType, Guid.NewGuid().ToString());
        var role2 = new Claim(RoleClaimType, Guid.NewGuid().ToString());

        var principal = CreatePrincipal(id, role1, role2);

        // Act
        var result = principal.FindValues(RoleClaimType);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(role1.Value, result);
        Assert.Contains(role2.Value, result);
    }

    /// <summary>
    /// The <see cref="ClaimsPrincipalExtensions.FindValues"/> method should
    /// return empty collection when no claims of given type exist.
    /// </summary>
    [Fact]
    public void FindValues_ReturnsEmptyOnClaimTypeMismatch()
    {
        // Arrange
        var id = new Claim(IdClaimType, Guid.NewGuid().ToString());

        var principal = CreatePrincipal(id);

        // Act
        var result = principal.FindValues(RoleClaimType);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// The <see cref="ClaimsPrincipalExtensions.FindValues"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="ClaimsPrincipal"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void FindValues_ThrowsOnNull()
    {
        // Arrange
        var principal = null as ClaimsPrincipal;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => principal!.FindValues(IdClaimType));
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> with the given <paramref name="claims"/>.
    /// </summary>
    /// <param name="claims">
    /// Claims to include.
    /// </param>
    /// <returns>
    /// Constructed principal.
    /// </returns>
    private static ClaimsPrincipal CreatePrincipal(params IEnumerable<Claim> claims)
        => new(new ClaimsIdentity(claims));
    #endregion
}