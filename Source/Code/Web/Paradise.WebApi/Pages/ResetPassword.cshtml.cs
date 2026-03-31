using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Paradise.ApplicationLogic.Services.Identity.Users;
using System.ComponentModel.DataAnnotations;

namespace Paradise.WebApi.Pages;

/// <summary>
/// Reset password page model.
/// </summary>
internal sealed class ResetPasswordModel : PageModel
{
    #region Properties
    /// <summary>
    /// The value to be used to determine the user
    /// and validate password reset request.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? IdentityToken { get; set; }

    /// <summary>
    /// User's new password.
    /// </summary>
    [BindProperty, Required]
    public string? Password { get; set; }

    /// <summary>
    /// User's new password confirmation.
    /// </summary>
    [BindProperty, Required, Compare(nameof(Password))]
    public string? PasswordConfirmation { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// A successful password reset indication flag.
    /// </summary>
    public bool IsSuccess { get; set; }
    #endregion

    #region Public methods
    /// <summary>
    /// Submit button click handler.
    /// </summary>
    /// <param name="userService">
    /// The <see cref="IUserService"/> used to reset the user's password.
    /// </param>
    /// <returns>
    /// The same page reflecting the password reset result.
    /// </returns>
    public async Task<IActionResult> OnPostAsync([FromServices] IUserService userService)
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await userService.ResetPasswordAsync(new(IdentityToken!, Password!, PasswordConfirmation!))
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            ErrorMessage = string.Join(Environment.NewLine, result.Errors.Select(error => error.Description));
            return Page();
        }

        IsSuccess = true;
        return Page();
    }
    #endregion
}