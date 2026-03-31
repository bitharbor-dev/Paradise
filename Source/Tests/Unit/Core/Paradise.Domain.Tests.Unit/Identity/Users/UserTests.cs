using Paradise.Domain.Base;
using Paradise.Domain.Base.Exceptions;
using Paradise.Domain.Identity.Users;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Tests.Unit.Identity.Users;

/// <summary>
/// <see cref="User"/> test class.
/// </summary>
public sealed class UserTests
{
    #region Properties
    /// <inheritdoc cref="DateTimeOffset.UnixEpoch"/>
    public static DateTimeOffset UnixEpoch { get; } = DateTimeOffset.UnixEpoch;

    /// <summary>
    /// Predefined GUID value to be used for data arrangement.
    /// </summary>
    public static Guid LeftId { get; } = Guid.Parse("0198610a-ac67-7bf0-8d08-676de1492235");

    /// <summary>
    /// Predefined GUID value to be used for data arrangement.
    /// </summary>
    public static Guid RightId { get; } = Guid.Parse("0198610a-ac67-7bf0-8d08-676de1492236");

    /// <summary>
    /// Provides member data for <see cref="ValidateState_ThrowsOnInvalidEmailAddress"/> method.
    /// </summary>
    public static TheoryData<string?> ValidateState_ThrowsOnInvalidEmailAddress_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };

    /// <summary>
    /// Provides member data for <see cref="ValidateState_ThrowsOnInvalidUserName"/> method.
    /// </summary>
    public static TheoryData<string?> ValidateState_ThrowsOnInvalidUserName_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };

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
    /// The <see cref="User.CanBeDeleted"/> method should
    /// return <see langword="true"/> if the
    /// sum of <see cref="User.DeletionRequestSubmitted"/>
    /// and specified deletion request lifetime is greater than the current time.
    /// </summary>
    [Fact]
    public void CanBeDeleted_ReturnsFalse()
    {
        // Arrange
        var lifetime = TimeSpan.FromSeconds(1);
        var currentTime = UnixEpoch.Subtract(lifetime).Subtract(lifetime);

        var entity = GetUser(Guid.Empty);
        entity.DeletionRequestSubmitted = UnixEpoch;

        // Act
        var result = entity.CanBeDeleted(lifetime, currentTime);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="User.CanBeDeleted"/> method should
    /// return <see langword="false"/> if the
    /// <see cref="User.DeletionRequestSubmitted"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void CanBeDeleted_ReturnsFalseOnNull()
    {
        // Arrange
        var lifetime = TimeSpan.FromSeconds(1);
        var currentTime = UnixEpoch;

        var entity = GetUser(Guid.Empty);
        entity.DeletionRequestSubmitted = null;

        // Act
        var result = entity.CanBeDeleted(lifetime, currentTime);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="User.CanBeDeleted"/> method should
    /// return <see langword="false"/> if the
    /// sum of <see cref="User.DeletionRequestSubmitted"/>
    /// and specified deletion request lifetime is less than the current time.
    /// </summary>
    [Fact]
    public void CanBeDeleted_ReturnsTrue()
    {
        // Arrange
        var lifetime = TimeSpan.FromSeconds(1);
        var currentTime = UnixEpoch.Add(lifetime).Add(lifetime);

        var entity = GetUser(Guid.Empty);
        entity.DeletionRequestSubmitted = UnixEpoch;

        // Act
        var result = entity.CanBeDeleted(lifetime, currentTime);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="User.CancelDeletionRequest"/> should
    /// set the <see cref="User.DeletionRequestSubmitted"/> back to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void CancelDeletionRequest()
    {
        // Arrange
        var entity = GetUser(Guid.Empty);
        entity.DeletionRequestSubmitted = UnixEpoch;

        // Act
        entity.CancelDeletionRequest();

        // Assert
        Assert.Null(entity.DeletionRequestSubmitted);
    }

    /// <summary>
    /// The <see cref="User.CanConfirmEmailAddress"/> method should
    /// return <see langword="false"/> if sum of the <see cref="User.Created"/>
    /// and specified confirmation period is greater than current time.
    /// </summary>
    [Fact]
    public void CanConfirmEmailAddress_ReturnsFalse()
    {
        // Arrange
        var confirmationPeriod = TimeSpan.FromSeconds(1);
        var currentTime = UnixEpoch.Subtract(confirmationPeriod).Subtract(confirmationPeriod);

        var entity = GetUser(Guid.Empty);
        entity.OnCreated(UnixEpoch);

        // Act
        var result = entity.CanConfirmEmailAddress(confirmationPeriod, currentTime);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="User.CanConfirmEmailAddress"/> method should
    /// return <see langword="true"/> if sum of the <see cref="User.Created"/>
    /// and specified confirmation period is less than current time.
    /// </summary>
    [Fact]
    public void CanConfirmEmailAddress_ReturnsTrue()
    {
        // Arrange
        var confirmationPeriod = TimeSpan.FromSeconds(1);
        var currentTime = UnixEpoch.Add(confirmationPeriod).Add(confirmationPeriod);

        var entity = GetUser(Guid.Empty);
        entity.OnCreated(UnixEpoch);

        // Act
        var result = entity.CanConfirmEmailAddress(confirmationPeriod, currentTime);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="User.Equals(User?)"/> method should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same type but different <see cref="Entity.Id"/>.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualId()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = GetUser(RightId);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="User.Equals(User?)"/> method should
    /// return <see langword="false"/> if one of the instances being compared
    /// is <see langword="null"/>.
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNull()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = GetUser(null);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="User.Equals(User?)"/> method should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> and type.
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueOnEqualIdAndType()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = GetUser(LeftId);

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="User.Equals(object?)"/> method should
    /// return <see langword="false"/> if both of the instances being compared
    /// have different types.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsFalseOnNonEqualType()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = new TestNamedEntity();

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="User.Equals(object?)"/> method should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> and type.
    /// </summary>
    [Fact]
    public void Equals_Overload_ReturnsTrueOnEqualIdAndType()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = GetUser(LeftId) as object;

        // Act
        var result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="User.GetHashCode"/> method should
    /// return the same values if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> and type.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsEqualValuesOnEqualIdAndType()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = GetUser(LeftId);

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="User.GetHashCode"/> method should
    /// return different values if both of the instances being compared
    /// have the same type but different <see cref="Entity.Id"/>.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnNonEqualId()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = GetUser(RightId);

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="User.GetHashCode"/> method should
    /// return different values if both of the instances being compared
    /// have different type.
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnNonEqualType()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = new TestNamedEntity();

        // Act
        var leftHash = left.GetHashCode();
        var rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    /// <summary>
    /// The <see cref="User.operator =="/> operator should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same type but different <see cref="Entity.Id"/>.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsFalseOnNonEqualId()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = GetUser(RightId);

        // Act
        var result = left == right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="User.operator =="/> operator should
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
        var left = GetUser(leftId);

        var right = GetUser(rightId);

        // Act
        var result = left == right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="User.operator =="/> operator should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> and type.
    /// </summary>
    [Fact]
    public void OperatorEquals_ReturnsTrueOnEqualIdAndType()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = GetUser(LeftId);

        // Act
        var result = left == right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="User.operator !="/> operator should
    /// return <see langword="true"/> if both of the instances being compared
    /// have the same type but different <see cref="Entity.Id"/>.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsTrueOnNonEqualId()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = GetUser(RightId);

        // Act
        var result = left != right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="User.operator !="/> operator should
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
        var left = GetUser(leftId);

        var right = GetUser(rightId);

        // Act
        var result = left != right;

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// The <see cref="User.operator !="/> operator should
    /// return <see langword="false"/> if both of the instances being compared
    /// have the same <see cref="Entity.Id"/> and type.
    /// </summary>
    [Fact]
    public void OperatorNotEquals_ReturnsFalseOnEqualIdAndType()
    {
        // Arrange
        var left = GetUser(LeftId);

        var right = GetUser(LeftId);

        // Act
        var result = left != right;

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// The <see cref="User.ValidateState"/> method should
    /// not throw any exception for the <see cref="User"/> instance
    /// which <see cref="User.Email"/> and <see cref="User.UserName"/>
    /// are not equal to <see langword="null"/> or <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    [Fact]
    public void ValidateState()
    {
        // Arrange
        var entity = GetUser(Guid.Empty);

        // Act & Assert
        entity.ValidateState();
    }

    /// <summary>
    /// The <see cref="User.ValidateState"/> method should
    /// throw the <see cref="DomainStateException{TEntity}"/> for the
    /// <see cref="User"/> instance which <see cref="User.Email"/>
    /// is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    /// <param name="emailAddress">
    /// Email address.
    /// </param>
    [Theory, MemberData(nameof(ValidateState_ThrowsOnInvalidEmailAddress_MemberData))]
    public void ValidateState_ThrowsOnInvalidEmailAddress(string? emailAddress)
    {
        // Arrange
        var entity = GetUser(Guid.Empty, email: emailAddress!);

        // Act & Assert
        Assert.Throws<DomainStateException<User>>(entity.ValidateState);
    }

    /// <summary>
    /// The <see cref="User.ValidateState"/> method should
    /// throw the <see cref="DomainStateException{TEntity}"/> for the
    /// <see cref="User"/> instance which <see cref="User.UserName"/>
    /// is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    /// <param name="userName">
    /// User-name.
    /// </param>
    [Theory, MemberData(nameof(ValidateState_ThrowsOnInvalidUserName_MemberData))]
    public void ValidateState_ThrowsOnInvalidUserName(string? userName)
    {
        // Arrange
        var entity = GetUser(Guid.Empty, userName: userName!);

        // Act & Assert
        Assert.Throws<DomainStateException<User>>(entity.ValidateState);
    }

    /// <summary>
    /// The <see cref="User.OnCreated"/> method should
    /// set the entity unique identifier and creation date,
    /// as well as invoke the <see cref="User.ValidateState"/> method.
    /// </summary>
    [Fact]
    public void OnCreated()
    {
        // Arrange
        var id = Guid.Empty;
        var utcNow = DateTimeOffset.UnixEpoch;

        var entity = GetUser(id);

        // Act
        entity.OnCreated(utcNow);

        // Assert
        Assert.NotEqual(id, entity.Id);
        Assert.Equal(utcNow, entity.Created);
    }

    /// <summary>
    /// The <see cref="User.OnModified"/> method should
    /// set the entity creation date,
    /// as well as invoke the <see cref="User.ValidateState"/> method.
    /// </summary>
    [Fact]
    public void OnModified()
    {
        // Arrange
        var id = Guid.Empty;
        var utcNow = DateTimeOffset.UnixEpoch;

        var entity = GetUser(id);

        // Act
        entity.OnModified(utcNow);

        // Assert
        Assert.Equal(utcNow, entity.Modified);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates an instance of <see cref="User"/> initialized with the provided identifier,
    /// or returns <see langword="null"/> when the identifier is unspecified.
    /// </summary>
    /// <param name="id">
    /// Unique identifier.
    /// </param>
    /// <param name="email">
    /// Email address.
    /// </param>
    /// <param name="userName">
    /// User-name.
    /// </param>
    /// <returns>
    /// A new <see cref="User"/> when <paramref name="id"/> has a value,
    /// otherwise - <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(id))]
    private static User? GetUser(Guid? id, string email = "Email", string userName = "UserName")
        => id.HasValue ? new(email, userName) { Id = id.Value } : null;
    #endregion
}