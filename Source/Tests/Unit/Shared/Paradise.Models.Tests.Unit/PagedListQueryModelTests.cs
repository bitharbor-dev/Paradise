namespace Paradise.Models.Tests.Unit;

/// <summary>
/// <see cref="PagedListQueryModel"/> test class.
/// </summary>
public sealed class PagedListQueryModelTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="PagedListQueryModel"/> constructor should
    /// throw the <see cref="ArgumentException"/> if the input
    /// <see cref="PagedListQueryModel.PageSize"/> is less than or equal to 0.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsOnZeroPageSize()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<ArgumentException>(()
            => new PagedListQueryModel(0, 1, false, null, [], null));

        // Assert
        Assert.Contains(nameof(PagedListQueryModel.PageSize), exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// The <see cref="PagedListQueryModel"/> constructor should
    /// throw the <see cref="ArgumentException"/> if the input
    /// <see cref="PagedListQueryModel.PageNumber"/> is less than or equal to 0.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsOnNegativePageNumber()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<ArgumentException>(()
            => new PagedListQueryModel(10, -3, false, null, [], null));

        // Assert
        Assert.Contains(nameof(PagedListQueryModel.PageNumber), exception.Message, StringComparison.Ordinal);
    }
    #endregion
}