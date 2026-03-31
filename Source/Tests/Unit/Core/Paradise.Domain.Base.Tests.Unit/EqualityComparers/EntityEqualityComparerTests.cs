using Paradise.Domain.Base.EqualityComparers;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.Tests.Unit.EqualityComparers;

/// <summary>
/// <see cref="EntityEqualityComparer"/> test class.
/// </summary>
public sealed class EntityEqualityComparerTests
{
    #region Properties
    /// <summary>
    /// System under test.
    /// </summary>
    public EntityEqualityComparer Comparer { get; } = new();

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
    /// The <see cref="EntityEqualityComparer.Equals"/> method should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same type but different <see cref="Entity.Id"/>.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualId()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(RightId);

        // Act
        var result = Comparer.Equals(left, right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="EntityEqualityComparer.Equals"/> method should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> but different types.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualType()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetOrderedEntity(LeftId);

        // Act
        var result = Comparer.Equals(left, right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="EntityEqualityComparer.Equals"/> method should
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
        var left = GetNamedEntity(leftId);
        var right = GetNamedEntity(rightId);

        // Act
        var result = Comparer.Equals(left, right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="EntityEqualityComparer.Equals"/> method should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> and type.
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueOnEqualIdAndType()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(LeftId);

        // Act
        var result = Comparer.Equals(left, right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="EntityEqualityComparer.Equals"/> method should
    /// return <see langword="true"/> if both of the instances being compared
    /// are <see langword="null"/>.
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueOnNullArguments()
    {
        // Arrange
        var left = GetNamedEntity(null);
        var right = GetNamedEntity(null);

        // Act
        var result = Comparer.Equals(left, right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="EntityEqualityComparer.GetHashCode"/> method should
    /// return the same values if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> and type.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsEqualValuesOnEqualIdAndType()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(LeftId);

        // Act
        var leftHash = Comparer.GetHashCode(left);
        var rightHash = Comparer.GetHashCode(right);

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="EntityEqualityComparer.GetHashCode"/> method should
    /// return different values if both of the instances being compared
    /// have the same type but different <see cref="Entity.Id"/>.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnNonEqualId()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(RightId);

        // Act
        var leftHash = Comparer.GetHashCode(left);
        var rightHash = Comparer.GetHashCode(right);

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="EntityEqualityComparer.GetHashCode"/> method should
    /// return different values if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> but different type.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnNonEqualType()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetOrderedEntity(LeftId);

        // Act
        var leftHash = Comparer.GetHashCode(left);
        var rightHash = Comparer.GetHashCode(right);

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="EntityEqualityComparer.GetHashCode"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="Entity"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void GetHashCode_ThrowsOnNull()
    {
        // Arrange
        var entity = GetNamedEntity(null);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(()
            => Comparer.GetHashCode(entity!));
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates an instance of <see cref="TestNamedEntity"/> initialized
    /// with the provided identifier,
    /// or returns <see langword="null"/> when the identifier is not specified.
    /// </summary>
    /// <param name="id">
    /// Identifier used to construct the value object instance.
    /// </param>
    /// <returns>
    /// A new <see cref="TestNamedEntity"/> when <paramref name="id"/> has a value,
    /// otherwise - <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(id))]
    private static TestNamedEntity? GetNamedEntity(Guid? id)
    {
        if (!id.HasValue)
            return null;

        var entity = new TestNamedEntity();
        entity.SetId(id.Value);

        return entity;
    }

    /// <summary>
    /// Creates an instance of <see cref="TestOrderedEntity"/> initialized
    /// with the provided identifier,
    /// or returns <see langword="null"/> when the identifier is not specified.
    /// </summary>
    /// <param name="id">
    /// Identifier used to construct the value object instance.
    /// </param>
    /// <returns>
    /// A new <see cref="TestOrderedEntity"/> when <paramref name="id"/> has a value,
    /// otherwise - <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(id))]
    private static TestOrderedEntity? GetOrderedEntity(Guid? id)
    {
        if (!id.HasValue)
            return null;

        var entity = new TestOrderedEntity();
        entity.SetId(id.Value);

        return entity;
    }
    #endregion
}