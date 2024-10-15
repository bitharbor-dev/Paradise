using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Paradise.WebApi.Client.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IServiceCollection"/> <see langword="interface"/>.
/// </summary>
public static class IServiceCollectionExtensions
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
    /// Target <see cref="IHttpClientBuilder"/> instance.
    /// </param>
    /// <returns>
    /// The <see cref="IHttpClientBuilder"/> instance.
    /// </returns>
    public static IHttpClientBuilder AddAuthenticationHeaderProvider<TProvider>(this IHttpClientBuilder builder)
        where TProvider : class, IAuthenticationHeaderProvider
    {
        builder.AddHttpMessageHandler<AuthenticationHeaderHandler>();

        builder.Services.TryAddScoped<AuthenticationHeaderHandler>();
        builder.Services.TryAddScoped<IAuthenticationHeaderProvider, TProvider>();

        return builder;
    }
    #endregion
}