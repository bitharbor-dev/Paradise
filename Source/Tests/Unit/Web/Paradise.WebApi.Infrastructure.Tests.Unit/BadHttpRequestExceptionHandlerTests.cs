using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Paradise.Tests.Doubles.Fakes.Microsoft.AspNetCore.Http;
using Paradise.Tests.Doubles.Fakes.Microsoft.AspNetCore.Http.Features;

namespace Paradise.WebApi.Infrastructure.Tests.Unit;

/// <summary>
/// <see cref="BadHttpRequestExceptionHandler"/> test class.
/// </summary>
public sealed class BadHttpRequestExceptionHandlerTests
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="BadHttpRequestExceptionHandlerTests"/> class.
    /// </summary>
    public BadHttpRequestExceptionHandlerTests()
    {
        ProblemDetailsService = new();
        CapturedException = new BadHttpRequestException(string.Empty);

        Context = new DefaultHttpContext();
        Context.Features.Set<IHttpResponseFeature>(new FakeHttpResponseFeature());

        Context.RequestServices = new ServiceCollection()
            .AddSingleton<IProblemDetailsService>(ProblemDetailsService)
            .BuildServiceProvider();

        Handler = new();
    }
    #endregion

    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    public BadHttpRequestExceptionHandler Handler { get; }

    /// <summary>
    /// An accessor to the <see cref="IProblemDetailsService"/> instance
    /// used by the test target or it's dependencies.
    /// </summary>
    public FakeProblemDetailsService ProblemDetailsService { get; }

    /// <summary>
    /// The <see cref="Exception"/> passed into
    /// <see cref="ExceptionHandler.TryHandleAsync"/> method call.
    /// </summary>
    public Exception CapturedException { get; }

    /// <summary>
    /// The <see cref="HttpContext"/> passed into
    /// <see cref="ExceptionHandler.TryHandleAsync"/> method call.
    /// </summary>
    public HttpContext Context { get; }

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="BadHttpRequestExceptionHandler.TryHandleAsync"/> method should
    /// handle the exception and write the response content.
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
    }

    /// <summary>
    /// The <see cref="BadHttpRequestExceptionHandler.TryHandleAsync"/> method should
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
    /// The <see cref="BadHttpRequestExceptionHandler.TryHandleAsync"/> method should
    /// not handle the non-<see cref="BadHttpRequestException"/> exceptions
    /// and return <see langword="false"/>.
    /// </summary>
    [Fact]
    public async Task TryHandleAsync_SkipsBadRequestException()
    {
        // Arrange
        var capturedException = new InvalidOperationException();

        // Act
        var result = await Handler.TryHandleAsync(Context, capturedException, Token);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="BadHttpRequestExceptionHandler.TryHandleAsync"/> method should
    /// not handle the exception and write the response content
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
    }
    #endregion
}