using Paradise.WebApi.Startup;

namespace Paradise.Tests.Doubles.Spies.Web.WebApi.Startup;

/// <summary>
/// Spy <see cref="IPostBuildStep"/> implementation.
/// </summary>
internal class SpyPostBuildStep : IPostBuildStep
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
    public Action<SpyPostBuildStep>? ExecutedCallback { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public ValueTask ExecuteAsync(PostBuildContext context)
    {
        Executed = true;

        ExecutedCallback?.Invoke(this);

        return ValueTask.CompletedTask;
    }
    #endregion
}