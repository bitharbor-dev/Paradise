using Paradise.Maintenance.Workers.Base;

namespace Paradise.Maintenance.Options.Base;

/// <summary>
/// Provides options for the <see cref="WorkerBase{TOptions}"/> classes.
/// </summary>
internal abstract class WorkerOptionsBase : IEquatable<WorkerOptionsBase>
{
    #region Properties
    /// <summary>
    /// Action execution delay.
    /// </summary>
    public virtual TimeSpan Delay { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Action execution interval.
    /// </summary>
    public virtual TimeSpan Interval { get; set; } = TimeSpan.FromDays(1d);
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public virtual bool Equals(WorkerOptionsBase? other)
    {
        return other is not null
            && other.GetType() == GetType()
            && other.Delay == Delay
            && other.Interval == Interval;
    }

    /// <inheritdoc/>
    public sealed override bool Equals(object? obj)
        => obj is WorkerOptionsBase options && Equals(options);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Delay, Interval);
    #endregion

    #region Operators
    /// <summary>
    /// Compares the given <paramref name="a"/> and <paramref name="b"/>
    /// objects for equality.
    /// </summary>
    /// <param name="a">
    /// The first <see cref="WorkerOptionsBase"/> to be compared.
    /// </param>
    /// <param name="b">
    /// The second <see cref="WorkerOptionsBase"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="a"/> equals <paramref name="b"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator ==(WorkerOptionsBase? a, WorkerOptionsBase? b)
        => a is not null && a.Equals(b);

    /// <summary>
    /// Compares the given <paramref name="a"/> and <paramref name="b"/>
    /// objects for inequality.
    /// </summary>
    /// <param name="a">
    /// The first <see cref="WorkerOptionsBase"/> to be compared.
    /// </param>
    /// <param name="b">
    /// The second <see cref="WorkerOptionsBase"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="a"/> does not equal <paramref name="b"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator !=(WorkerOptionsBase? a, WorkerOptionsBase? b)
        => !(a == b);
    #endregion
}