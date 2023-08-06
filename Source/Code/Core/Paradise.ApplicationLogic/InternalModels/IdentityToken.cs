using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Localization.ExceptionsHandling;
using System.Text.Json.Serialization;

namespace Paradise.ApplicationLogic.InternalModels;

/// <summary>
/// Represents an application value-token.
/// </summary>
/// <remarks>
/// <para>
/// Don't be fooled, this model has nothing to do with <see cref="IdentityUserToken{TKey}"/> class.
/// </para>
/// This model is being used in operations like email or password reset
/// when you need to know who is the actor and what is the new value, but the
/// target property is defined by the corresponding method.
/// <para>
/// The new value is optional, since you may just want
/// to pass an encrypted token through the web.
/// </para>
/// </remarks>
internal sealed class IdentityToken
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityToken"/> class.
    /// </summary>
    /// <param name="email">
    /// Email.
    /// </param>
    /// <param name="innerToken">
    /// Inner token.
    /// </param>
    /// <param name="value">
    /// Optional value.
    /// </param>
    /// <param name="expiryDate">
    /// Token expiry date.
    /// <para>
    /// <see langword="null"/> value means that token never expires.
    /// </para>
    /// </param>
    [JsonConstructor]
    public IdentityToken(string email, string innerToken, string? value = null, DateTime? expiryDate = null)
    {
        if (!email.IsValidEmailAddress())
            throw new ArgumentException(string.Format(ExceptionMessages.InvalidEmailAddress, email));

        Email = email;
        InnerToken = innerToken;
        Value = value;
        ExpiryDate = expiryDate;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Optional value.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    /// Inner token.
    /// </summary>
    public string InnerToken { get; }

    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Token expiry date.
    /// <para>
    /// <see langword="null"/> value means that token never expires.
    /// </para>
    /// </summary>
    public DateTime? ExpiryDate { get; }
    #endregion

    #region Public methods
    /// <summary>
    /// Gets the <see langword="bool"/> value
    /// indicating whether the current identity token
    /// is outdated.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current identity token is outdated,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public bool IsOutdated()
        => ExpiryDate.HasValue && ExpiryDate < DateTime.UtcNow;
    #endregion
}