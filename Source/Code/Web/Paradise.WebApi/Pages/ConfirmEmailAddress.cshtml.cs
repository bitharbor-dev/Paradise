using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Paradise.ApplicationLogic.Services.Identity.Users;

namespace Paradise.WebApi.Pages;

/// <summary>
/// Confirm email address page model.
/// </summary>
internal sealed class ConfirmEmailAddressModel : PageModel
{
    #region Properties
    /// <summary>
    /// The value to be used to determine the user
    /// and confirm email address.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? IdentityToken { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// A successful email address confirmation indication flag.
    /// </summary>
    public bool IsSuccess { get; set; }
    #endregion

    #region Public methods
    /// <summary>
    /// Page load handler.
    /// </summary>
    /// <param name="userService">
    /// The <see cref="IUserService"/> used to confirm the user's email address.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public async Task OnGetAsync([FromServices] IUserService userService)
    {
        if (!ModelState.IsValid)
            return;

        var result = await userService.ConfirmEmailAddressAsync(IdentityToken!)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            ErrorMessage = string.Join(Environment.NewLine, result.Errors.Select(error => error.Description));
            return;
        }

        IsSuccess = true;
    }
    #endregion
}