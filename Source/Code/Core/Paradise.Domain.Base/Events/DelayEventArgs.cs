namespace Paradise.Domain.Base.Events;

/// <summary>
/// Contains the <see cref="DomainEventRetryOptions.Delaying"/> event data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DelayEventArgs"/> class.
/// </remarks>
/// <param name="delay">
/// A <see cref="TimeSpan"/> representing the delay to wait before the next retry.
/// </param>
public sealed class DelayEventArgs(TimeSpan delay) : EventArgs
{
    #region Properties
    /// <summary>
    /// A <see cref="TimeSpan"/> representing the delay to wait before the next retry.
    /// </summary>
    public TimeSpan Delay { get; } = delay;
    #endregion
}