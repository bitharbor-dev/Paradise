namespace Paradise.Domain.Base;

/// <summary>
/// Provides default properties and/or methods for all value objects.
/// </summary>
public interface IValueObject : IDomainObject
{
    #region Methods
    /// <summary>
    /// Gets an array of <see cref="IValueObject"/>
    /// properties to determine its equality compared to the other objects
    /// of the same type.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="object"/>
    /// to determine <see cref="IValueObject"/> equality.
    /// </returns>
    IEnumerable<object?> GetEqualityComponents();
    #endregion
}