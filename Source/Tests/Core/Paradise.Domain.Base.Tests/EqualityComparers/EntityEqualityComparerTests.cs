using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Base.Tests.EqualityComparers;

/// <summary>
/// Test class for the <see cref="EntityEqualityComparer"/>.
/// </summary>
public sealed class EntityEqualityComparerTests
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityEqualityComparerTests"/> class.
    /// </summary>
    public EntityEqualityComparerTests()
        => Comparer = new();
    #endregion

    #region Properties
    /// <summary>
    /// A <see cref="EntityEqualityComparer"/> instance to be tested.
    /// </summary>
    public EntityEqualityComparer Comparer { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="EntityEqualityComparer.Equals"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/> since both of the instances being compared
    /// have different <see cref="IDatabaseRecord.Id"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualId()
    {
        // Arrange
        var firstEntity = new FakeEntity1 { Id = Guid.NewGuid() };

        var secondEntity = new FakeEntity1 { Id = Guid.NewGuid() };

        // Act
        var result = Comparer.Equals(firstEntity, secondEntity);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="EntityEqualityComparer.Equals"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/> since both of the instances being compared
    /// have different types.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualType()
    {
        // Arrange
        var id = Guid.NewGuid();

        var firstEntity = new FakeEntity1 { Id = id };

        var secondEntity = new FakeEntity2 { Id = id };

        // Act
        var result = Comparer.Equals(firstEntity, secondEntity);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="EntityEqualityComparer.Equals"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/> since one of the instances being compared
    /// is <see langword="null"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Theory, MemberData(nameof(Equals_ReturnsFalseOnNullArgument_MemberData))]
    public void Equals_ReturnsFalseOnNullArgument(FakeEntity1? a, FakeEntity1? b)
    {
        // Arrange

        // Act
        var result = Comparer.Equals(a, b);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="EntityEqualityComparer.Equals"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/> since both of the instances being compared
    /// have the same <see cref="IDatabaseRecord.Id"/> and type.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueOnEqualIdAndType()
    {
        // Arrange
        var id = Guid.NewGuid();

        var firstEntity = new FakeEntity1 { Id = id };

        var secondEntity = new FakeEntity1 { Id = id };

        // Act
        var result = Comparer.Equals(firstEntity, secondEntity);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// <see cref="EntityEqualityComparer.Equals"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/> since both of the instances being compared
    /// equals <see langword="null"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueOnNullArguments()
    {
        // Arrange
        FakeEntity1? firstEntity = null;
        FakeEntity1? secondEntity = null;

        // Act
        var result = Comparer.Equals(firstEntity, secondEntity);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// <see cref="EntityEqualityComparer.GetHashCode"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns equal values since both of the instances
    /// have the same <see cref="IDatabaseRecord.Id"/> and type.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsEqualValuesOnEqualIdAndType()
    {
        // Arrange
        var id = Guid.NewGuid();

        var firstEntity = new FakeEntity1 { Id = id };

        var secondEntity = new FakeEntity1 { Id = id };

        // Act
        var hash1 = Comparer.GetHashCode(firstEntity);
        var hash2 = Comparer.GetHashCode(secondEntity);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    /// <summary>
    /// <see cref="EntityEqualityComparer.GetHashCode"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns different values since both of the instances
    /// have different <see cref="IDatabaseRecord.Id"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnNonEqualId()
    {
        // Arrange
        var firstEntity = new FakeEntity1 { Id = Guid.NewGuid() };

        var secondEntity = new FakeEntity1 { Id = Guid.NewGuid() };

        // Act
        var hash1 = Comparer.GetHashCode(firstEntity);
        var hash2 = Comparer.GetHashCode(secondEntity);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    /// <summary>
    /// <see cref="EntityEqualityComparer.GetHashCode"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns different values since both of the instances
    /// have different types.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnNonEqualType()
    {
        // Arrange
        var id = Guid.NewGuid();

        var firstEntity = new FakeEntity1 { Id = id };

        var secondEntity = new FakeEntity2 { Id = id };

        // Act
        var hash1 = Comparer.GetHashCode(firstEntity);
        var hash2 = Comparer.GetHashCode(secondEntity);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    /// <summary>
    /// <see cref="EntityEqualityComparer.GetHashCode"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ArgumentNullException"/> since the given instance
    /// equals <see langword="null"/>.
    /// </para>
    /// </summary>
    [Fact]
    public void GetHashCode_ThrowsOnNullArgument()
    {
        // Arrange
        FakeEntity1? firstEntity = null;

        // Act

        // Assert
        Assert.Throws<ArgumentNullException>(()
            => Comparer.GetHashCode(firstEntity!));
    }
    #endregion

    #region Data generation
    /// <summary>
    /// Provides member data for <see cref="Equals_ReturnsFalseOnNullArgument"/> method.
    /// </summary>
    public static TheoryData<FakeEntity1?, FakeEntity1?> Equals_ReturnsFalseOnNullArgument_MemberData => new()
    {
        { new FakeEntity1() { Id = Guid.NewGuid() }, null },
        { null, new FakeEntity1() { Id = Guid.NewGuid() } }
    };
    #endregion
}