using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Paradise.Models.Domain.UserModels;

/// <summary>
/// User two-factor authentication schema.
/// </summary>
public sealed class UserTwoFactorAuthenticationModel
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="UserTwoFactorAuthenticationModel"/> class.
    /// </summary>
    /// <param name="twoFactorCode">
    /// The code to be used to confirm the user's login.
    /// </param>
    /// <param name="identityToken">
    /// The value to be used to determine the user
    /// and validate a login request.
    /// </param>
    [JsonConstructor]
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructors not working with constructor attributes.")]
    public UserTwoFactorAuthenticationModel(string twoFactorCode, string identityToken)
    {
        TwoFactorCode = twoFactorCode;
        IdentityToken = identityToken;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The code to be used to confirm the user's login.
    /// </summary>
    [Required]
    public string TwoFactorCode { get; set; }

    /// <summary>
    /// The value to be used to determine the user
    /// and validate a login request.
    /// </summary>
    [Required]
    public string IdentityToken { get; set; }
    #endregion
}