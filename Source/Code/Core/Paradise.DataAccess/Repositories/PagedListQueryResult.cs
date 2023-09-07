namespace Paradise.DataAccess.Repositories;

/// <summary>
/// Represents a paged list query result for the <typeparamref name="TEntity"/> entity.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="PagedListQueryResult{TEntity}"/> class.
/// </remarks>
/// <param name="currentPageItems">
/// Current page items.
/// </param>
/// <param name="total">
/// Total number of records.
/// </param>
public sealed class PagedListQueryResult<TEntity>(IEnumerable<TEntity> currentPageItems, int total)
{
    #region Properties
    /// <summary>
    /// Current page items.
    /// </summary>
    public IEnumerable<TEntity> CurrentPageItems { get; set; } = currentPageItems;

    /// <summary>
    /// Total number of records.
    /// </summary>
    public int Total { get; set; } = total;
    #endregion
}