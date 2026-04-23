using Microsoft.EntityFrameworkCore;
using Paradise.DataAccess.Repositories.Base.Extensions;
using Paradise.Domain.Base;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Repositories;

/// <summary>
/// Represents a query for retrieving a paged list of items,
/// with support for ordering and lookup filtering.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public sealed class PagedListQuery<TEntity> where TEntity : class, IDomainObject
{
    #region Constants
    /// <summary>
    /// Default page number.
    /// </summary>
    public const ushort DefaultPageNumber = 1;
    /// <summary>
    /// Default page size.
    /// </summary>
    public const int DefaultPageSize = 10;
    #endregion

    #region Properties
    /// <summary>
    /// The number of items per page.
    /// <para>
    /// Must be greater than zero.
    /// </para>
    /// </summary>
    public int PageSize
    {
        get;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0);

            field = value;
        }
    } = DefaultPageSize;

    /// <summary>
    /// The page number.
    /// <para>
    /// Must be greater than zero.
    /// </para>
    /// </summary>
    public int PageNumber
    {
        get;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0);

            field = value;
        }
    } = DefaultPageNumber;

    /// <summary>
    /// Indicates whether the results should be sorted
    /// in ascending (<see langword="true"/>)
    /// or descending (<see langword="false"/>) order.
    /// </summary>
    public bool OrderAscending { get; set; } = true;

    /// <summary>
    /// The property name used for ordering.
    /// <para>
    /// If <see langword="null"/>, no ordering is applied.
    /// </para>
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Additional filter.
    /// </summary>
    public Expression<Func<TEntity, bool>>? Filter { get; set; }

    /// <summary>
    /// Additional entity projection.
    /// </summary>
    public Expression<Func<TEntity, TEntity>>? Projection { get; set; }

    /// <summary>
    /// The list of entity's navigation properties
    /// to be included into the query.
    /// </summary>
    public IEnumerable<string> NavigationProperties { get; set; } = [];

    /// <summary>
    /// The list of entity properties to search within
    /// when applying the <see cref="LookupValue"/>.
    /// </summary>
    public IEnumerable<string> LookupProperties { get; set; } = [];

    /// <summary>
    /// The value to be searched across
    /// the <see cref="LookupProperties"/>.
    /// </summary>
    public string? LookupValue { get; set; }

    /// <summary>
    /// Calculates the page skip to be used in the query.
    /// </summary>
    internal int PageSkip
        => PageNumber > 1 ? (PageNumber - 1) * PageSize : 0;
    #endregion

    #region Internal methods
    /// <summary>
    /// Applies current query parameters the given <paramref name="queryable"/>.
    /// </summary>
    /// <param name="queryable">
    /// A <see cref="IQueryable{T}"/> of <typeparamref name="TEntity"/>
    /// to winch the query parameters to be applied.
    /// </param>
    internal void Apply(ref IQueryable<TEntity> queryable)
    {
        foreach (var navigationProperty in NavigationProperties)
            queryable = queryable.Include(navigationProperty);

        if (Filter is not null)
            queryable = queryable.Where(Filter);

        queryable = queryable
            .FilterBy(LookupProperties, LookupValue)
            .OrderByPropertyName(OrderBy ?? nameof(IDomainObject.Created), OrderAscending);

        if (Projection is not null)
            queryable = queryable.AsNoTracking().Select(Projection);
    }
    #endregion
}