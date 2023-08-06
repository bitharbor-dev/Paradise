using Paradise.Models;
using System.Net;
using static System.Environment;

namespace Paradise.ApplicationLogic.Tests;

/// <summary>
/// Contains extension methods for the <see cref="Result"/> class.
/// </summary>
internal static class ResultExtensions
{
    #region Public methods
    /// <summary>
    /// Asserts that the input <paramref name="result"/>
    /// has the given <paramref name="statusCode"/>,
    /// its value does not equal <see langword="null"/>,
    /// and the <see cref="Result.IsSuccess"/> equals <see langword="true"/>.
    /// </summary>
    /// <param name="result">
    /// The input <see cref="Result"/> instance.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value
    /// to be compared to the input <paramref name="result"/> status code.
    /// </param>
    public static void AssertSuccess(this Result result, HttpStatusCode? statusCode)
    {
        Assert.True(result.IsSuccess, string.Join(NewLine, result.Errors));
        Assert.Equal(statusCode, result.StatusCode);
    }

    /// <summary>
    /// Asserts that the input <paramref name="result"/>
    /// has the given <paramref name="statusCode"/>,
    /// its value equals <see langword="null"/>,
    /// the <see cref="Result.IsSuccess"/> equals <see langword="false"/>,
    /// and the <see cref="Result.Errors"/> contains
    /// the given <paramref name="errors"/> sequence,
    /// </summary>
    /// <param name="result">
    /// The input <see cref="Result"/> instance.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value
    /// to be compared to the input <paramref name="result"/> status code.
    /// </param>
    /// <param name="errors">
    /// The <see cref="ErrorCode"/> sequence to be checked for existence
    /// in the input <paramref name="result"/>.
    /// </param>
    public static void AssertFail(this Result result, HttpStatusCode statusCode, params ErrorCode[] errors)
    {
        var expectedStatusCode = statusCode;
        var expectedErrors = errors.Order();

        var actualStatusCode = result.StatusCode;
        var actualErrors = result.Errors.Select(error => error.Code).Order();

        var expectedErrorsString = $"Expected: {string.Join(", ", expectedErrors)}";
        var actualErrorsString = $"Actual: {string.Join(", ", actualErrors)}";

        var message = $"{expectedErrorsString}{NewLine}{actualErrorsString}";

        Assert.False(result.IsSuccess);
        Assert.Equal(expectedStatusCode, actualStatusCode);
        Assert.True(expectedErrors.SequenceEqual(actualErrors), message);
    }
    #endregion
}