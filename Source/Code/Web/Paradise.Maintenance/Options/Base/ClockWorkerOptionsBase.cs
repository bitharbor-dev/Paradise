using Paradise.Maintenance.Workers.Base;

namespace Paradise.Maintenance.Options.Base;

/// <summary>
/// Provides options for the <see cref="WorkerBase{TOptions}"/> classes.
/// </summary>
internal class ClockWorkerOptionsBase : WorkerOptionsBase
{
    #region Fields
    private TimeOnly _executionTime;
    #endregion

    #region Properties
    /// <summary>
    /// Action execution time.
    /// </summary>
    public TimeOnly ExecutionTime
    {
        get => _executionTime;
        set
        {
            _executionTime = value;
            CalculateDelayAndInterval();
        }
    }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override bool Equals(WorkerOptionsBase? other)
    {
        return other is ClockWorkerOptionsBase clockWorkerOptions
            && clockWorkerOptions.GetType() == GetType()
            && clockWorkerOptions.ExecutionTime == ExecutionTime;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => ExecutionTime.GetHashCode();
    #endregion

    #region Private methods
    /// <summary>
    /// Calculates the timer delay and interval in order to
    /// execute the bounded action at the specific time of the day.
    /// </summary>
    private void CalculateDelayAndInterval()
    {
        var delta = DateTime.UtcNow.TimeOfDay.Ticks - _executionTime.Ticks;

        Delay = TimeSpan.FromTicks(Math.Abs(delta));
        Interval = TimeSpan.FromDays(1d) - Delay;
    }
    #endregion
}