using Microsoft.AspNetCore.Identity;
using Paradise.Models;

namespace Paradise.ApplicationLogic.Infrastructure.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IdentityResult"/> <see langword="class"/>.
/// </summary>
public static class IdentityResultExtensions
{
    #region Public methods
    /// <summary>
    /// Converts an <see cref="IdentityResult"/> into a <see cref="Result"/> instance.
    /// </summary>
    /// <param name="identityResult">
    /// The <see cref="IdentityResult"/> returned from an ASP.NET Identity operation.
    /// </param>
    /// <param name="status">
    /// Indicates the result status. Provides more information on the nature of the errors, in case of any.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> containing the mapped errors from <paramref name="identityResult"/>
    /// and the specified <paramref name="status"/>.
    /// </returns>
    public static Result GetResult(this IdentityResult identityResult, OperationStatus status = OperationStatus.Failure)
    {
        ArgumentNullException.ThrowIfNull(identityResult);

        var errors = identityResult
            .Errors
            .Select(error => new ApplicationError(ErrorCode.DefaultError, error.Description));

        return new(status, errors);
    }

    /// <summary>
    /// Converts an <see cref="IdentityResult"/> into a <see cref="Result{T}"/> instance with a default value.
    /// </summary>
    /// <typeparam name="TValue">
    /// The type of the value that the <see cref="Result{TValue}"/> can hold.
    /// </typeparam>
    /// <param name="identityResult">
    /// The <see cref="IdentityResult"/> returned from an ASP.NET Identity operation.
    /// </param>
    /// <param name="status">
    /// Indicates the result status. Provides more information on the nature of the errors, in case of any.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the mapped errors from <paramref name="identityResult"/>,
    /// a default value of type <typeparamref name="TValue"/>, and the specified <paramref name="status"/>.
    /// </returns>
    public static Result<TValue> GetResult<TValue>(this IdentityResult identityResult, OperationStatus status = OperationStatus.Failure)
    {
        ArgumentNullException.ThrowIfNull(identityResult);

        var errors = identityResult
            .Errors
            .Select(error => new ApplicationError(ErrorCode.DefaultError, error.Description));

        return new(status, errors, default);
    }
    #endregion
}