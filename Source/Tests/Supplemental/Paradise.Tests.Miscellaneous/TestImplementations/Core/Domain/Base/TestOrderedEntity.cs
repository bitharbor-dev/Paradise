using Paradise.Domain.Base;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;

/// <summary>
/// Test <see cref="Entity"/> implementation.
/// </summary>
public sealed class TestOrderedEntity : Entity
{
    #region Properties
    /// <summary>
    /// Entity order.
    /// </summary>
    public ushort Order { get; set; }
    #endregion

    #region Public methods
    /// <summary>
    /// Sets the value of <see cref="Entity.Id"/> property.
    /// </summary>
    /// <param name="id">
    /// The value to set.
    /// </param>
    public void SetId(Guid id)
        => Id = id;
    #endregion
}