using Paradise.Domain.Base;

namespace Paradise.DataAccess.Repositories;

/// <summary>
/// Represents the result of a paged list query for the <typeparamref name="TEntity"/> entity.
/// </summary>
/// <typeparam name="TEntity">
/// The entity type.
/// </typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="PagedListQueryResult{TEntity}"/> class.
/// </remarks>
/// <param name="currentPageItems">
/// The items contained in the current page.
/// </param>
/// <param name="total">
/// The total number of records across all pages.
/// </param>
public sealed class PagedListQueryResult<TEntity>(IEnumerable<TEntity> currentPageItems, int total)
    where TEntity : class, IDomainObject
{
    #region Properties
    /// <summary>
    /// The items contained in the current page.
    /// </summary>
    public IEnumerable<TEntity> CurrentPageItems { get; } = currentPageItems;

    /// <summary>
    /// The total number of records across all pages.
    /// </summary>
    public int Total { get; } = total;
    #endregion
}