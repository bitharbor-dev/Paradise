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
    #region Protected methods
    /// <inheritdoc/>
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = provider.GetHeader();

        return base.Send(request, cancellationToken);
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = await provider.GetHeaderAsync()
            .ConfigureAwait(false);

        return await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
    #endregion
}