using Paradise.WebApi.Startup;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Spies.Web.WebApi.Startup;

/// <summary>
/// Spy <see cref="IPreBuildStep"/> implementation.
/// </summary>
internal class SpyPreBuildStep : IPreBuildStep
{
    #region Properties
    /// <summary>
    /// Step Id.
    /// </summary>
    public Guid Id { get; } = Guid.CreateVersion7();

    /// <summary>
    /// Indicates whether the step was executed.
    /// </summary>
    public bool Executed { get; private set; }

    /// <summary>
    /// An action invoked in the end of <see cref="ExecuteAsync"/> method.
    /// </summary>
    public Action<SpyPreBuildStep>? ExecutedCallback { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task ExecuteAsync(PreBuildContext context)
    {
        Executed = true;

        ExecutedCallback?.Invoke(this);

        return Task.CompletedTask;
    }
    #endregion
}