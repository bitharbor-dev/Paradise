using Paradise.Domain.Base;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;

/// <summary>
/// Test <see cref="ValueObject"/> implementation.
/// </summary>
public sealed class TestValidatableValueObject : ValueObject
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

    /// <inheritdoc/>
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StateValidated;
    }
    #endregion
}