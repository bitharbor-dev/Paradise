using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Paradise.Models.WebApi.Services.Authentication;

/// <summary>
/// Two-factor authentication schema.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TwoFactorAuthenticationModel"/> class.
/// </remarks>
/// <param name="twoFactorCode">
/// The code to be used to confirm the user's login.
/// </param>
/// <param name="identityToken">
/// The value to be used to determine the user
/// and validate a login request.
/// </param>
[method: JsonConstructor]
public sealed class TwoFactorAuthenticationModel(string twoFactorCode, string identityToken)
{
    #region Properties
    /// <summary>
    /// The code to be used to confirm the user's login.
    /// </summary>
    [DataType(DataType.Text)]
    public string TwoFactorCode { get; } = twoFactorCode;

    /// <summary>
    /// The value to be used to determine the user
    /// and validate a login request.
    /// </summary>
    [DataType(DataType.Text)]
    public string IdentityToken { get; } = identityToken;
    #endregion
}