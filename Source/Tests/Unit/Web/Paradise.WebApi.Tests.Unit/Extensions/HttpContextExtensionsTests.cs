using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Paradise.WebApi.Extensions;
using System.Security.Claims;

namespace Paradise.WebApi.Tests.Unit.Extensions;

/// <summary>
/// <see cref="HttpContextExtensions"/> test class.
/// </summary>
public sealed class HttpContextExtensionsTests
{

    #region Constants
    private const string UserIdClaimType = "unique-identifier";
    #endregion

    #region Properties
    /// <summary>
    /// Provides member data for <see cref="GetUserId_ReturnsEmptyOnInvalidClaimValue"/> method.
    /// </summary>
    public static TheoryData<string> GetUserId_ReturnsEmptyOnInvalidClaimValue_MemberData { get; } = new()
    {
        { string.Empty  },
        { " "           }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="HttpContextExtensions.GetUserId"/> method should
    /// return the authenticated user's identifier
    /// when the required claim exists and contains a valid <see cref="Guid"/> value.
    /// </summary>
    [Fact]
    public void GetUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var context = CreateContext(new(UserIdClaimType, userId.ToString()));

        // Act
        var result = context.GetUserId();

        // Assert
        Assert.Equal(userId, result);
    }

    /// <summary>
    /// The <see cref="HttpContextExtensions.GetUserId"/> method should
    /// use the configured claim type from <see cref="IdentityOptions"/> to
    /// return the authenticated user's identifier.
    /// </summary>
    [Fact]
    public void GetUserId_UsesConfiguredClaimType()
    {
        // Arrange
        const string CustomClaimType = "id";

        var userId = Guid.NewGuid();

        var context = CreateContext(new(CustomClaimType, userId.ToString()), CustomClaimType);

        // Act
        var result = context.GetUserId();

        // Assert
        Assert.Equal(userId, result);
    }

    /// <summary>
    /// The <see cref="HttpContextExtensions.GetUserId"/> method should
    /// return <see cref="Guid.Empty"/> if the
    /// required user identifier claim does not exist.
    /// </summary>
    [Fact]
    public void GetUserId_ReturnsEmptyOnMissingClaim()
    {
        // Arrange
        var context = CreateContext();

        // Act
        var result = context.GetUserId();

        // Assert
        Assert.Equal(Guid.Empty, result);
    }

    /// <summary>
    /// The <see cref="HttpContextExtensions.GetUserId"/> method should
    /// return <see cref="Guid.Empty"/> if the
    /// user identifier claim value is invalid.
    /// </summary>
    /// <param name="claimValue">
    /// Invalid claim value.
    /// </param>
    [Theory, MemberData(nameof(GetUserId_ReturnsEmptyOnInvalidClaimValue_MemberData))]
    public void GetUserId_ReturnsEmptyOnInvalidClaimValue(string claimValue)
    {
        // Arrange
        var context = CreateContext(new(UserIdClaimType, claimValue));

        // Act
        var result = context.GetUserId();

        // Assert
        Assert.Equal(Guid.Empty, result);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates a configured <see cref="HttpContext"/> instance for testing.
    /// </summary>
    /// <param name="userIdClaim">
    /// The claim to add.
    /// </param>
    /// <param name="configuredIdClaimType">
    /// The expected type of the user Id claim.
    /// </param>
    /// <returns>
    /// Configured <see cref="HttpContext"/> instance.
    /// </returns>
    private static DefaultHttpContext CreateContext(Claim? userIdClaim = null,
                                                    string configuredIdClaimType = UserIdClaimType)
    {
        void UseConfiguredClaimType(IdentityOptions options)
        {
            var claimsOptions = options.ClaimsIdentity;
            claimsOptions.UserIdClaimType = configuredIdClaimType;
        }

        var services = new ServiceCollection().Configure<IdentityOptions>(UseConfiguredClaimType);
        var claims = userIdClaim is not null ? new[] { userIdClaim } : [];
        var identity = new ClaimsIdentity(claims);

        return new()
        {
            RequestServices = services.BuildServiceProvider(),
            User = new(identity)
        };
    }
    #endregion
}