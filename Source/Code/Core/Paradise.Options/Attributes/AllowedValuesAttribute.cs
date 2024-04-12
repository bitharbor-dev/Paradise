using System.ComponentModel.DataAnnotations;
using static Paradise.Localization.ExceptionsHandling.ExceptionMessagesProvider;

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
internal sealed class AllowedValuesAttribute<T>(params T?[] values) : ValidationAttribute
{
    #region Fields
    private Type? _equalityComparerType;
    #endregion

    #region Properties
    /// <summary>
    /// Allowed values array.
    /// </summary>
    public IEnumerable<T?> Values { get; } = values ?? [];

    /// <summary>
    /// Equality comparer type.
    /// </summary>
    /// <remarks>
    /// The input value is expected to be a type
    /// which implements the <see cref="IEqualityComparer{T}"/> interface
    /// and have parameterless constructor.
    /// </remarks>
    public Type? EqualityComparerType
    {
        get => _equalityComparerType;
        set
        {
            if (value is not null)
            {
                if (!value.IsAssignableTo(typeof(IEqualityComparer<T>)))
                {
                    var message = GetInvalidEqualityComparerTypeMessage(value);

                    throw new InvalidOperationException(message);
                }

                if (value.GetConstructor(Type.EmptyTypes) is null)
                {
                    var message = GetInvalidEqualityComparerConstructorMessage(value);

                    throw new InvalidOperationException(message);
                }
            }

            _equalityComparerType = value;
        }
    }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (values.Length is 0)
            return true;

        if (value is not T castedValue)
        {
            var message = GetFailedToCastMessage(value?.GetType(), typeof(T));

            throw new InvalidCastException(message);
        }

        IEqualityComparer<T?>? equalityComparer = null;

        if (_equalityComparerType is not null)
        {
            if (Activator.CreateInstance(_equalityComparerType) is IEqualityComparer<T?> comparer)
            {
                equalityComparer = comparer;
            }
            else
            {
                var message = GetFailedToCreateInstanceOfTypeMessage(typeof(IEqualityComparer<T?>));

                throw new InvalidOperationException(message);
            }
        }

        equalityComparer ??= EqualityComparer<T?>.Default;

        return Values.Contains(castedValue, equalityComparer);
    }
    #endregion
}