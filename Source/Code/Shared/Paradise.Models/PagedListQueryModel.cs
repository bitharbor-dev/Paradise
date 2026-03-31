using Paradise.Localization.ExceptionHandling;
using System.Text.Json.Serialization;

namespace Paradise.Models;

/// <summary>
/// Represents a query for retrieving a paged list of items,
/// with support for ordering and lookup filtering.
/// </summary>
public class PagedListQueryModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="PagedListQueryModel"/> class.
    /// </summary>
    /// <param name="pageSize">
    /// The number of items per page.
    /// <para>
    /// Must be greater than zero.
    /// </para>
    /// </param>
    /// <param name="pageNumber">
    /// The page number.
    /// <para>
    /// Must be greater than zero.
    /// </para>
    /// </param>
    /// <param name="orderAscending">
    /// Indicates whether the results should be sorted
    /// in ascending (<see langword="true"/>)
    /// or descending (<see langword="false"/>) order.
    /// </param>
    /// <param name="orderBy">
    /// The property name used for ordering.
    /// <para>
    /// If <see langword="null"/>, no ordering is applied.
    /// </para>
    /// </param>
    /// <param name="lookupProperties">
    /// The list of entity properties to search within
    /// when applying the <see cref="LookupValue"/>.
    /// </param>
    /// <param name="lookupValue">
    /// The value to be searched across
    /// the <see cref="LookupProperties"/>.
    /// </param>
    [JsonConstructor]
    public PagedListQueryModel(int pageSize, int pageNumber, bool orderAscending, string? orderBy,
                               IEnumerable<string> lookupProperties, string? lookupValue)
    {
        ThrowIfValueIsLessThanOrEqualToZero(pageSize, nameof(PageSize));
        ThrowIfValueIsLessThanOrEqualToZero(pageNumber, nameof(PageNumber));

        PageSize = pageSize;
        PageNumber = pageNumber;
        OrderAscending = orderAscending;
        OrderBy = orderBy;
        LookupProperties = lookupProperties;
        LookupValue = lookupValue;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The number of items per page.
    /// <para>
    /// Must be greater than zero.
    /// </para>
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// The page number.
    /// <para>
    /// Must be greater than zero.
    /// </para>
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Indicates whether the results should be sorted
    /// in ascending (<see langword="true"/>)
    /// or descending (<see langword="false"/>) order.
    /// </summary>
    public bool OrderAscending { get; }

    /// <summary>
    /// The property name used for ordering.
    /// <para>
    /// If <see langword="null"/>, no ordering is applied.
    /// </para>
    /// </summary>
    public string? OrderBy { get; }

    /// <summary>
    /// The list of entity properties to search within
    /// when applying the <see cref="LookupValue"/>.
    /// </summary>
    public virtual IEnumerable<string> LookupProperties { get; }

    /// <summary>
    /// The value to be searched across
    /// the <see cref="LookupProperties"/>.
    /// </summary>
    public string? LookupValue { get; }
    #endregion

    #region Private methods
    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the given
    /// <paramref name="value"/> is less than or equal to 0.
    /// </summary>
    /// <param name="value">
    /// An <see cref="int"/> value to be checked.
    /// </param>
    /// <param name="propertyName">
    /// Property name, which value to be checked.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the given <paramref name="value"/> is
    /// less than or equal to 0.
    /// </exception>
    private static void ThrowIfValueIsLessThanOrEqualToZero(int value, string propertyName)
    {
        if (value <= 0)
        {
            var message = ExceptionMessages.GetMessageValueCanNotBeLessOrEqualToZero(propertyName);

            throw new ArgumentException(message);
        }
    }
    #endregion
}