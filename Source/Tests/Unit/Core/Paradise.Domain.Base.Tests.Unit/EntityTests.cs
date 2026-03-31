using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.Tests.Unit;

/// <summary>
/// <see cref="Entity"/> test class.
/// </summary>
public sealed class EntityTests
{
    #region Properties
    /// <summary>
    /// Predefined GUID value to be used for data arrangement.
    /// </summary>
    public static Guid LeftId { get; } = Guid.Parse("0198610a-ac67-7bf0-8d08-676de1492235");

    /// <summary>
    /// Predefined GUID value to be used for data arrangement.
    /// </summary>
    public static Guid RightId { get; } = Guid.Parse("0198610a-ac67-7bf0-8d08-676de1492236");

    /// <summary>
    /// Provides member data for <see cref="OperatorEquals_ReturnsFalseOnNull"/> method.
    /// </summary>
    public static TheoryData<Guid?, Guid?> OperatorEquals_ReturnsFalseOnNull_MemberData { get; } = new()
    {
        { LeftId,   null    },
        { null,     RightId }
    };

    /// <summary>
    /// Provides member data for <see cref="OperatorNotEquals_ReturnsTrueOnNullArgument"/> method.
    /// </summary>
    public static TheoryData<Guid?, Guid?> OperatorNotEquals_ReturnsTrueOnNullArgument_MemberData { get; } = new()
    {
        { LeftId,   null    },
        { null,     RightId }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="Entity.Equals(Entity?)"/> method should
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
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Entity.Equals(Entity?)"/> method should
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
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Entity.Equals(Entity?)"/> method should
    /// return <see langword="false"/> if one of the instances being compared
    /// is <see langword="null"/>.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNull()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(null);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Entity.Equals(Entity?)"/> method should
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
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="Entity.Equals(object?)"/> method should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same type but different <see cref="Entity.Id"/>.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsFalseOnNonEqualId()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(RightId) as object;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Entity.Equals(object?)"/> method should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> but different types.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsFalseOnNonEqualType()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetOrderedEntity(LeftId) as object;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Entity.Equals(object?)"/> method should
    /// return <see langword="false"/> if one of the instances being compared
    /// is <see langword="null"/>.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsFalseOnNull()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(null) as object;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Entity.Equals(object?)"/> method should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> and type.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsTrueOnEqualIdAndType()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(LeftId) as object;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="Entity.GetHashCode"/> method should
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
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="Entity.GetHashCode"/> method should
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
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="Entity.GetHashCode"/> method should
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
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="Entity.operator =="/> operator should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same type but different <see cref="Entity.Id"/>.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsFalseOnNonEqualId()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(RightId);

        // Act
        var result = left == right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Entity.operator =="/> operator should
    /// return <see langword="false"/> if one of the instances being compared
    /// is <see langword="null"/>.
    /// </summary>
    /// <param name="leftId">
    /// A GUID value used to instantiate left operand.
    /// </param>
    /// <param name="rightId">
    /// A GUID value used to instantiate right operand.
    /// </param>
    [Theory, MemberData(nameof(OperatorEquals_ReturnsFalseOnNull_MemberData))]
    public void OperatorEquals_ReturnsFalseOnNull(Guid? leftId, Guid? rightId)
    {
        // Arrange
        var left = GetNamedEntity(leftId);
        var right = GetNamedEntity(rightId);

        // Act
        var result = left == right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Entity.operator =="/> operator should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> and type.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsTrueOnEqualIdAndType()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(LeftId);

        // Act
        var result = left == right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="Entity.operator !="/> operator should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same type but different <see cref="Entity.Id"/>.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsTrueOnNonEqualId()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(RightId);

        // Act
        var result = left != right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="Entity.operator !="/> operator should
    /// return <see langword="true"/> if one of the instances being compared
    /// is <see langword="null"/>.
    /// </summary>
    /// <param name="leftId">
    /// A GUID value used to instantiate left operand.
    /// </param>
    /// <param name="rightId">
    /// A GUID value used to instantiate right operand.
    /// </param>
    [Theory, MemberData(nameof(OperatorNotEquals_ReturnsTrueOnNullArgument_MemberData))]
    public void OperatorNotEquals_ReturnsTrueOnNullArgument(Guid? leftId, Guid? rightId)
    {
        // Arrange
        var left = GetNamedEntity(leftId);
        var right = GetNamedEntity(rightId);

        // Act
        var result = left != right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="Entity.operator !="/> operator should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> and type.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsFalseOnEqualIdAndType()
    {
        // Arrange
        var left = GetNamedEntity(LeftId);
        var right = GetNamedEntity(LeftId);

        // Act
        var result = left != right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Entity.OnCreated"/> method should
    /// set the entity unique identifier and creation date,
    /// as well as invoke the <see cref="Entity.ValidateState"/> method.
    /// </summary>
    [Fact]
    public void OnCreated()
    {
        // Arrange
        var utcNow = DateTimeOffset.UnixEpoch;
        var entity = new TestValidatableEntity();

        // Act
        entity.OnCreated(utcNow);

        // Assert
        Assert.NotEqual(default, entity.Id);
        Assert.Equal(utcNow, entity.Created);
        Assert.True(entity.StateValidated);
    }

    /// <summary>
    /// The <see cref="Entity.OnModified"/> method should
    /// set the entity creation date,
    /// as well as invoke the <see cref="Entity.ValidateState"/> method.
    /// </summary>
    [Fact]
    public void OnModified()
    {
        // Arrange
        var utcNow = DateTimeOffset.UnixEpoch;
        var entity = new TestValidatableEntity();

        // Act
        entity.OnModified(utcNow);

        // Assert
        Assert.Equal(utcNow, entity.Modified);
        Assert.True(entity.StateValidated);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates an instance of <see cref="TestNamedEntity"/> initialized
    /// with the provided identifier,
    /// or returns <see langword="null"/> when the identifier is unspecified.
    /// </summary>
    /// <param name="id">
    /// Unique identifier.
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
    /// or returns <see langword="null"/> when the identifier is unspecified.
    /// </summary>
    /// <param name="id">
    /// Unique identifier.
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