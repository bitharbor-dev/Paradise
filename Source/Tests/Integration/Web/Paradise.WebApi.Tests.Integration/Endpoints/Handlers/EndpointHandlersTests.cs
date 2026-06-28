using Paradise.Tests.Extensibility.Web.Hosting;

namespace Paradise.WebApi.Tests.Integration.Endpoints.Handlers;

/// <summary>
/// Base endpoints handlers test class.
/// </summary>
public abstract class EndpointHandlersTests : IAsyncDisposable
{
    #region Fields
    private bool _disposed;
    #endregion

    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    protected DefaultWebApplicationFactory Application { get; } = new();

    /// <summary>
    /// <see cref="Application"/> client.
    /// </summary>
    protected HttpClient Client
        => field ??= Application.CreateClient();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    protected CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);

        _disposed = true;
    }
    #endregion

    #region Protected methods
    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    protected virtual ValueTask DisposeAsyncCore()
        => Application.DisposeAsync();
    #endregion
}