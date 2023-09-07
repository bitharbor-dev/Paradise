using Microsoft.EntityFrameworkCore;
using Paradise.DataAccess.Extensions;
using Paradise.Localization.ExceptionsHandling;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Paradise.DataAccess.Repositories;

/// <summary>
/// Represents a paged list query for the <typeparamref name="TEntity"/> entity.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
public sealed class PagedListQuery<TEntity> where TEntity : class
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

    #region Fields
    private int _pageNumber = DefaultPageNumber;
    private int _pageSize = DefaultPageSize;
    #endregion

    #region Properties
    /// <summary>
    /// Page size.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set
        {
            ThrowIfValueIsLessOrEqualToZero(value);

            _pageSize = value;
        }
    }

    /// <summary>
    /// Page number.
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            ThrowIfValueIsLessOrEqualToZero(value);

            _pageNumber = value;
        }
    }

    /// <summary>
    /// Indicates whether the output list should be ordered
    /// ascending or descending.
    /// </summary>
    public bool OrderAscending { get; set; } = true;

    /// <summary>
    /// Property name by which the items should be ordered.
    /// </summary>
    /// <remarks>
    /// No ordering is applied if set to <see langword="null"/>.
    /// </remarks>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Additional filter.
    /// </summary>
    public Expression<Func<TEntity, bool>>? Filter { get; set; }

    /// <summary>
    /// The list of entity's navigation properties
    /// to be included into the query.
    /// </summary>
    public IEnumerable<string> NavigationProperties { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The list of entity's properties to lookup
    /// through with the <see cref="LookupValue"/>.
    /// </summary>
    public IEnumerable<string> LookupProperties { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The value to be searched through the
    /// <see cref="LookupProperties"/> with.
    /// </summary>
    public string? LookupValue { get; set; }

    /// <summary>
    /// Calculates the page skip to be used in the query.
    /// </summary>
    internal int PageSkip
        => _pageNumber > 1 ? (_pageNumber - 1) * _pageSize : 0;
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
        foreach (var includedProperty in NavigationProperties)
            queryable = queryable.Include(includedProperty);

        if (Filter is not null)
            queryable = queryable.Where(Filter);

        queryable = queryable
            .FilterBy(LookupProperties, LookupValue)
            .OrderByPropertyName(OrderBy, OrderAscending);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the given
    /// <paramref name="value"/> is less or equal to 0.
    /// </summary>
    /// <param name="value">
    /// An <see cref="int"/> value to be checked.
    /// </param>
    /// <param name="propertyName">
    /// Property name, which value to be checked..
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the given <paramref name="value"/> is
    /// less or equal to 0.
    /// </exception>
    private static void ThrowIfValueIsLessOrEqualToZero(int value, [CallerMemberName] string? propertyName = null)
    {
        if (value > 0)
            return;

        var messageFormat = ExceptionMessages.ValueCanNotBeLessOrEqualToZero;
        var message = string.Format(CultureInfo.CurrentCulture, messageFormat, propertyName);

        throw new ArgumentException(message);
    }
    #endregion
}