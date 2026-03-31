using Paradise.Domain.Base;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;

/// <summary>
/// Test <see cref="ValueObject"/> implementation.
/// </summary>
public sealed class TestRelationalValueObject : ValueObject
{
    #region Properties
    /// <summary>
    /// Parent entity Id.
    /// </summary>
    public Guid ParentId { get; set; }

    /// <summary>
    /// Parent entity.
    /// </summary>
    public TestRelationalEntity? Parent { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Id;
        yield return ParentId;
    }
    #endregion
}