using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.AspNetCore.Http.Features;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;
using System.Text.Json;
using OptionsBuilder = Microsoft.Extensions.Options.Options;

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

        Context = new DefaultHttpContext();
        Context.Features.Set<IHttpResponseFeature>(new FakeHttpResponseFeature());

        Handler = new(_logger, OptionsBuilder.Create(JsonSerializerOptions.Default));
    }
    #endregion

    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    public ExceptionHandler Handler { get; }

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

        // Act
        var result = await Handler.TryHandleAsync(Context, CapturedException, Token);

        // Assert
        Assert.True(result);

        var entry = Assert.Single(LoggedMessages);
        Assert.Same(CapturedException, entry.Exception);
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

    /// <summary>
    /// The <see cref="ExceptionHandler.HandleFallbackAsync"/> method should
    /// log fallback invocation and write the default failure response.
    /// </summary>
    [Fact]
    public async Task HandleFallbackAsync()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddSingleton<ILogger<ExceptionHandler>>(_logger);

        services.AddSingleton(OptionsBuilder.Create(JsonSerializerOptions.Default));

        Context.RequestServices = services.BuildServiceProvider();

        // Act
        await ExceptionHandler.HandleFallbackAsync(Context);

        // Assert
        Assert.Single(LoggedMessages);
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