using Paradise.Common.Extensions;
using System.Runtime.CompilerServices;
using System.Text;
using static Paradise.Localization.ExceptionHandling.ExceptionMessages;

namespace Paradise.Domain.Base;

/// <summary>
/// Represents a domain state error message formatting contract.
/// </summary>
/// <param name="value">
/// Property value.
/// </param>
/// <param name="additionalInformation">
/// Additional information to be captured into error message.
/// </param>
/// <param name="propertyName">
/// Property name.
/// </param>
public sealed class DomainStateError<TEntity>(object? value, string? additionalInformation = null,
                                              [CallerArgumentExpression(nameof(value))] string? propertyName = null)
{
    #region Constants
    private const string NullValueRepresentation = "null";
    #endregion

    #region Properties
    /// <summary>
    /// Formatted error message.
    /// </summary>
    public string Message { get; } = CreateMessage(typeof(TEntity), value, propertyName, additionalInformation);
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public override string ToString()
        => Message;
    #endregion

    #region Operators
    /// <summary>
    /// Implicitly converts the given <paramref name="message"/>
    /// into a <see cref="string"/> by calling <see cref="ToString"/> method.
    /// </summary>
    /// <param name="message">
    /// The <see cref="DomainStateError{TEntity}"/> to be converted into a <see cref="string"/>.
    /// </param>
    public static implicit operator string?(DomainStateError<TEntity> message)
        => message?.ToString();
    #endregion

    #region Private methods
    /// <summary>
    /// Creates error message based on the input arguments.
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
    /// A <see cref="string"/> value containing error
    /// message based on the input arguments.
    /// </returns>
    private static string CreateMessage(Type entityType, object? value, string? propertyName, string? additionalInformation)
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