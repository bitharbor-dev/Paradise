using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.EventListeners.Base;
using Paradise.ApplicationLogic.Infrastructure.DataProtection;
using Paradise.Common.Web;
using Paradise.Models;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.DataProtection;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.ApplicationLogic.EventListeners.Base;

namespace Paradise.ApplicationLogic.Tests.Unit.EventListeners.Base;

/// <summary>
/// <see cref="IdentityTokenEventListener{TEvent}"/> test class.
/// </summary>
public sealed class IdentityTokenEventListenerTests
{
    #region Constants
    private const string IdentityTokenPlaceholder = $"{{{ParameterNames.IdentityTokenParameter}}}";
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityTokenEventListenerTests"/> class.
    /// </summary>
    public IdentityTokenEventListenerTests()
    {
        Protector = new FakeDataProtector();

        var provider = new ServiceCollection()
            .AddScoped(_ => Protector)
            .BuildServiceProvider();

        Listener = new(provider);
    }
    #endregion

    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    public TestIdentityTokenEventListener Listener { get; }

    /// <summary>
    /// An <see cref="IDataProtector"/> instance used to
    /// arrange data and validate test results.
    /// </summary>
    public IDataProtector Protector { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="IdentityTokenEventListener{TEvent}.ProtectIdentityToken"/> method should
    /// return a protected identity token.
    /// </summary>
    /// <remarks>
    /// The <see cref="IdentityTokenEventListener{TEvent}.ProtectIdentityToken"/> method is
    /// indirectly called via the <see cref="TestIdentityTokenEventListener.ExposeProtectIdentityToken"/> method.
    /// </remarks>
    [Fact]
    public void CreateIdentityToken()
    {
        // Arrange
        var emailAddress = "test@email.com";
        var innerToken = "InnerToken";

        var identityTokenModel = new IdentityToken(emailAddress, innerToken);
        var identityToken = Protector.Protect(identityTokenModel);

        // Act
        var result = Listener.ExposeProtectIdentityToken(identityTokenModel);

        // Assert
        Assert.Equal(identityToken, result);
    }

    /// <summary>
    /// The <see cref="IdentityTokenEventListener{TEvent}.CreateIdentityTokenLink"/> method should
    /// return a URI containing a web-encoded protected identity token.
    /// </summary>
    /// <remarks>
    /// The <see cref="IdentityTokenEventListener{TEvent}.CreateIdentityTokenLink"/> method is
    /// indirectly called via the <see cref="TestIdentityTokenEventListener.ExposeCreateIdentityTokenLink"/> method.
    /// </remarks>
    [Fact]
    public void CreateIdentityTokenLink()
    {
        // Arrange
        var emailAddress = "test@email.com";
        var innerToken = "InnerToken";

        var baseUrl = new Uri("https://localhost:5001");
        var path = $"/path?token={IdentityTokenPlaceholder}";

        var identityTokenModel = new IdentityToken(emailAddress, innerToken);
        var identityToken = Protector.Protect(identityTokenModel);

        var formattedPath = $"{path.Replace(IdentityTokenPlaceholder, identityToken, StringComparison.Ordinal)}";
        var expectedLink = new Uri(baseUrl, formattedPath);

        // Act
        var result = Listener.ExposeCreateIdentityTokenLink(identityTokenModel, baseUrl, path);

        // Assert
        Assert.Equal(expectedLink, result);
    }

    /// <summary>
    /// The <see cref="IdentityTokenEventListener{TEvent}.CreateIdentityTokenLink"/> method should
    /// return a URI containing a web-encoded protected identity token
    /// and a query string built from provider query parameters.
    /// </summary>
    /// <remarks>
    /// The <see cref="IdentityTokenEventListener{TEvent}.CreateIdentityTokenLink"/> method is
    /// indirectly called via the <see cref="TestIdentityTokenEventListener.ExposeCreateIdentityTokenLink"/> method.
    /// </remarks>
    [Fact]
    public void CreateIdentityTokenLink_WithQueryParameters()
    {
        // Arrange
        var emailAddress = "test@email.com";
        var innerToken = "InnerToken";

        var baseUrl = new Uri("https://localhost:5001");
        var path = $"/path?token={IdentityTokenPlaceholder}";

        var identityTokenModel = new IdentityToken(emailAddress, innerToken);
        var identityToken = Protector.Protect(identityTokenModel);

        var queryParameters = new Dictionary<string, string?>
        {
            ["key"] = "value"
        };
        var queryString = QueryString.Create(queryParameters).Value;

        var formattedPath = $"{path.Replace(IdentityTokenPlaceholder, identityToken, StringComparison.Ordinal)}{queryString}";
        var expectedLink = new Uri(baseUrl, formattedPath);

        // Act
        var result = Listener.ExposeCreateIdentityTokenLink(identityTokenModel, baseUrl, path, queryParameters);

        // Assert
        Assert.Equal(expectedLink, result);
    }

    /// <summary>
    /// The <see cref="IdentityTokenEventListener{TEvent}.CreateIdentityTokenLink"/> method should
    /// throw the <see cref="ArgumentException"/> if the input
    /// path lacks identity token placeholder.
    /// </summary>
    /// <remarks>
    /// The <see cref="IdentityTokenEventListener{TEvent}.CreateIdentityTokenLink"/> method is
    /// indirectly called via the <see cref="TestIdentityTokenEventListener.ExposeCreateIdentityTokenLink"/> method.
    /// </remarks>
    [Fact]
    public void CreateIdentityTokenLink_ThrowsOnMissingPlaceholder()
    {
        // Arrange
        var emailAddress = "test@email.com";
        var innerToken = "InnerToken";

        var baseUrl = new Uri("https://localhost:5001");
        var path = "/path?token={InvalidPlaceholder}";

        // Act & Assert
        Assert.Throws<ArgumentException>(()
            => Listener.ExposeCreateIdentityTokenLink(new(emailAddress, innerToken), baseUrl, path));
    }

    /// <summary>
    /// The <see cref="IdentityTokenEventListener{TEvent}.CreateIdentityTokenLink"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="Uri"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void CreateIdentityTokenLink_ThrowsOnNull()
    {
        // Arrange
        var emailAddress = "test@email.com";
        var innerToken = "InnerToken";

        var baseUrl = null as Uri;
        var path = $"/path?token={IdentityTokenPlaceholder}";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Listener.ExposeCreateIdentityTokenLink(new(emailAddress, innerToken), baseUrl, path));
    }
    #endregion
}