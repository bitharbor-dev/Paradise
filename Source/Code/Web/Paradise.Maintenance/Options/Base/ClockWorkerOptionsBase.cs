using Paradise.Maintenance.Workers.Base;
using static Paradise.Localization.ExceptionsHandling.ExceptionMessagesProvider;

namespace Paradise.Maintenance.Options.Base;

/// <summary>
/// Provides options for the <see cref="WorkerBase{TOptions}"/> classes.
/// </summary>
internal abstract class ClockWorkerOptionsBase : WorkerOptionsBase
{
    #region Fields
    private static readonly TimeSpan _oneDay = TimeSpan.FromDays(1d);

    private TimeSpan _executionTime;
    #endregion

    #region Properties
    /// <summary>
    /// Action execution time.
    /// </summary>
    public TimeOnly ExecutionTime
    {
        get => TimeOnly.FromTimeSpan(_executionTime);
        set
        {
            _executionTime = value.ToTimeSpan();
            CalculateDelay();
        }
    }

    /// <inheritdoc/>
    public override TimeSpan Interval
    {
        get => base.Interval;
        set
        {
            if (value != _oneDay)
                throw new InvalidOperationException(GetInvalidClockWorkerIntervalMessage());

            base.Interval = value;
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
    /// Calculates the timer delay in order to
    /// execute the bounded action at the specific time of the day.
    /// </summary>
    private void CalculateDelay()
    {
        var currentTime = DateTime.UtcNow.TimeOfDay;

        Delay = currentTime < _executionTime
            ? _executionTime - currentTime
            : _oneDay - currentTime + _executionTime;
    }
    #endregion
}