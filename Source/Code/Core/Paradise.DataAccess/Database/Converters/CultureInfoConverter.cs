using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;
using System.Linq.Expressions;

namespace Paradise.DataAccess.Database.Converters;

/// <inheritdoc/>
internal sealed class CultureInfoConverter : ValueConverter<CultureInfo?, int?>
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="CultureInfoConverter"/> class.
    /// </summary>
    public CultureInfoConverter() : base(ConvertTo, ConvertFrom) { }
    #endregion

    #region Private methods
    /// <summary>
    /// <see cref="CultureInfo"/> into a <see cref="int"/> conversion expression.
    /// </summary>
    private static Expression<Func<CultureInfo?, int?>> ConvertTo
        => culture => culture == null ? null : culture.LCID;

    /// <summary>
    /// <see cref="int"/> into a <see cref="CultureInfo"/> conversion expression.
    /// </summary>
    private static Expression<Func<int?, CultureInfo?>> ConvertFrom
        => lcid => lcid == null ? null : CultureInfo.GetCultureInfo(lcid.Value);
    #endregion
}