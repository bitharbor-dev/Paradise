using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Infrastructure.DataProtection;
using Paradise.Domain.Base.Events;
using Paradise.Models;
using static Paradise.Common.Web.ParameterNames;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.ApplicationLogic.EventListeners.Base;

/// <summary>
/// A domain event listener with identity tokens creation capabilities.
/// </summary>
/// <typeparam name="TEvent">
/// The type of domain event this listener handles.
/// </typeparam>
/// <param name="serviceProvider">
/// Application root <see cref="IServiceProvider"/>.
/// </param>
public abstract class IdentityTokenEventListener<TEvent>(IServiceProvider serviceProvider)
    : IDomainEventListener<TEvent> where TEvent : IDomainEvent
{
    #region Public methods
    /// <inheritdoc/>
    public abstract Task ProcessAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    #endregion

    #region Protected methods
    /// <summary>
    /// Protects the given <paramref name="identityToken"/>.
    /// </summary>
    /// <param name="identityToken">
    /// The <see cref="IdentityToken"/> to protect.
    /// </param>
    /// <returns>
    /// A <see langword="string"/> value containing the identity token.
    /// </returns>
    protected string ProtectIdentityToken(IdentityToken identityToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dataProtector = scope.ServiceProvider.GetRequiredService<IDataProtector>();

        return dataProtector.Protect(identityToken);
    }

    /// <summary>
    /// Creates a link with an identity token.
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
    protected Uri CreateIdentityTokenLink(IdentityToken identityToken, Uri? baseUrl, string path,
                                          Dictionary<string, string?>? queryParameters = null)
    {
        ArgumentNullException.ThrowIfNull(baseUrl);
        ArgumentNullException.ThrowIfNull(path);

        using var scope = serviceProvider.CreateScope();
        var dataProtector = scope.ServiceProvider.GetRequiredService<IDataProtector>();

        var placeholder = $"{{{IdentityTokenParameter}}}";

        if (!path.Contains(placeholder, StringComparison.Ordinal))
        {
            var message = GetMessageMissingSubString(path, placeholder);

            throw new ArgumentException(message, nameof(path));
        }

        var identityTokenString = dataProtector.Protect(identityToken);

        path = path.Replace(placeholder, identityTokenString, StringComparison.OrdinalIgnoreCase);

        if (queryParameters is not null)
            path += QueryString.Create(queryParameters).Value;

        return new(baseUrl, path);
    }
    #endregion
}