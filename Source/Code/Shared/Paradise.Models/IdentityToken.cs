using Paradise.Common.Extensions;
using Paradise.Localization.ExceptionHandling;
using System.Text.Json.Serialization;

namespace Paradise.Models;

/// <summary>
/// Represents an application value-token.
/// </summary>
/// <remarks>
/// This class represents a transient token used in identity-related operations
/// such as email or password reset, or passing encrypted state through the web.
/// </remarks>
public sealed class IdentityToken
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityToken"/> class.
    /// </summary>
    /// <param name="emailAddress">
    /// Email address.
    /// </param>
    /// <param name="innerToken">
    /// Inner token.
    /// </param>
    /// <param name="newValue">
    /// The new value associated with the token, if any.
    /// </param>
    /// <param name="expiryDate">
    /// Token expiry date.
    /// <para>
    /// <see langword="null"/> value means that token never expires.
    /// </para>
    /// </param>
    [JsonConstructor]
    public IdentityToken(string emailAddress, string innerToken, string? newValue = null, DateTimeOffset? expiryDate = null)
    {
        if (!emailAddress.IsValidEmailAddress())
        {
            var message = ExceptionMessages.GetMessageInvalidEmailAddress(emailAddress);

            throw new ArgumentException(message);
        }

        EmailAddress = emailAddress;
        InnerToken = innerToken;
        NewValue = newValue;
        ExpiryDate = expiryDate;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Email address.
    /// </summary>
    public string EmailAddress { get; }

    /// <summary>
    /// Inner token.
    /// </summary>
    public string InnerToken { get; }

    /// <summary>
    /// The new value associated with the token, if any.
    /// </summary>
    public string? NewValue { get; }

    /// <summary>
    /// Token expiry date.
    /// <para>
    /// <see langword="null"/> value means that token never expires.
    /// </para>
    /// </summary>
    public DateTimeOffset? ExpiryDate { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// Gets the <see langword="bool"/> value indicating
    /// whether the current identity token is expired.
    /// </summary>
    /// <param name="currentTime">
    /// Current time.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the current identity token is expired,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public bool IsExpired(DateTimeOffset currentTime)
        => ExpiryDate < currentTime;
    #endregion
}