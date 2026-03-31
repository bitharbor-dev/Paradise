using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Paradise.WebApi.Client.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IHttpClientBuilder"/> <see langword="interface"/>.
/// </summary>
public static class IHttpClientBuilderExtensions
{
    #region Public methods
    /// <summary>
    /// Adds the implementation of <see cref="IAuthenticationHeaderProvider"/>
    /// to be invoked during HTTP requests.
    /// </summary>
    /// <typeparam name="TProvider">
    /// Provider type.
    /// </typeparam>
    /// <param name="builder">
    /// The <see cref="IHttpClientBuilder"/>.
    /// </param>
    /// <returns>
    /// An <see cref="IHttpClientBuilder"/> that can be used to configure the client.
    /// </returns>
    public static IHttpClientBuilder AddAuthenticationHandlerAndHeaderProvider<TProvider>(this IHttpClientBuilder builder)
        where TProvider : class, IAuthenticationHeaderProvider
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.TryAddScoped<AuthenticationHeaderHandler>();
        builder.Services.TryAddScoped<IAuthenticationHeaderProvider, TProvider>();

        return builder.AddHttpMessageHandler<AuthenticationHeaderHandler>();
    }
    #endregion
}