using Paradise.Models.Extensions;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using static Paradise.Localization.DataRepresentation.RepresentationMessages;
using static System.Text.CompositeFormat;

namespace Paradise.Models;

/// <summary>
/// Represents an application error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationError"/> structure.
/// </remarks>
/// <param name="code">
/// Error code.
/// </param>
/// <param name="description">
/// Error description.
/// </param>
[method: JsonConstructor]
public readonly struct ApplicationError(ErrorCode code, string description) : IEquatable<ApplicationError>
{
    #region Fields
    private static readonly CompositeFormat _toStringFormat = Parse(ApplicationErrorToStringFormat);
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationError"/> structure.
    /// </summary>
    /// <param name="description">
    /// Error description.
    /// </param>
    /// <param name="isCritical">
    /// Indicates whether the error is critical.
    /// </param>
    internal ApplicationError(string description, bool isCritical) : this(default, description)
        => IsCritical = isCritical;
    #endregion

    #region Properties
    /// <summary>
    /// Error code.
    /// </summary>
    public ErrorCode Code { get; } = code;

    /// <summary>
    /// Error description.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// Indicates whether the error is critical.
    /// </summary>
    [JsonIgnore]
    public bool IsCritical { get; } = code.GetIsCritical();
    #endregion

    #region Public methods
    /// <summary>
    /// Returns the string representation of the current instance.
    /// </summary>
    /// <returns>
    /// Human readable string, representing the error information:
    /// <c>
    /// 'Code: 1 | Description: Some error information.'
    /// </c>
    /// </returns>
    public override string ToString()
    {
        var codeNumber = (int)Code;

        return string.Format(CultureInfo.CurrentCulture,
                             _toStringFormat,
                             codeNumber,
                             Description);
    }

    /// <inheritdoc/>
    public bool Equals(ApplicationError other)
        => other == this;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is ApplicationError applicationError && Equals(applicationError);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Code, Description);
    #endregion

    #region Operators
    /// <summary>
    /// Implicitly converts the given <paramref name="error"/>
    /// into a <see cref="string"/> by calling <see cref="ToString"/> method.
    /// </summary>
    /// <param name="error">
    /// The <see cref="ApplicationError"/> to be converted into a <see cref="string"/>.
    /// </param>
    public static implicit operator string(ApplicationError error)
        => error.ToString();

    /// <summary>
    /// Compares the given <paramref name="left"/> and <paramref name="right"/>
    /// objects for equality.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="ApplicationError"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="ApplicationError"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> equals <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator ==(ApplicationError left, ApplicationError right)
        => left.Code == right.Code
        && left.Description == right.Description;

    /// <summary>
    /// Compares the given <paramref name="left"/> and <paramref name="right"/>
    /// objects for inequality.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="ApplicationError"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="ApplicationError"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> does not equal <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator !=(ApplicationError left, ApplicationError right)
        => !(left == right);
    #endregion
}