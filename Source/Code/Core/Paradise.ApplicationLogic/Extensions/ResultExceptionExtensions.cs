using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Exceptions;
using Paradise.Common.Extensions;
using Paradise.Models;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Paradise.ApplicationLogic.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ResultException"/> <see langword="class"/>.
/// </summary>
internal static class ResultExceptionExtensions
{
    #region Public methods
    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="value"/>
    /// equals <see langword="true"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="bool"/> value to be tested.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    public static void ThrowIfTrue([DoesNotReturnIf(true)] this bool value, HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
    {
        if (value)
            throw new ResultException(statusCode, errorCode, args);
    }

    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="value"/>
    /// equals <see langword="false"/>.
    /// </summary>
    /// <param name="value">
    /// The <see cref="bool"/> value to be tested.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    public static void ThrowIfFalse([DoesNotReturnIf(false)] this bool value, HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
        => ThrowIfTrue(!value, statusCode, errorCode, args);

    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="value"/>
    /// equals <see langword="null"/> or whitespace.
    /// </summary>
    /// <param name="value">
    /// The <see cref="string"/> value to be tested.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    public static void ThrowIfNullOrWhiteSpace([NotNull] this string? value, HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
        => ThrowIfTrue(value.IsNullOrWhiteSpace(), statusCode, errorCode, args);

    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="value"/>
    /// equals is empty or whitespace.
    /// </summary>
    /// <param name="value">
    /// The <see cref="string"/> value to be tested.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    public static void ThrowIfEmptyOrWhiteSpace(this string value, HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
        => ThrowIfTrue(value.Length is 0 || value.All(c => c is ' '), statusCode, errorCode, args);

    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="identityResult"/>
    /// is unsuccessful.
    /// </summary>
    /// <param name="identityResult">
    /// The <see cref="IdentityResult"/> to add errors from.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    public static void ThrowIfUnsuccessfulIdentityResult(this IdentityResult identityResult, HttpStatusCode statusCode = HttpStatusCode.ServiceUnavailable)
    {
        if (!identityResult.Succeeded)
            ResultException.Throw(identityResult, statusCode);
    }

    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="sequence"/>
    /// contains no elements.
    /// </summary>
    /// <typeparam name="T">
    /// Sequence element type.
    /// </typeparam>
    /// <param name="sequence">
    /// The <see cref="IEnumerable{T}"/> to be tested.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    public static void ThrowIfEmpty<T>(this IEnumerable<T> sequence, HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
        => ThrowIfFalse(sequence.Any(), statusCode, errorCode, args);

    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="sequence"/>
    /// equals <see langword="null"/> or contains no elements.
    /// </summary>
    /// <typeparam name="T">
    /// Sequence element type.
    /// </typeparam>
    /// <param name="sequence">
    /// The <see cref="IEnumerable{T}"/> to be tested.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    public static void ThrowIfNullOrEmpty<T>([NotNull] this IEnumerable<T>? sequence, HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
        => ThrowIfFalse(sequence is not null && sequence.Any(), statusCode, errorCode, args);

    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="sequence"/>
    /// contains any elements.
    /// </summary>
    /// <typeparam name="T">
    /// Sequence element type.
    /// </typeparam>
    /// <param name="sequence">
    /// The <see cref="IEnumerable{T}"/> to be tested.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    public static void ThrowIfNotEmpty<T>(this IEnumerable<T> sequence, HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
        => ThrowIfTrue(sequence.Any(), statusCode, errorCode, args);

    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="value"/>
    /// equals <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Value type.
    /// </typeparam>
    /// <param name="value">
    /// The <typeparamref name="T"/> value to be tested.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    public static void ThrowIfNull<T>([NotNull] this T? value, HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
        => ThrowIfTrue(value is null, statusCode, errorCode, args);

    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="value"/>
    /// does not equal <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Value type.
    /// </typeparam>
    /// <param name="value">
    /// The <typeparamref name="T"/> value to be tested.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    public static void ThrowIfNotNull<T>(this T? value, HttpStatusCode statusCode, ErrorCode errorCode, params object?[] args)
        => ThrowIfTrue(value is not null, statusCode, errorCode, args);

    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="value"/>
    /// is equal to <paramref name="comparisonValue"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Value type.
    /// </typeparam>
    /// <param name="value">
    /// The <typeparamref name="T"/> value to be tested.
    /// </param>
    /// <param name="comparisonValue">
    /// The <typeparamref name="T"/> value to be compared with.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="equalityComparer">
    /// The <see cref="IEqualityComparer{T}"/> to use.
    /// Leave <see langword="null"/> to use <see cref="EqualityComparer{T}.Default"/>.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    public static void ThrowIfEqual<T>(this T? value, T? comparisonValue, HttpStatusCode statusCode, ErrorCode errorCode, IEqualityComparer<T>? equalityComparer = null, params object?[] args)
    {
        equalityComparer ??= EqualityComparer<T>.Default;

        var valuesAreEqual = equalityComparer.Equals(comparisonValue, value);

        ThrowIfTrue(valuesAreEqual, statusCode, errorCode, args);
    }

    /// <summary>
    /// Throws a <see cref="ResultException"/> if the given <paramref name="value"/>
    /// is not equal to <paramref name="comparisonValue"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Value type.
    /// </typeparam>
    /// <param name="value">
    /// The <typeparamref name="T"/> value to be tested.
    /// </param>
    /// <param name="comparisonValue">
    /// The <typeparamref name="T"/> value to be compared with.
    /// </param>
    /// <param name="statusCode">
    /// The <see cref="HttpStatusCode"/> value to be set.
    /// </param>
    /// <param name="errorCode">
    /// <see cref="ErrorCode"/> that references
    /// the corresponding error description.
    /// </param>
    /// <param name="equalityComparer">
    /// The <see cref="IEqualityComparer{T}"/> to use.
    /// Leave <see langword="null"/> to use <see cref="EqualityComparer{T}.Default"/>.
    /// </param>
    /// <param name="args">
    /// An object array that contains zero or more objects to format.
    /// </param>
    public static void ThrowIfNotEqual<T>(this T? value, T? comparisonValue, HttpStatusCode statusCode, ErrorCode errorCode, IEqualityComparer<T>? equalityComparer = null, params object?[] args)
    {
        equalityComparer ??= EqualityComparer<T>.Default;

        var valuesAreEqual = equalityComparer.Equals(comparisonValue, value);

        ThrowIfFalse(valuesAreEqual, statusCode, errorCode, args);
    }
    #endregion
}