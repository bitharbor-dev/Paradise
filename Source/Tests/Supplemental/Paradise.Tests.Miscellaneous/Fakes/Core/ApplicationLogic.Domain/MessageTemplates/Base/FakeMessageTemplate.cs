using Paradise.ApplicationLogic.Domain.MessageTemplates.Base;

namespace Paradise.Tests.Miscellaneous.Fakes.Core.ApplicationLogic.Domain.MessageTemplates.Base;

/// <summary>
/// Fake <see cref="MessageTemplate"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeMessageTemplate"/> class.
/// </remarks>
/// <param name="templateName">
/// Template name.
/// </param>
/// <param name="templateText">
/// Template text.
/// </param>
public sealed class FakeMessageTemplate(string templateName, string templateText) : MessageTemplate(templateName, templateText)
{
    #region Public methods
    /// <inheritdoc/>
    public override IEnumerable<object?> GetEqualityComponents()
        => [];
    #endregion
}