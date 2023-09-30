using Paradise.Localization.ModelsLocalization;
using Paradise.Models.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Paradise.Models;

/// <summary>
/// Represents an application error.
/// </summary>
public readonly struct ApplicationError : IEquatable<ApplicationError>
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationError"/> structure
    /// with the specified error code and description.
    /// </summary>
    /// <param name="code">
    /// Error code.
    /// </param>
    /// <param name="description">
    /// Error description.
    /// </param>
    [JsonConstructor]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors not working with constructor attributes.")]
    public ApplicationError(ErrorCode code, string description)
    {
        Code = code;
        Description = description;
        IsCritical = code.GetIsCritical();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Error code.
    /// </summary>
    public ErrorCode Code { get; }

    /// <summary>
    /// Error description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Indicates whether the error is critical.
    /// </summary>
    [JsonIgnore]
    public bool IsCritical { get; }
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
        var messageFormat = ModelsLocalizationMessages.ErrorToStringFormat;
        var code = (int)Code;

        return string.Format(CultureInfo.CurrentCulture,
                             messageFormat,
                             code,
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