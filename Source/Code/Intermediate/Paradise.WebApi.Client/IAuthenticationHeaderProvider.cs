using System.Net.Http.Headers;

namespace Paradise.WebApi.Client;

/// <summary>
/// Provides authentication header to perform authorized API calls.
/// </summary>
public interface IAuthenticationHeaderProvider
{
    #region Properties
    /// <summary>
    /// Default authentication scheme.
    /// </summary>
    string DefaultScheme { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Gets the authentication header.
    /// </summary>
    /// <returns>
    /// A new instance of the <see cref="AuthenticationHeaderValue"/> class.
    /// </returns>
    AuthenticationHeaderValue? GetHeader();

    /// <summary>
    /// Gets the authentication header.
    /// </summary>
    /// <returns>
    /// A new instance of the <see cref="AuthenticationHeaderValue"/> class.
    /// </returns>
    Task<AuthenticationHeaderValue?> GetHeaderAsync();
    #endregion
}