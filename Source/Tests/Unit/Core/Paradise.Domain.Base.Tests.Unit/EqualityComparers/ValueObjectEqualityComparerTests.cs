using Paradise.Domain.Base.EqualityComparers;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.Tests.Unit.EqualityComparers;

/// <summary>
/// <see cref="ValueObjectEqualityComparer"/> test class.
/// </summary>
public sealed class ValueObjectEqualityComparerTests
{
    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    public ValueObjectEqualityComparer Comparer { get; } = new();

    /// <summary>
    /// Predefined GUID value to be used for data arrangement.
    /// </summary>
    public static Guid LeftId { get; } = Guid.Parse("0198610a-ac67-7bf0-8d08-676de1492235");

    /// <summary>
    /// Predefined GUID value to be used for data arrangement.
    /// </summary>
    public static Guid RightId { get; } = Guid.Parse("0198610a-ac67-7bf0-8d08-676de1492236");

    /// <summary>
    /// Provides member data for <see cref="Equals_ReturnsFalseOnNull"/> method.
    /// </summary>
    public static TheoryData<Guid?, Guid?> Equals_ReturnsFalseOnNull_MemberData { get; } = new()
    {
        { LeftId,   null    },
        { null,     RightId }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="ValueObjectEqualityComparer.Equals"/> method should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same type but different <see cref="ValueObject.GetEqualityComponents"/> return values.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualComponents()
    {
        // Arrange
        var left = GetNamedValueObject(LeftId);
        var right = GetNamedValueObject(RightId);

        // Act
        var result = Comparer.Equals(left, right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="ValueObjectEqualityComparer.Equals"/> method should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same <see cref="ValueObject.GetEqualityComponents"/> return values but different types.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualType()
    {
        // Arrange
        var left = GetNamedValueObject(LeftId);
        var right = GetOrderedValueObject(LeftId);

        // Act
        var result = Comparer.Equals(left, right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="ValueObjectEqualityComparer.Equals"/> method should
    /// return <see langword="false"/> if one of the instances being compared
    /// is <see langword="null"/>.
    /// </summary>
    /// <param name="leftId">
    /// A GUID value used to instantiate left operand.
    /// </param>
    /// <param name="rightId">
    /// A GUID value used to instantiate right operand.
    /// </param>
    [Theory, MemberData(nameof(Equals_ReturnsFalseOnNull_MemberData))]
    public void Equals_ReturnsFalseOnNull(Guid? leftId, Guid? rightId)
    {
        // Arrange
        var left = GetNamedValueObject(leftId);
        var right = GetNamedValueObject(rightId);

        // Act
        var result = Comparer.Equals(left, right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="ValueObjectEqualityComparer.Equals"/> method should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same <see cref="ValueObject.GetEqualityComponents"/> return values and type.
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueOnEqualComponentsAndType()
    {
        // Arrange
        var left = GetNamedValueObject(LeftId);
        var right = GetNamedValueObject(LeftId);

        // Act
        var result = Comparer.Equals(left, right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="ValueObjectEqualityComparer.Equals"/> method should
    /// return <see langword="true"/> if both of the instances being compared
    /// are <see langword="null"/>.
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueOnNullArguments()
    {
        // Arrange
        var left = GetNamedValueObject(null);
        var right = GetNamedValueObject(null);

        // Act
        var result = Comparer.Equals(left, right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="ValueObjectEqualityComparer.GetHashCode"/> method should
    /// return the same values if both of the instances being compared
    /// have the same <see cref="ValueObject.GetEqualityComponents"/> return values and type.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsEqualValuesOnEqualComponentsAndType()
    {
        // Arrange
        var left = GetNamedValueObject(LeftId);
        var right = GetNamedValueObject(LeftId);

        // Act
        var leftHash = Comparer.GetHashCode(left);
        var rightHash = Comparer.GetHashCode(right);

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="ValueObjectEqualityComparer.GetHashCode"/> method should
    /// return different values if both of the instances being compared
    /// have the same type but different <see cref="ValueObject.GetEqualityComponents"/> return values.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnNonEqualComponents()
    {
        // Arrange
        var left = GetNamedValueObject(LeftId);
        var right = GetNamedValueObject(RightId);

        // Act
        var leftHash = Comparer.GetHashCode(left);
        var rightHash = Comparer.GetHashCode(right);

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="ValueObjectEqualityComparer.GetHashCode"/> method should
    /// return different values if both of the instances being compared
    /// have the same <see cref="ValueObject.GetEqualityComponents"/> return values but different type.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnNonEqualType()
    {
        // Arrange
        var left = GetNamedValueObject(LeftId);
        var right = GetOrderedValueObject(LeftId);

        // Act
        var leftHash = Comparer.GetHashCode(left);
        var rightHash = Comparer.GetHashCode(right);

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="ValueObjectEqualityComparer.GetHashCode"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="ValueObject"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void GetHashCode_ThrowsOnNull()
    {
        // Arrange
        var valueObject = GetNamedValueObject(null);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Comparer.GetHashCode(valueObject!));
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates an instance of <see cref="TestNamedValueObject"/> initialized
    /// with the provided identifier,
    /// or returns <see langword="null"/> when the identifier is not specified.
    /// </summary>
    /// <param name="id">
    /// Identifier used to construct the value object instance.
    /// </param>
    /// <returns>
    /// A new <see cref="TestNamedValueObject"/> when <paramref name="id"/> has a value,
    /// otherwise - <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(id))]
    private static TestNamedValueObject? GetNamedValueObject(Guid? id)
    {
        if (!id.HasValue)
            return null;

        var entity = new TestNamedValueObject();
        entity.SetId(id.Value);

        return entity;
    }

    /// <summary>
    /// Creates an instance of <see cref="TestOrderedValueObject"/> initialized
    /// with the provided identifier,
    /// or returns <see langword="null"/> when the identifier is not specified.
    /// </summary>
    /// <param name="id">
    /// Identifier used to construct the value object instance.
    /// </param>
    /// <returns>
    /// A new <see cref="TestOrderedValueObject"/> when <paramref name="id"/> has a value,
    /// otherwise - <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(id))]
    private static TestOrderedValueObject? GetOrderedValueObject(Guid? id)
    {
        if (!id.HasValue)
            return null;

        var entity = new TestOrderedValueObject();
        entity.SetId(id.Value);

        return entity;
    }
    #endregion
}