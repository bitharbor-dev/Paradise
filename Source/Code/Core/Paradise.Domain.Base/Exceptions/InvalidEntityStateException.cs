using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static Paradise.Localization.ExceptionsHandling.ExceptionMessagesProvider;

namespace Paradise.Domain.Base.Exceptions;

/// <summary>
/// Represents errors that occur during domain layer CRUD operations.
/// </summary>
public sealed class InvalidEntityStateException : Exception
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEntityStateException"/> class.
    /// </summary>
    /// <param name="entityType">
    /// Entity type.
    /// </param>
    /// <param name="value">
    /// Property value.
    /// </param>
    /// <param name="additionalInformation">
    /// Additional information to be captured into exception message.
    /// </param>
    /// <param name="propertyName">
    /// Property name.
    /// </param>
    private InvalidEntityStateException(Type entityType, object? value, string? additionalInformation, string? propertyName)
        : base(CreateExceptionMessage(entityType, value, additionalInformation, propertyName)) { }
    #endregion

    #region Public methods
    /// <summary>
    /// Throws an <see cref="InvalidEntityStateException"/>.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entity type.
    /// </typeparam>
    /// <param name="value">
    /// Property value.
    /// </param>
    /// <param name="propertyName">
    /// Property name.
    /// </param>
    /// <param name="additionalInformation">
    /// Additional information to be captured into exception message.
    /// </param>
    [DoesNotReturn]
    public static void Throw<TEntity>(object? value, string? additionalInformation = null, [CallerArgumentExpression(nameof(value))] string? propertyName = null)
        where TEntity : IDatabaseRecord
        => throw new InvalidEntityStateException(typeof(TEntity), value, additionalInformation, propertyName);
    #endregion

    #region Private methods
    /// <summary>
    /// Creates exception message based on the input arguments.
    /// </summary>
    /// <param name="entityType">
    /// Entity type.
    /// </param>
    /// <param name="value">
    /// Entity's property value.
    /// </param>
    /// <param name="additionalInformation">
    /// Optional additional information.
    /// </param>
    /// <param name="propertyName">
    /// Entity's property name.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> value containing exception
    /// message based on the input arguments.
    /// </returns>
    private static string CreateExceptionMessage(Type entityType, object? value, string? additionalInformation, string? propertyName)
    {
        var exceptionMessageSeparator = $"{Environment.NewLine}{GetInvalidEntityStateAdditionalInformationMessage()}";

        var message = GetInvalidEntityStateMessage(entityType, propertyName, value);

        if (!string.IsNullOrWhiteSpace(additionalInformation))
            message = string.Concat(message, exceptionMessageSeparator, additionalInformation);

        return message;
    }
    #endregion
}