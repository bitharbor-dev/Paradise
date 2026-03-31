using Paradise.Models;

namespace Paradise.WebApi.Infrastructure.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ResultBase"/> <see langword="class"/>.
/// </summary>
public static class ResultBaseExtensions
{
    #region Public methods
    /// <summary>
    /// Converts the given <paramref name="result"/> into
    /// a new <see cref="ApplicationActionResult"/> instance.
    /// </summary>
    /// <param name="result">
    /// The <see cref="ResultBase"/> to convert.
    /// </param>
    /// <returns>
    /// New <see cref="ApplicationActionResult"/> instance.
    /// </returns>
    public static ApplicationActionResult AsActionResult(this ResultBase result)
        => new(result);
    #endregion
}