using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Paradise.Tests.Doubles.Fakes.Microsoft.AspNetCore.Http;
using Paradise.Tests.Doubles.Fakes.Microsoft.AspNetCore.Http.Features;
using Paradise.Tests.Doubles.Fakes.Microsoft.Extensions.Logging;

namespace Paradise.WebApi.Infrastructure.Tests.Unit;

/// <summary>
/// <see cref="ExceptionHandler"/> test class.
/// </summary>
public sealed class ExceptionHandlerTests : IDisposable
{
    #region Fields
    private readonly List<MessageLoggedEventArgs> _loggedMessages = [];

    private readonly FakeLogger<ExceptionHandler> _logger = new();
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlerTests"/> class.
    /// </summary>
    public ExceptionHandlerTests()
    {
        _logger.MessageLogged += OnMessageLogged;

        ProblemDetailsService = new();

        Context = new DefaultHttpContext();
        Context.Features.Set<IHttpResponseFeature>(new FakeHttpResponseFeature());

        Context.RequestServices = new ServiceCollection()
            .AddSingleton<IProblemDetailsService>(ProblemDetailsService)
            .BuildServiceProvider();

        Handler = new(_logger);
    }
    #endregion

    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    public ExceptionHandler Handler { get; }

    /// <summary>
    /// An accessor to the <see cref="IProblemDetailsService"/> instance
    /// used by the test target or it's dependencies.
    /// </summary>
    public FakeProblemDetailsService ProblemDetailsService { get; }

    /// <summary>
    /// The <see cref="Exception"/> passed into
    /// <see cref="ExceptionHandler.TryHandleAsync"/> method call.
    /// </summary>
    public Exception CapturedException { get; } = new InvalidOperationException();

    /// <summary>
    /// The <see cref="HttpContext"/> passed into
    /// <see cref="ExceptionHandler.TryHandleAsync"/> method call.
    /// </summary>
    public HttpContext Context { get; }

    /// <summary>
    /// Contains messages which were logged during tests.
    /// </summary>
    internal IEnumerable<MessageLoggedEventArgs> LoggedMessages
        => _loggedMessages.AsReadOnly();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => _logger.MessageLogged -= OnMessageLogged;

    /// <summary>
    /// The <see cref="ExceptionHandler.TryHandleAsync"/> method should
    /// handle the exception, log it, and write the response content.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync()
    {
        // Arrange
        ProblemDetailsService.Result = true;

        // Act
        var result = await Handler.TryHandleAsync(Context, CapturedException, Token);

        // Assert
        Assert.True(result);

        var entry = Assert.Single(LoggedMessages);
        Assert.Same(CapturedException, entry.Exception);
        Assert.Same(CapturedException, ProblemDetailsService.DetailsContext?.Exception);
    }

    /// <summary>
    /// The <see cref="ExceptionHandler.TryHandleAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="HttpContext"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_ThrowsOnNull()
    {
        // Arrange
        var context = null as HttpContext;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => Handler.TryHandleAsync(context!, CapturedException, Token).AsTask());
    }

    /// <summary>
    /// The <see cref="ExceptionHandler.TryHandleAsync"/> method should
    /// not handle the exception, log it, and write the response content
    /// if the response has already started.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_SkipsStartedResponse()
    {
        // Arrange
        Context.Response.OnStarting(() => Task.CompletedTask);

        // Act
        var result = await Handler.TryHandleAsync(Context, CapturedException, Token);

        // Assert
        Assert.False(result);

        var entry = Assert.Single(LoggedMessages);
        Assert.Same(CapturedException, entry.Exception);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// The <see cref="FakeLogger{T}.MessageLogged"/> event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="MessageLoggedEventArgs"/> instance containing the event data.
    /// </param>
    private void OnMessageLogged(object? sender, MessageLoggedEventArgs e)
         => _loggedMessages.Add(e);
    #endregion
}