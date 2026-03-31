using Paradise.DataAccess.Repositories.Base.Extensions;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;

namespace Paradise.DataAccess.Tests.Unit.Repositories.Base.Extensions;

/// <summary>
/// <see cref="IQueryableExtensions"/> test class.
/// </summary>
public sealed class IQueryableExtensionsTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="FilterBy_ReturnsOriginalOnNullOrEmptyValue"/> method.
    /// </summary>
    public static TheoryData<string?> FilterBy_ReturnsOriginalOnNullOrEmptyValue_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };

    /// <summary>
    /// Provides member data for <see cref="OrderByPropertyName_ReturnsOriginalOnNullOrEmptyName"/> method.
    /// </summary>
    public static TheoryData<string?> OrderByPropertyName_ReturnsOriginalOnNullOrEmptyName_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="IQueryableExtensions.FilterBy"/> method should
    /// modify the input query, such that the filtering will be applied
    /// using the input property names and the value they should contain.
    /// </summary>
    [Fact]
    public void FilterBy()
    {
        // Arrange
        var value1 = "Entity1";
        var value2 = "Entity2";

        var queryable = new List<TestNamedEntity>
        {
            new() { Name = value1 },
            new() { Name = value2 }
        }.AsQueryable();

        // Act
        var result = queryable.FilterBy([nameof(TestNamedEntity.Name)], value1);

        // Assert
        var entity = Assert.Single(result);
        Assert.Equal(value1, entity.Name);
    }

    /// <summary>
    /// The <see cref="IQueryableExtensions.FilterBy"/> method should
    /// modify the input query, such that the case insensitive filtering will be applied
    /// using the input property names and the value they should contain.
    /// </summary>
    [Fact]
    public void FilterBy_CaseInsensitive()
    {
        // Arrange
        var value1 = "Entity1";
        var value2 = value1.ToUpperInvariant();

        var queryable = new List<TestNamedEntity>
        {
            new() { Name = value1 },
            new() { Name = value2 }
        }.AsQueryable();

        // Act
        var result = queryable.FilterBy([nameof(TestNamedEntity.Name)], value1);

        // Assert
        Assert.Collection(result, item => Assert.Equal(value1, item.Name),
                                  item => Assert.Equal(value2, item.Name));
    }

    /// <summary>
    /// The <see cref="IQueryableExtensions.FilterBy"/> method should
    /// return the input query, if no filtering properties specified.
    /// </summary>
    [Fact]
    public void FilterBy_ReturnsOriginalOnEmptyProperties()
    {
        // Arrange
        var value = "Entity1";

        var queryable = new List<TestNamedEntity>
        {
            new() { Name = value }
        }.AsQueryable();

        // Act
        var result = queryable.FilterBy([], value);

        // Assert
        Assert.Same(queryable, result);
    }

    /// <summary>
    /// The <see cref="IQueryableExtensions.FilterBy"/> method should
    /// return the input query, if the filtering value
    /// is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    /// <param name="value">
    /// Filter value.
    /// </param>
    [Theory, MemberData(nameof(FilterBy_ReturnsOriginalOnNullOrEmptyValue_MemberData))]
    public void FilterBy_ReturnsOriginalOnNullOrEmptyValue(string? value)
    {
        // Arrange
        var queryable = new List<TestNamedEntity>
        {
            new() { Name = value }
        }.AsQueryable();

        // Act
        var result = queryable.FilterBy([nameof(TestNamedEntity.Name)], value);

        // Assert
        Assert.Same(queryable, result);
    }

    /// <summary>
    /// The <see cref="IQueryableExtensions.FilterBy"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// filtering properties list contains one or more non-string properties.
    /// </summary>
    [Fact]
    public void FilterBy_ThrowsOnNonStringProperty()
    {
        // Arrange
        var value = "Entity1";

        var queryable = new List<TestNamedEntity>().AsQueryable();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => queryable.FilterBy([nameof(TestNamedEntity.Id)], value));
    }

    /// <summary>
    /// The <see cref="IQueryableExtensions.FilterBy"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// filtering properties list contains one or more property which
    /// cannot be found on the queryable item type.
    /// </summary>
    [Fact]
    public void FilterBy_ThrowsOnUnknownProperty()
    {
        // Arrange
        var value = "Entity1";

        var queryable = new List<TestNamedEntity>().AsQueryable();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => queryable.FilterBy([nameof(Exception.Message)], value));
    }

    /// <summary>
    /// The <see cref="IQueryableExtensions.OrderByPropertyName"/> method
    /// modify the input query, such that the ascending ordering will be applied
    /// using the input property name.
    /// </summary>
    [Fact]
    public void OrderByPropertyName_Ascending()
    {
        // Arrange
        var value1 = "Entity1";
        var value2 = "Entity2";

        var queryable = new List<TestNamedEntity>
        {
            new() { Name = value1 },
            new() { Name = value2 }
        }.AsQueryable();

        // Act
        var result = queryable.OrderByPropertyName(nameof(TestNamedEntity.Name), true);

        // Assert
        Assert.Collection(result, item => Assert.Equal(value1, item.Name),
                                  item => Assert.Equal(value2, item.Name));
    }

    /// <summary>
    /// The <see cref="IQueryableExtensions.OrderByPropertyName"/> method
    /// modify the input query, such that the descending ordering will be applied
    /// using the input property name.
    /// </summary>
    [Fact]
    public void OrderByPropertyName_Descending()
    {
        // Arrange
        var value1 = "Entity1";
        var value2 = "Entity2";

        var queryable = new List<TestNamedEntity>
        {
            new() { Name = value1 },
            new() { Name = value2 }
        }.AsQueryable();

        // Act
        var result = queryable.OrderByPropertyName(nameof(TestNamedEntity.Name), false);

        // Assert
        Assert.Collection(result, item => Assert.Equal(value2, item.Name),
                                  item => Assert.Equal(value1, item.Name));
    }

    /// <summary>
    /// The <see cref="IQueryableExtensions.OrderByPropertyName"/> method should
    /// return the input query, if the ordering property name
    /// is equal to <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only.
    /// </summary>
    /// <param name="name">
    /// Ordering property name.
    /// </param>
    [Theory, MemberData(nameof(OrderByPropertyName_ReturnsOriginalOnNullOrEmptyName_MemberData))]
    public void OrderByPropertyName_ReturnsOriginalOnNullOrEmptyName(string? name)
    {
        // Arrange
        var queryable = new List<TestNamedEntity>().AsQueryable();

        // Act
        var result = queryable.OrderByPropertyName(name, false);

        // Assert
        Assert.Same(queryable, result);
    }

    /// <summary>
    /// The <see cref="IQueryableExtensions.OrderByPropertyName"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// ordering property cannot be found on the queryable item type.
    /// </summary>
    [Fact]
    public void OrderByPropertyName_ThrowsOnUnknownProperty()
    {
        // Arrange
        var queryable = new List<TestNamedEntity>().AsQueryable();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(()
            => queryable.OrderByPropertyName(nameof(Exception.Message), true));
    }
    #endregion
}