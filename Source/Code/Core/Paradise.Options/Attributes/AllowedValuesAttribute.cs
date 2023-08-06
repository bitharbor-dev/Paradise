using System.ComponentModel.DataAnnotations;

namespace Paradise.Options.Attributes;

/// <summary>
/// Provides disallowed values validation functionalities.
/// </summary>
/// <typeparam name="T">
/// Values type.
/// </typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="AllowedValuesAttribute{T}"/> class.
/// </remarks>
/// <param name="values">
/// Allowed values array.
/// </param>
internal sealed class AllowedValuesAttribute<T>(params T[] values) : ValidationAttribute
{
    #region Properties
    /// <summary>
    /// Allowed values array.
    /// </summary>
    public T[] Values { get; } = values ?? Array.Empty<T>();

    /// <summary>
    /// Equality comparer type.
    /// </summary>
    public Type? EqualityComparerType { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (values.Length is 0)
            return true;

        if (value is not T castedValue)
            throw new InvalidCastException(); // TODO: Provide proper exception message.

        IEqualityComparer<T>? equalityComparer = null;

        if (EqualityComparerType is not null)
        {
            if (Activator.CreateInstance(EqualityComparerType) is not IEqualityComparer<T> comparer)
                throw new ArgumentException(nameof(EqualityComparerType)); // TODO: Provide proper exception message.

            equalityComparer = comparer;
        }

        equalityComparer ??= EqualityComparer<T>.Default;

        return Values.Contains(castedValue, equalityComparer);
    }
    #endregion
}