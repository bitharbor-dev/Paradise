namespace Paradise.Models.Tests.Unit;

/// <summary>
/// <see cref="PagedListQueryModel"/> test class.
/// </summary>
public sealed class PagedListQueryModelTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="PagedListQueryModel"/> constructor should
    /// successfully create a new instance of the class and
    /// set all property values passed in.
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // Arrange
        var pageSize = 10;
        var pageNumber = 1;
        var orderAscending = true;
        var orderBy = "OrderBy";
        var lookupProperties = new[] { "LookupProperty" };
        var lookupValue = "LookupValue";

        // Act
        var model = new PagedListQueryModel(pageSize, pageNumber, orderAscending, orderBy, lookupProperties, lookupValue);

        // Assert
        Assert.Equal(pageSize, model.PageSize);
        Assert.Equal(pageNumber, model.PageNumber);
        Assert.Equal(orderAscending, model.OrderAscending);
        Assert.Equal(orderBy, model.OrderBy);
        Assert.Equivalent(lookupProperties, model.LookupProperties);
        Assert.Equal(lookupValue, model.LookupValue);
    }

    /// <summary>
    /// The <see cref="PagedListQueryModel"/> constructor should
    /// throw the <see cref="ArgumentOutOfRangeException"/> if the input
    /// <see cref="PagedListQueryModel.PageSize"/> is less than or equal to 0.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsOnZeroPageSize()
    {
        // Arrange

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(()
            => new PagedListQueryModel(0, 1, false, null, [], null));
    }

    /// <summary>
    /// The <see cref="PagedListQueryModel"/> constructor should
    /// throw the <see cref="ArgumentOutOfRangeException"/> if the input
    /// <see cref="PagedListQueryModel.PageNumber"/> is less than or equal to 0.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsOnNegativePageNumber()
    {
        // Arrange

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(()
            => new PagedListQueryModel(10, -3, false, null, [], null));
    }
    #endregion
}