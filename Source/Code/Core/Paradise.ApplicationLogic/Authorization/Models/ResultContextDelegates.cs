namespace Paradise.ApplicationLogic.Authorization.Models;

/// <summary>
/// A container class to be used to
/// pass down methods invoked during authentication process.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ResultContextDelegates"/> class.
/// </remarks>
/// <param name="success">
/// Calls success creating a ticket with the principal and properties.
/// </param>
/// <param name="noResult">
/// Indicates that there was no information returned for this authentication scheme.
/// </param>
/// <param name="failWithException">
/// Indicates that there was a failure during authentication.
/// </param>
/// <param name="failWithMessage">
/// Indicates that there was a failure during authentication.
/// </param>
public sealed class ResultContextDelegates(Action success,
                                           Action noResult,
                                           Action<Exception> failWithException,
                                           Action<string> failWithMessage)
{
    #region Properties
    /// <summary>
    /// Calls success creating a ticket with the principal and properties.
    /// </summary>
    public Action Success { get; set; } = success;

    /// <summary>
    /// Indicates that there was no information returned for this authentication scheme.
    /// </summary>
    public Action NoResult { get; set; } = noResult;

    /// <summary>
    /// Indicates that there was a failure during authentication.
    /// </summary>
    public Action<Exception> FailWithException { get; set; } = failWithException;

    /// <summary>
    /// Indicates that there was a failure during authentication.
    /// </summary>
    public Action<string> FailWithMessage { get; set; } = failWithMessage;
    #endregion
}