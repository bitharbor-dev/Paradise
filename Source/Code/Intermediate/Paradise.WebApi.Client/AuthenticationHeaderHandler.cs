namespace Paradise.WebApi.Client;

/// <summary>
/// Attaches the access token to the HTTP request.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthenticationHeaderHandler"/> class.
/// </remarks>
/// <param name="provider">
/// Authentication header value provider.
/// </param>
internal sealed class AuthenticationHeaderHandler(IAuthenticationHeaderProvider provider) : DelegatingHandler
{
    #region Properties
    /// <summary>
    /// The key used to set authorize request parameter.
    /// </summary>
    public static HttpRequestOptionsKey<bool> AuthorizeParameterKey { get; } = new("Authorize");
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (IsAuthorizationEnabled(request))
            request.Headers.Authorization = provider.GetHeader();

        return base.Send(request, cancellationToken);
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (IsAuthorizationEnabled(request))
        {
            request.Headers.Authorization = await provider.GetHeaderAsync()
                .ConfigureAwait(false);
        }

        return await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Determines whether the authorization header
    /// should be attached to the given <paramref name="httpRequest"/>.
    /// </summary>
    /// <param name="httpRequest">
    /// The HTTP request message being processed.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the authorization header
    /// should be attached to the given <paramref name="httpRequest"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    private static bool IsAuthorizationEnabled(HttpRequestMessage httpRequest)
        => httpRequest.Options.TryGetValue(AuthorizeParameterKey, out var authorize) && authorize;
    #endregion
}