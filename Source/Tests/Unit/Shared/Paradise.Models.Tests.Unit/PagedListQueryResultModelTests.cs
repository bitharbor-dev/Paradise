namespace Paradise.Models.Tests.Unit;

/// <summary>
/// <see cref="PagedListQueryResultModel{T}"/> test class.
/// </summary>
public sealed class PagedListQueryResultModelTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="PagedListQueryResultModel{T}"/> constructor should
    /// successfully create a new instance of the class and
    /// populate the <see cref="PagedListQueryResultModel{T}.CurrentPageItems"/>
    /// and <see cref="PagedListQueryResultModel{T}.Total"/> properties.
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // Arrange
        var currentPageItems = Array.Empty<object>();
        var total = 0;

        // Act
        var queryResult = new PagedListQueryResultModel<object>(currentPageItems, total);

        // Assert
        Assert.Equal(currentPageItems, queryResult.CurrentPageItems);
        Assert.Equal(total, queryResult.Total);
    }
    #endregion
}