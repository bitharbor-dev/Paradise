using Paradise.Domain.Base;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;

/// <summary>
/// Test <see cref="ValueObject"/> implementation.
/// </summary>
public sealed class TestNamedValueObject : ValueObject
{
    #region Properties
    /// <summary>
    /// Value object name.
    /// </summary>
    public string? Name { get; set; }
    #endregion

    #region Public methods
    /// <summary>
    /// Sets the value of <see cref="ValueObject.Id"/> property.
    /// </summary>
    /// <param name="id">
    /// The value to set.
    /// </param>
    public void SetId(Guid id)
        => Id = id;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Id;
        yield return Name;
    }
    #endregion
}