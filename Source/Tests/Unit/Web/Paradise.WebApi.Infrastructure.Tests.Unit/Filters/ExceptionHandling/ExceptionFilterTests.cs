using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;
using Paradise.WebApi.Infrastructure.Filters.ExceptionHandling;

namespace Paradise.WebApi.Infrastructure.Tests.Unit.Filters.ExceptionHandling;

/// <summary>
/// <see cref="ExceptionFilter"/> test class.
/// </summary>
public sealed class ExceptionFilterTests : IDisposable
{
    #region Fields
    private readonly List<MessageLoggedEventArgs> _loggedMessages = [];

    private readonly FakeLogger<ExceptionFilter> _logger = new();
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionFilterTests"/> class.
    /// </summary>
    public ExceptionFilterTests()
    {
        _logger.MessageLogged += OnMessageLogged;

        var services = new ServiceCollection()
            .AddSingleton<ILogger<ExceptionFilter>>(_logger)
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = services };

        var actionContext = new ActionContext(httpContext, new(), new());

        Context = new(actionContext, [])
        {
            Exception = CapturedException
        };
    }
    #endregion

    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    public ExceptionFilter Filter { get; } = ExceptionFilter.Instance;

    /// <summary>
    /// The <see cref="Exception"/> assigned to the input
    /// <see cref="ExceptionContext"/> instance.
    /// </summary>
    public Exception CapturedException { get; } = new InvalidOperationException();

    /// <summary>
    /// The <see cref="ExceptionContext"/> passed into
    /// <see cref="ExceptionFilter.OnException"/> method call.
    /// </summary>
    public ExceptionContext Context { get; }

    /// <summary>
    /// Contains messages which were logged during tests.
    /// </summary>
    internal IEnumerable<MessageLoggedEventArgs> LoggedMessages
        => _loggedMessages.AsReadOnly();
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => _logger.MessageLogged -= OnMessageLogged;

    /// <summary>
    /// The <see cref="ExceptionFilter.OnException"/> method should
    /// handle the exception, log it, and produce an <see cref="IActionResult"/>.
    /// </summary>
    [Fact]
    public void OnException()
    {
        // Arrange

        // Act
        Filter.OnException(Context);

        // Assert
        Assert.True(Context.ExceptionHandled);
        Assert.NotNull(Context.Result);

        var message = Assert.Single(LoggedMessages);
        Assert.Same(CapturedException, message.Exception);
    }

    /// <summary>
    /// The <see cref="ExceptionFilter.OnException"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="ExceptionContext"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void OnException_ThrowsOnNull()
    {
        // Arrange
        var context = null as ExceptionContext;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Filter.OnException(context!));
    }

    /// <summary>
    /// The <see cref="ExceptionFilter.Order"/> property should
    /// return <see cref="int.MaxValue"/> minus ten.
    /// </summary>
    [Fact]
    public void Order()
    {
        // Arrange

        // Act
        var order = Filter.Order;

        // Assert
        Assert.Equal(int.MaxValue - 10, order);
    }

    /// <summary>
    /// The <see cref="ExceptionFilter.Instance"/> property should
    /// always return the same instance.
    /// </summary>
    [Fact]
    public void Instance_ReturnsSingleton()
    {
        // Arrange
        var staticFilter = ExceptionFilter.Instance;

        // Act

        // Assert
        Assert.Same(staticFilter, Filter);
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