namespace Paradise.Models.Attributes;

/// <summary>
/// An attribute the be used to mark <see cref="ErrorCode"/> entries as critical or not.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="IsCriticalAttribute"/> class.
/// </remarks>
/// <param name="value">
/// Indicates whether the <see cref="ErrorCode"/> is critical.
/// </param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class IsCriticalAttribute(bool value) : Attribute
{
    #region Properties
    /// <summary>
    /// Indicates whether the <see cref="ErrorCode"/> is critical.
    /// </summary>
    public bool Value { get; } = value;
    #endregion
}