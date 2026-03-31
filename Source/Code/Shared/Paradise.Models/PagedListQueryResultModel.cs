using System.Text.Json.Serialization;

namespace Paradise.Models;

/// <summary>
/// Represents the result of a paged list query for the <typeparamref name="T"/> entity.
/// </summary>
/// <typeparam name="T">
/// The entity type.
/// </typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="PagedListQueryResultModel{T}"/> class.
/// </remarks>
/// <param name="currentPageItems">
/// The items contained in the current page.
/// </param>
/// <param name="total">
/// The total number of records across all pages.
/// </param>
[method: JsonConstructor]
public sealed class PagedListQueryResultModel<T>(IEnumerable<T> currentPageItems, int total)
{
    #region Properties
    /// <summary>
    /// The items contained in the current page.
    /// </summary>
    public IEnumerable<T> CurrentPageItems { get; } = currentPageItems;

    /// <summary>
    /// The total number of records across all pages.
    /// </summary>
    public int Total { get; } = total;
    #endregion
}