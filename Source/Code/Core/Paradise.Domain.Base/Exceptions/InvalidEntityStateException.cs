using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static Paradise.Localization.ExceptionsHandling.ExceptionMessages;

namespace Paradise.Domain.Base.Exceptions;

/// <summary>
/// Represents errors that occur during domain layer CRUD operations.
/// </summary>
public sealed class InvalidEntityStateException : Exception
{
    #region Constants
    /// <summary>
    /// Separator string to be used during primary exception message
    /// and additional information concatenation.
    /// </summary>
    private static readonly string ExceptionMessageSeparator = $"{Environment.NewLine}{InvalidEntityStateAdditionalInformation}";
    #endregion

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
    private static string CreateExceptionMessage(Type entityType, object? value, string? additionalInformation, string? propertyName)
    {
        var message = string.Format(InvalidEntityState, entityType.Name, propertyName, value);

        if (!string.IsNullOrWhiteSpace(additionalInformation))
            message = string.Concat(message, ExceptionMessageSeparator, additionalInformation);

        return message;
    }
    #endregion
}