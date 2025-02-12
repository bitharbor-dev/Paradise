namespace Paradise.Domain.Base.Tests.EqualityComparers;

/// <summary>
/// Test class for the <see cref="ValueObjectEqualityComparer"/>.
/// </summary>
public sealed class ValueObjectEqualityComparerTests
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueObjectEqualityComparerTests"/> class.
    /// </summary>
    public ValueObjectEqualityComparerTests()
        => Comparer = new();
    #endregion

    #region Properties
    /// <summary>
    /// A <see cref="ValueObjectEqualityComparer"/> instance to be tested.
    /// </summary>
    public ValueObjectEqualityComparer Comparer { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="ValueObjectEqualityComparer.Equals"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="false"/> since both of the instances being compared
    /// have different <see cref="ValueObject.GetEqualityComponents"/> return values.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Equals_ReturnsFalseOnNonEqualComponents()
    {
        // Arrange
        var firstValueObject = new FakeValueObject1 { Id = Guid.NewGuid() };

        var secondValueObject = new FakeValueObject1 { Id = Guid.NewGuid() };

        // Act
        var result = Comparer.Equals(firstValueObject, secondValueObject);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="ValueObjectEqualityComparer.Equals"/> test method.
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

        var firstValueObject = new FakeValueObject1 { Id = id };

        var secondValueObject = new FakeValueObject2 { Id = id };

        // Act
        var result = Comparer.Equals(firstValueObject, secondValueObject);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="ValueObjectEqualityComparer.Equals"/> test method.
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
    public void Equals_ReturnsFalseOnNullArgument(FakeValueObject1? a, FakeValueObject1? b)
    {
        // Arrange

        // Act
        var result = Comparer.Equals(a, b);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// <see cref="ValueObjectEqualityComparer.Equals"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns <see langword="true"/> since both of the instances being compared
    /// have the same <see cref="ValueObject.Id"/> and type.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void Equals_ReturnsTrueOnEqualComponentsAndType()
    {
        // Arrange
        var id = Guid.NewGuid();

        var firstValueObject = new FakeValueObject1 { Id = id };

        var secondValueObject = new FakeValueObject1 { Id = id };

        // Act
        var result = Comparer.Equals(firstValueObject, secondValueObject);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// <see cref="ValueObjectEqualityComparer.Equals"/> test method.
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
        FakeValueObject1? firstValueObject = null;
        FakeValueObject1? secondValueObject = null;

        // Act
        var result = Comparer.Equals(firstValueObject, secondValueObject);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// <see cref="ValueObjectEqualityComparer.GetHashCode"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns equal values since both of the instances
    /// have the same <see cref="ValueObject.GetEqualityComponents"/> return values and type.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsEqualValuesOnEqualComponentsAndType()
    {
        // Arrange
        var id = Guid.NewGuid();

        var firstValueObject = new FakeValueObject1 { Id = id };

        var secondValueObject = new FakeValueObject1 { Id = id };

        // Act
        var hash1 = Comparer.GetHashCode(firstValueObject);
        var hash2 = Comparer.GetHashCode(secondValueObject);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    /// <summary>
    /// <see cref="ValueObjectEqualityComparer.GetHashCode"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// Returns different values since both of the instances
    /// have different <see cref="ValueObject.GetEqualityComponents"/> return values.
    /// </para>
    /// </para>
    /// </summary>
    [Fact]
    public void GetHashCode_ReturnsNonEqualValuesOnNonEqualComponents()
    {
        // Arrange
        var firstValueObject = new FakeValueObject1 { Id = Guid.NewGuid() };

        var secondValueObject = new FakeValueObject1 { Id = Guid.NewGuid() };

        // Act
        var hash1 = Comparer.GetHashCode(firstValueObject);
        var hash2 = Comparer.GetHashCode(secondValueObject);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    /// <summary>
    /// <see cref="ValueObjectEqualityComparer.GetHashCode"/> test method.
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

        var firstValueObject = new FakeValueObject1 { Id = id };

        var secondValueObject = new FakeValueObject2 { Id = id };

        // Act
        var hash1 = Comparer.GetHashCode(firstValueObject);
        var hash2 = Comparer.GetHashCode(secondValueObject);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    /// <summary>
    /// <see cref="ValueObjectEqualityComparer.GetHashCode"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="NullReferenceException"/> since the given instance
    /// equals <see langword="null"/>.
    /// </para>
    /// </summary>
    [Fact]
    public void GetHashCode_ThrowsOnNullArgument()
    {
        // Arrange
        FakeValueObject1? firstValueObject = null;

        // Act

        // Assert
        Assert.Throws<ArgumentNullException>(()
            => Comparer.GetHashCode(firstValueObject!));
    }
    #endregion

    #region Data generation
    /// <summary>
    /// Provides member data for <see cref="Equals_ReturnsFalseOnNullArgument"/> method.
    /// </summary>
    public static TheoryData<FakeValueObject1?, FakeValueObject1?> Equals_ReturnsFalseOnNullArgument_MemberData => new()
    {
        { new FakeValueObject1() { Id = Guid.NewGuid() }, null },
        { null, new FakeValueObject1() { Id = Guid.NewGuid() } }
    };
    #endregion
}