using Paradise.Domain.Base;

namespace Paradise.Tests.Miscellaneous.Fakes.Core.Domain;

/// <summary>
/// Fake <see cref="ValueObject"/> implementation.
/// </summary>
public sealed class FakeValueObject2 : ValueObject
{
    #region Public methods
    /// <inheritdoc/>
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Id;
        yield return Created;
        yield return Modified;
    }
    #endregion
}