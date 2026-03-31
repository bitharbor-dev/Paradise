using Paradise.Domain.Base;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;

/// <summary>
/// Test <see cref="Entity"/> implementation.
/// </summary>
public sealed class TestRelationalEntity : Entity
{
    #region Properties
    /// <summary>
    /// Child value object Id.
    /// </summary>
    public Guid ChildId { get; set; }

    /// <summary>
    /// Child value object.
    /// </summary>
    public TestRelationalValueObject? Child { get; set; }
    #endregion
}