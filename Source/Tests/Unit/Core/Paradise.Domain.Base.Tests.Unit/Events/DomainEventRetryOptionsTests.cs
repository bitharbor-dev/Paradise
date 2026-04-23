using Paradise.Domain.Base.Events;

namespace Paradise.Domain.Base.Tests.Unit.Events;

/// <summary>
/// <see cref="DomainEventRetryOptions"/> test class.
/// </summary>
public sealed class DomainEventRetryOptionsTests
{
    #region Properties
    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;

    /// <summary>
    /// Provides member data for <see cref="DelayAsync_Exponential"/> method.
    /// </summary>
    public static TheoryData<ushort, TimeSpan> DelayAsync_Exponential_MemberData { get; } = new()
    {
        { 0,    TimeSpan.FromTicks(0)   },
        { 1,    TimeSpan.FromTicks(2)   },
        { 2,    TimeSpan.FromTicks(4)   },
        { 3,    TimeSpan.FromTicks(8)   },
    };

    /// <summary>
    /// Provides member data for <see cref="DelayAsync_NonExponential"/> method.
    /// </summary>
    public static TheoryData<ushort, TimeSpan> DelayAsync_NonExponential_MemberData { get; } = new()
    {
        { 0,    TimeSpan.FromTicks(0)   },
        { 1,    TimeSpan.FromTicks(1)   },
        { 2,    TimeSpan.FromTicks(1)   },
        { 3,    TimeSpan.FromTicks(1)   },
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="DomainEventRetryOptions"/> constructor should
    /// set the default values of the instance properties.
    /// </summary>
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange
        ushort defaultMaxRetries = 3;
        var defaultBaseDelay = TimeSpan.FromSeconds(2);
        var defaultUseExponentialBackoff = true;

        // Act
        var options = new DomainEventRetryOptions();

        // Assert
        Assert.Equal(defaultMaxRetries, options.MaxRetries);
        Assert.Equal(defaultBaseDelay, options.BaseDelay);
        Assert.Equal(defaultUseExponentialBackoff, options.UseExponentialBackOff);
    }

    /// <summary>
    /// The <see cref="DomainEventRetryOptions.DelayAsync"/> method should
    /// delay exponentially depending on the input <paramref name="attempt"/>.
    /// </summary>
    /// <param name="attempt">
    /// The zero-based attempt number. 0 is the initial attempt, retries start from 1.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(DelayAsync_Exponential_MemberData))]
    public async Task DelayAsync_Exponential(ushort attempt, TimeSpan expectedResult)
    {
        // Arrange
        var delay = null as TimeSpan?;

        var options = new DomainEventRetryOptions
        {
            BaseDelay = TimeSpan.FromTicks(1),
            UseExponentialBackOff = true
        };

        options.Delaying += (s, e) => delay = e.Delay;

        // Act
        await options.DelayAsync(attempt, Token);

        // Assert
        Assert.NotNull(delay);
        Assert.Equal(expectedResult, delay.Value);
    }

    /// <summary>
    /// The <see cref="DomainEventRetryOptions.DelayAsync"/> method should
    /// delay non-exponentially depending on the input <paramref name="attempt"/>.
    /// </summary>
    /// <param name="attempt">
    /// The zero-based attempt number. 0 is the initial attempt, retries start from 1.
    /// </param>
    /// <param name="expectedResult">
    /// Expected result.
    /// </param>
    [Theory, MemberData(nameof(DelayAsync_NonExponential_MemberData))]
    public async Task DelayAsync_NonExponential(ushort attempt, TimeSpan expectedResult)
    {
        // Arrange
        var delay = null as TimeSpan?;

        var options = new DomainEventRetryOptions
        {
            BaseDelay = TimeSpan.FromTicks(1),
            UseExponentialBackOff = false
        };

        options.Delaying += (s, e) => delay = e.Delay;

        // Act
        await options.DelayAsync(attempt, Token);

        // Assert
        Assert.NotNull(delay);
        Assert.Equal(expectedResult, delay.Value);
    }

    /// <summary>
    /// The <see cref="DomainEventRetryOptions.DelayAsync"/> method should
    /// invoke the <see cref="DomainEventRetryOptions.Delaying"/> event.
    /// </summary>
    [Fact]
    public async Task DelayAsync_InvokesDelayingEvent()
    {
        // Arrange
        var invoked = false;

        var options = new DomainEventRetryOptions();

        options.Delaying += (s, e) => invoked = true;

        // Act
        await options.DelayAsync(0, Token);

        // Assert
        Assert.True(invoked);
    }

    /// <summary>
    /// The <see cref="DomainEventRetryOptions.DelayAsync"/> method should
    /// throw the <see cref="TaskCanceledException"/> if the input
    /// <see cref="CancellationToken"/> signals operation cancellation.
    /// </summary>
    [Fact]
    public async Task DelayAsync_ThrowsOnCancellation()
    {
        // Arrange
        var options = new DomainEventRetryOptions();

        using var tokenSource = new CancellationTokenSource();
        await tokenSource.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(()
            => options.DelayAsync(1, tokenSource.Token));
    }
    #endregion
}