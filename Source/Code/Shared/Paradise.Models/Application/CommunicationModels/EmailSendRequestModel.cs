using System.Globalization;

namespace Paradise.Models.Application.CommunicationModels;

/// <summary>
/// Represents an email send request.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailSendRequestModel"/> class.
/// </remarks>
/// <param name="basicData">
/// Basic email information.
/// </param>
/// <param name="templateName">
/// Email template name.
/// </param>
/// <param name="culture">
/// Email template culture.
/// <para>
/// Pass <see langword="null"/> to use invariant culture.
/// </para>
/// </param>
/// <param name="bodyArgs">
/// An object array that contains zero or more objects to format the email template body.
/// </param>
/// <param name="subjectArgs">
/// An object array that contains zero or more objects to format the email template subject.
/// </param>
/// <param name="useNullOrInvariantCultureAsFallback">
/// Indicates whether the <see cref="CultureInfo.InvariantCulture"/>
/// or <see langword="null"/> culture should be used
/// in case the template with the specified <see cref="Culture"/>
/// does not exist.
/// </param>
public sealed class EmailSendRequestModel(BaseEmailModel basicData, string templateName, CultureInfo? culture,
                                          object?[]? bodyArgs = null, object?[]? subjectArgs = null,
                                          bool useNullOrInvariantCultureAsFallback = true)
{
    #region Properties
    /// <summary>
    /// Basic email information.
    /// </summary>
    public BaseEmailModel BasicData { get; } = basicData;

    /// <summary>
    /// Email template name.
    /// </summary>
    public string TemplateName { get; } = templateName;

    /// <summary>
    /// Email template culture.
    /// <para>
    /// Pass <see langword="null"/> to use invariant culture.
    /// </para>
    /// </summary>
    public CultureInfo? Culture { get; } = culture;

    /// <summary>
    /// An object array that contains zero or more objects to format the email template body.
    /// </summary>
    public IList<object?>? BodyArgs { get; } = bodyArgs;

    /// <summary>
    /// An object array that contains zero or more objects to format the email template subject.
    /// </summary>
    public IList<object?>? SubjectArgs { get; } = subjectArgs;

    /// <summary>
    /// Indicates whether the <see cref="CultureInfo.InvariantCulture"/>
    /// or <see langword="null"/> culture should be used
    /// in case the template with the specified <see cref="Culture"/>
    /// does not exist.
    /// </summary>
    public bool UseNullOrInvariantCultureAsFallback { get; } = useNullOrInvariantCultureAsFallback;
    #endregion
}