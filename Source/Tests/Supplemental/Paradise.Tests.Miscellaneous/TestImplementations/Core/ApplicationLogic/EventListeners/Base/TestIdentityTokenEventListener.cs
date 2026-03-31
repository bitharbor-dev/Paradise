using Paradise.ApplicationLogic.EventListeners.Base;
using Paradise.Models;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Events;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Core.ApplicationLogic.EventListeners.Base;

/// <summary>
/// Test <see cref="IdentityTokenEventListener{TEvent}"/>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestIdentityTokenEventListener"/> class.
/// </remarks>
/// <param name="serviceProvider">
/// Application root <see cref="IServiceProvider"/>.
/// </param>
public sealed class TestIdentityTokenEventListener(IServiceProvider serviceProvider)
    : IdentityTokenEventListener<TestDomainEvent>(serviceProvider)
{
    #region Public methods
    /// <inheritdoc/>
    public override Task ProcessAsync(TestDomainEvent domainEvent, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <summary>
    /// Exposes the <see cref="IdentityTokenEventListener{TEvent}.ProtectIdentityToken"/>
    /// for tests execution.
    /// </summary>
    /// <param name="identityToken">
    /// The <see cref="IdentityToken"/> to protect.
    /// </param>
    /// <returns>
    /// A <see langword="string"/> value containing the identity token.
    /// </returns>
    public string ExposeProtectIdentityToken(IdentityToken identityToken)
        => ProtectIdentityToken(identityToken);

    /// <summary>
    /// Exposes the <see cref="IdentityTokenEventListener{TEvent}.CreateIdentityTokenLink"/>
    /// for tests execution.
    /// </summary>
    /// <param name="identityToken">
    /// The <see cref="IdentityToken"/> to protect and place withing the link.
    /// </param>
    /// <param name="baseUrl">
    /// Base <see cref="Uri"/>.
    /// </param>
    /// <param name="path">
    /// API action path.
    /// </param>
    /// <param name="queryParameters">
    /// Link query parameters.
    /// </param>
    /// <returns>
    /// <see cref="Uri"/>, which leads to the specified API action
    /// and contains an identity token.
    /// </returns>
    public Uri ExposeCreateIdentityTokenLink(IdentityToken identityToken, Uri? baseUrl, string path,
                                             Dictionary<string, string?>? queryParameters = null)
        => CreateIdentityTokenLink(identityToken, baseUrl, path, queryParameters);
    #endregion
}