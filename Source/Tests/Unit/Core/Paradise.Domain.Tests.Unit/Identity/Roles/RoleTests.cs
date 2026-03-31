using Paradise.Domain.Base.Exceptions;
using Paradise.Domain.Identity.Roles;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Tests.Unit.Identity.Roles;

/// <summary>
/// <see cref="Role"/> test class.
/// </summary>
public sealed class RoleTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="ValidateState_ThrowsOnNullEmptyOrWhitespaceName"/> method.
    /// </summary>
    public static TheoryData<string?> ValidateState_ThrowsOnNullEmptyOrWhitespaceName_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };

    /// <summary>
    /// Provides member data for <see cref="OperatorEquals_ReturnsFalseOnNull"/> method.
    /// </summary>
    public static TheoryData<string?, string?> OperatorEquals_ReturnsFalseOnNull_MemberData { get; } = new()
    {
        { "LeftName",   null        },
        { null,         "RightName" }
    };

    /// <summary>
    /// Provides member data for <see cref="OperatorNotEquals_ReturnsTrueOnNullArgument"/> method.
    /// </summary>
    public static TheoryData<string?, string?> OperatorNotEquals_ReturnsTrueOnNullArgument_MemberData { get; } = new()
    {
        { "LeftName",   null        },
        { null,         "RightName" }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="Role.Equals(Role?)"/> method should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same type but different <see cref="Role.GetEqualityComponents"/> return values.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualComponents()
    {
        // Arrange
        var left = GetRole(Guid.Empty, name: "LeftName");

        var right = GetRole(Guid.Empty, name: "RightName");

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Role.Equals(Role?)"/> method should
    /// return <see langword="false"/> if one of the instances being compared
    /// is <see langword="null"/>.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNull()
    {
        // Arrange
        var left = GetRole(Guid.Empty);

        var right = GetRole(null);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Role.Equals(Role?)"/> method should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same <see cref="Role.GetEqualityComponents"/> return values and type.
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueOnEqualComponentsAndType()
    {
        // Arrange
        var left = GetRole(Guid.Empty);

        var right = GetRole(Guid.Empty);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="Role.Equals(object?)"/> method should
    /// return <see langword="false"/> if both of the instances being compared
    /// have different types.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsFalseOnNonEqualType()
    {
        // Arrange
        var left = GetRole(Guid.Empty);

        var right = new TestNamedValueObject();

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Role.Equals(object?)"/> method should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same <see cref="Role.GetEqualityComponents"/> return values and type.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsTrueOnEqualComponentsAndType()
    {
        // Arrange
        var left = GetRole(Guid.Empty);

        var right = GetRole(Guid.Empty) as object;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="Role.GetEqualityComponents"/> method should
    /// return <see cref="Role.Name"/> value.
    /// </summary>
    [Fact]
    public void GetEqualityComponents()
    {
        // Arrange
        var entity = GetRole(Guid.Empty);

        // Act
        var result = entity.GetEqualityComponents();

        // Assert
        Assert.Contains(entity.Name, result);

        Assert.Single(result);
    }

    /// <summary>
    /// The <see cref="Role.GetHashCode"/> method should
    /// return the same values if both of the instances being compared
    /// have the same <see cref="Role.GetEqualityComponents"/> return values and type.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsEqualValuesOnEqualComponentsAndType()
    {
        // Arrange
        var left = GetRole(Guid.Empty);

        var right = GetRole(Guid.Empty);

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="Role.GetHashCode"/> method should
    /// return different values if both of the instances being compared
    /// have the same type but different <see cref="Role.GetEqualityComponents"/> return values.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnNonEqualComponents()
    {
        // Arrange
        var left = GetRole(Guid.Empty, name: "LeftName");

        var right = GetRole(Guid.Empty, name: "RightName");

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="Role.GetHashCode"/> method should
    /// return different values if both of the instances being compared
    /// have different type.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnNonEqualType()
    {
        // Arrange
        var left = GetRole(Guid.Empty);

        var right = new TestNamedValueObject();

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="Role.operator =="/> operator should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same type but different <see cref="Role.GetEqualityComponents"/> return values.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsFalseOnNonEqualComponents()
    {
        // Arrange
        var left = GetRole(Guid.Empty, name: "LeftName");

        var right = GetRole(Guid.Empty, name: "RightName");

        // Act
        var result = left == right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Role.operator =="/> operator should
    /// return <see langword="false"/> if one of the instances being compared
    /// is <see langword="null"/>.
    /// </summary>
    /// <param name="leftName">
    /// A string value used to instantiate left operand.
    /// </param>
    /// <param name="rightName">
    /// A string value used to instantiate right operand.
    /// </param>
    [Theory, MemberData(nameof(OperatorEquals_ReturnsFalseOnNull_MemberData))]
    public void OperatorEquals_ReturnsFalseOnNull(string? leftName, string? rightName)
    {
        // Arrange
        var left = GetRole(Guid.Empty, name: leftName!);

        var right = GetRole(Guid.Empty, name: rightName!);

        // Act
        var result = left == right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Role.operator =="/> operator should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same <see cref="Role.GetEqualityComponents"/> return values and type.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsTrueOnEqualComponentsAndType()
    {
        // Arrange
        var left = GetRole(Guid.Empty);

        var right = GetRole(Guid.Empty);

        // Act
        var result = left == right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="Role.operator !="/> operator should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same type but different <see cref="Role.GetEqualityComponents"/> return values.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsTrueOnNonEqualComponents()
    {
        // Arrange
        var left = GetRole(Guid.Empty, name: "LeftName");

        var right = GetRole(Guid.Empty, name: "RightName");

        // Act
        var result = left != right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="Role.operator !="/> operator should
    /// return <see langword="true"/> if one of the instances being compared
    /// is <see langword="null"/>.
    /// </summary>
    /// <param name="leftName">
    /// A string value used to instantiate left operand.
    /// </param>
    /// <param name="rightName">
    /// A string value used to instantiate right operand.
    /// </param>
    [Theory, MemberData(nameof(OperatorNotEquals_ReturnsTrueOnNullArgument_MemberData))]
    public void OperatorNotEquals_ReturnsTrueOnNullArgument(string? leftName, string? rightName)
    {
        // Arrange
        var left = GetRole(Guid.Empty, name: leftName!);

        var right = GetRole(Guid.Empty, name: rightName!);

        // Act
        var result = left != right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="Role.operator !="/> operator should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same <see cref="Role.GetEqualityComponents"/> return values and type.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsFalseOnEqualComponentsAndType()
    {
        // Arrange
        var left = GetRole(Guid.Empty);

        var right = GetRole(Guid.Empty);

        // Act
        var result = left != right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="Role.ValidateState"/> method should
    /// not throw any exception for the <see cref="Role"/> instance
    /// which <see cref="Role.Name"/>
    /// is not equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only
    /// and does not contain invalid characters.
    /// </summary>
    [Fact]
    public void ValidateState()
    {
        // Arrange
        var entity = GetRole(Guid.Empty);

        // Act & Assert
        entity.ValidateState();
    }

    /// <summary>
    /// The <see cref="Role.ValidateState"/> method should
    /// throw the <see cref="DomainStateException{TEntity}"/> for the
    /// <see cref="Role"/> instance
    /// which <see cref="Role.Name"/> contains invalid characters.
    /// </summary>
    [Fact]
    public void ValidateState_ThrowsOnInvalidCharacters()
    {
        // Arrange
        var entity = GetRole(Guid.Empty, name: "Name-123");

        // Act & Assert
        Assert.Throws<DomainStateException<Role>>(entity.ValidateState);
    }

    /// <summary>
    /// The <see cref="Role.ValidateState"/> method should
    /// throw the <see cref="DomainStateException{TEntity}"/> for the
    /// <see cref="Role"/> instance
    /// which <see cref="Role.Name"/>
    /// is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    /// <param name="name">
    /// Role name.
    /// </param>
    [Theory, MemberData(nameof(ValidateState_ThrowsOnNullEmptyOrWhitespaceName_MemberData))]
    public void ValidateState_ThrowsOnNullEmptyOrWhitespaceName(string? name)
    {
        // Arrange
        var entity = GetRole(Guid.Empty, name: name!);

        // Act & Assert
        Assert.Throws<DomainStateException<Role>>(entity.ValidateState);
    }

    /// <summary>
    /// The <see cref="Role.OnCreated"/> method should
    /// set the entity unique identifier and creation date,
    /// as well as invoke the <see cref="Role.ValidateState"/> method.
    /// </summary>
    [Fact]
    public void OnCreated()
    {
        // Arrange
        var id = Guid.Empty;
        var utcNow = DateTimeOffset.UnixEpoch;

        var entity = GetRole(id);

        // Act
        entity.OnCreated(utcNow);

        // Assert
        Assert.NotEqual(id, entity.Id);
        Assert.Equal(utcNow, entity.Created);
    }

    /// <summary>
    /// The <see cref="Role.OnModified"/> method should
    /// set the entity creation date,
    /// as well as invoke the <see cref="Role.ValidateState"/> method.
    /// </summary>
    [Fact]
    public void OnModified()
    {
        // Arrange
        var id = Guid.Empty;
        var utcNow = DateTimeOffset.UnixEpoch;

        var entity = GetRole(id);

        // Act
        entity.OnModified(utcNow);

        // Assert
        Assert.Equal(utcNow, entity.Modified);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates an instance of <see cref="Role"/> initialized with the provided identifier,
    /// or returns <see langword="null"/> when the identifier is unspecified.
    /// </summary>
    /// <param name="id">
    /// Unique identifier.
    /// </param>
    /// <param name="name">
    /// Role name.
    /// </param>
    /// <param name="isDefault">
    /// Indicates whether the role is default and should be
    /// assigned automatically when a user has been created.
    /// </param>
    /// <returns>
    /// A new <see cref="Role"/> when <paramref name="id"/> has a value,
    /// otherwise - <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(id))]
    private static Role? GetRole(Guid? id, string name = "Name", bool isDefault = false)
        => id.HasValue ? new(name, isDefault) { Id = id.Value } : null;
    #endregion
}