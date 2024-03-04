using Paradise.Maintenance.Options.Base;

namespace Paradise.Maintenance.Workers.Base;

/// <summary>
/// <see cref="WorkerBase{TOptions}.OptionsChanged"/> event arguments.
/// </summary>
/// <typeparam name="TOptions">
/// Worker options type.
/// </typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="WorkerOptionsChangedEventArgs{TOptions}"/> class.
/// </remarks>
/// <param name="oldValue">
/// Old value.
/// </param>
/// <param name="newValue">
/// New value.
/// </param>
internal sealed class WorkerOptionsChangedEventArgs<TOptions>(TOptions oldValue, TOptions newValue) : EventArgs
    where TOptions : WorkerOptionsBase
{
    #region Properties
    /// <summary>
    /// Old value.
    /// </summary>
    public TOptions OldValue { get; } = oldValue;

    /// <summary>
    /// New value.
    /// </summary>
    public TOptions NewValue { get; } = newValue;
    #endregion
}