using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Paradise.ApplicationLogic.Services.Identity.Users;

namespace Paradise.WebApi.Pages;

/// <summary>
/// Reset email address page model.
/// </summary>
internal sealed class ResetEmailAddressModel : PageModel
{
    #region Properties
    /// <summary>
    /// The value to be used to determine the user
    /// and validate email address reset request.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? IdentityToken { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// A successful email address reset indication flag.
    /// </summary>
    public bool IsSuccess { get; set; }
    #endregion

    #region Public methods
    /// <summary>
    /// Page load handler.
    /// </summary>
    /// <param name="userService">
    /// The <see cref="IUserService"/> used to reset the user's email address.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public async Task OnGetAsync([FromServices] IUserService userService)
    {
        if (!ModelState.IsValid)
            return;

        var result = await userService.ResetEmailAddressAsync(IdentityToken!)
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