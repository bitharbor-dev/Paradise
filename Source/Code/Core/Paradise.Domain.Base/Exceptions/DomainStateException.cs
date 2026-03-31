using Paradise.Common.Extensions;
using System.Runtime.CompilerServices;
using System.Text;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.Domain.Base.Exceptions;

/// <summary>
/// Represents the errors that occur during domain layer CRUD operations.
/// </summary>
/// <typeparam name="TEntity">
/// Entity type.
/// </typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="DomainStateException{TEntity}"/> class.
/// </remarks>
/// <param name="value">
/// Property value.
/// </param>
/// <param name="additionalInformation">
/// Additional information to be captured into exception message.
/// </param>
/// <param name="propertyName">
/// Property name.
/// </param>
public sealed class DomainStateException<TEntity>(object? value,
                                                  string? additionalInformation = null,
                                                  [CallerArgumentExpression(nameof(value))] string? propertyName = null)
    : Exception(CreateExceptionMessage(typeof(TEntity), value, propertyName, additionalInformation))
{
    #region Constants
    private const string NullValueRepresentation = "null";
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
    /// <param name="propertyName">
    /// Entity's property name.
    /// </param>
    /// <param name="additionalInformation">
    /// Optional additional information.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> value containing exception
    /// message based on the input arguments.
    /// </returns>
    private static string CreateExceptionMessage(Type entityType, object? value, string? propertyName, string? additionalInformation)
    {
        var stringValue = value is null
            ? NullValueRepresentation
            : value.ToString();

        var messageBuilder = new StringBuilder(GetMessageInvalidDomainState(entityType, propertyName, stringValue));

        if (additionalInformation.IsNotNullOrWhiteSpace())
        {
            var additionalInformationHeader = GetMessageInvalidDomainStateAdditionalInformation();

            messageBuilder.AppendLine(additionalInformationHeader);
            messageBuilder.Append(additionalInformation);
        }

        return messageBuilder.ToString();
    }
    #endregion
}