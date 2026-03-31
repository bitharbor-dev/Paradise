using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates.Base;
using System.Globalization;

namespace Paradise.Tests.Miscellaneous.TestImplementations.Core.ApplicationLogic.Infrastructure.Domain.MessageTemplates.Base;

/// <summary>
/// Fake <see cref="TestMessageTemplate"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestMessageTemplate"/> class.
/// </remarks>
/// <param name="templateName">
/// Template name.
/// </param>
/// <param name="culture">
/// Template culture.
/// </param>
/// <param name="templateText">
/// Template text.
/// </param>
public sealed class TestMessageTemplate(string templateName, CultureInfo? culture, string templateText)
    : MessageTemplate(templateName, culture, templateText);