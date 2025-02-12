using Microsoft.AspNetCore.Identity;

namespace Paradise.Models.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IdentityResult"/> <see langword="class"/>.
/// </summary>
public static class IdentityResultExtensions
{
    #region Public methods
    /// <summary>
    /// Gets the list of <see cref="ApplicationError"/> instances from the given
    /// <paramref name="result"/>.
    /// </summary>
    /// <param name="result">
    /// The <see cref="IdentityResult"/> to get errors from.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="ApplicationError"/> representing
    /// the given <paramref name="result"/>.
    /// </returns>
    public static IEnumerable<ApplicationError> AsErrors(this IdentityResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        foreach (var error in result.Errors)
        {
            if (Enum.TryParse(error.Code, out ErrorCode errorCode))
                yield return new(errorCode, error.Description);
        }
    }
    #endregion
}