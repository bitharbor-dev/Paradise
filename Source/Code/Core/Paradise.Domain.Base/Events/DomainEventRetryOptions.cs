using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.Events;

/// <summary>
/// Represents the configuration options for retrying the processing
/// of domain events in case of failure.
/// </summary>
public sealed class DomainEventRetryOptions
{
    #region Properties
    /// <summary>
    /// Maximum number of retry attempts allowed
    /// for a single domain event listener.
    /// </summary>
    /// <remarks>
    /// A value of <c>0</c> disables retry attempts.
    /// </remarks>
    public ushort MaxRetries { get; set; } = 3;

    /// <summary>
    /// Base delay between retry attempts.
    /// </summary>
    /// <remarks>
    /// If <see cref="UseExponentialBackoff"/> is <see langword="true"/>, the delay
    /// will increase exponentially with each retry attempt.
    /// </remarks>
    public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Indicates whether exponential back-off should be applied to retry delays.
    /// </summary>
    /// <remarks>
    /// When enabled, the actual delay for each retry attempt will be
    /// calculated as <c>BaseDelay × 2^retryCount</c>.
    /// </remarks>
    public bool UseExponentialBackoff { get; set; } = true;
    #endregion

    #region Public methods
    /// <summary>
    /// Creates a cancellable task that completes after delay
    /// calculated from the given <paramref name="attempt"/>.
    /// </summary>
    /// <param name="attempt">
    /// The zero-based attempt number. 0 is the initial attempt, retries start from 1.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A Task that represents the time delay.
    /// </returns>
    public Task DelayAsync(ushort attempt, CancellationToken cancellationToken)
    {
        var delay = GetDelay(attempt);

        Delaying?.Invoke(this, new(delay));

        return Task.Delay(delay, cancellationToken);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Calculates the delay before the next retry attempt based on the current attempt number,
    /// where attempt 0 represents the initial (non-retried) dispatch.
    /// </summary>
    /// <param name="attempt">
    /// The zero-based attempt number. 0 is the initial attempt, retries start from 1.
    /// </param>
    /// <returns>
    /// A <see cref="TimeSpan"/> representing the delay to wait before the next retry.
    /// Returns <see cref="TimeSpan.Zero"/> for the initial attempt (attempt 0).
    /// </returns>
    [SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Omitted for readability.")]
    private TimeSpan GetDelay(ushort attempt)
    {
        if (attempt is 0)
            return TimeSpan.Zero;

        if (UseExponentialBackoff)
            return BaseDelay * Math.Pow(2, attempt);

        return BaseDelay;
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when <see cref="DelayAsync"/> is invoked.
    /// </summary>
    public event EventHandler<DelayEventArgs>? Delaying;
    #endregion
}