using Paradise.Maintenance.Workers.Base;

namespace Paradise.Maintenance.Options.Base;

/// <summary>
/// Provides options for the <see cref="WorkerBase{TOptions}"/> classes.
/// </summary>
internal abstract class ClockWorkerOptionsBase : WorkerOptionsBase
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
        var oneDay = TimeSpan.FromDays(1d);
        var oneDayTicks = oneDay.Ticks;
        var currentTimeTicks = DateTime.UtcNow.TimeOfDay.Ticks;
        var executionTimeTicks = _executionTime.Ticks;

        Delay = TimeSpan.FromTicks(
            currentTimeTicks < executionTimeTicks
                ? executionTimeTicks - currentTimeTicks
                : oneDayTicks - currentTimeTicks + executionTimeTicks);

        Interval = oneDay;
    }
    #endregion
}