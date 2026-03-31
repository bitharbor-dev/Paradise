using Paradise.Domain.Base;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Spies.Core.Domain.Base;

/// <summary>
/// Spy <see cref="Entity"/> implementation.
/// </summary>
public sealed class SpyEntity : Entity
{
    #region Properties
    /// <summary>
    /// Indicates whether the <see cref="ValidateState"/> method was called.
    /// </summary>
    public bool StateValidated { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override void ValidateState()
    {
        StateValidated = true;

        base.ValidateState();
    }
    #endregion
}