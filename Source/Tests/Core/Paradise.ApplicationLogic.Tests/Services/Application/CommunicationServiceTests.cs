using Paradise.ApplicationLogic.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Services.Application;
using Paradise.DataAccess.Repositories.Application.Implementation;
using Paradise.DataAccess.Repositories.Base;
using Paradise.Models.Application.CommunicationModels;
using System.Globalization;

namespace Paradise.ApplicationLogic.Tests.Services.Application;

/// <summary>
/// Test class for the <see cref="CommunicationService"/> methods.
/// </summary>
public sealed class CommunicationServiceTests
{
    #region Constants
    /// <summary>
    /// Test email address.
    /// </summary>
    private const string TestEmail = "test@test.com";
    /// <summary>
    /// Invalid test email address.
    /// </summary>
    private const string InvalidEmailAddress = "Invalid email address.";
    #endregion

    #region Fields
    private static readonly string[] _defaultArgs = ["Test parameter"];
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="CommunicationServiceTests"/> class.
    /// </summary>
    public CommunicationServiceTests()
    {
        var applicationDataSource = GetApplicationDataSource();

        var smtpOptions = GetSmtpOptions();

        var smtpClient = GetSmtpClient();

        var repository = new EmailTemplatesRepository(applicationDataSource);

        Source = applicationDataSource;

        Service = new(smtpOptions, smtpClient, repository);

        Service.EmailMessageSent += (s, e) => SentEmailsCache.Add(e);
    }
    #endregion

    #region Properties
    /// <summary>
    /// A <see cref="IDataSource"/> instance used to
    /// arrange data and validate test results.
    /// </summary>
    public IDataSource Source { get; }

    /// <summary>
    /// A <see cref="CommunicationService"/> instance to be tested.
    /// </summary>
    public CommunicationService Service { get; }

    /// <summary>
    /// All sent emails will appear in this list during test methods execution.
    /// </summary>
    private List<EmailMessageSentEventArgs> SentEmailsCache { get; } = [];
    #endregion

    #region Public methods
    /// <summary>
    /// <see cref="CommunicationService.SendEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// successful execution.
    /// <para>
    /// An email message is sent, and in this particular case
    /// it's meta-data is captured in <see cref="SentEmailsCache"/>.
    /// </para>
    /// </para>
    /// </summary>
    [Theory, MemberData(nameof(SendEmailAsync_MemberData))]
    public async void SendEmailAsync(BaseEmailModel baseEmailModel)
    {
        ArgumentNullException.ThrowIfNull(baseEmailModel);

        // Arrange
        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        var request = new EmailSendRequestModel(baseEmailModel,
                                                templateName,
                                                culture,
                                                _defaultArgs,
                                                _defaultArgs);

        await CreateEmailTemplateAsync(templateName: templateName,
                                       culture: culture,
                                       templateText: "Body with {parameter}0",
                                       subject: "Subject with {parameter}0",
                                       placeholdersNumber: 1,
                                       subjectPlaceholdersNumber: 1);

        // Act
        var result = await Service.SendEmailAsync(request);

        // Assert
        result.AssertSuccess(OK);

        var message = SentEmailsCache.First().Message;

        Assert.True(message.To.SequenceEqual(baseEmailModel.To));

        Assert.True((message.Cc is null && baseEmailModel.Cc is null)
            || (message.Cc is not null && baseEmailModel.Cc is not null && message.Cc.SequenceEqual(baseEmailModel.Cc)));

        Assert.True((message.Bcc is null && baseEmailModel.Bcc is null)
            || (message.Bcc is not null && baseEmailModel.Bcc is not null && message.Bcc.SequenceEqual(baseEmailModel.Bcc)));

        Assert.True((message.Attachmetns is null && baseEmailModel.Attachmetns is null)
            || (message.Attachmetns is not null && baseEmailModel.Attachmetns is not null && message.Attachmetns.SequenceEqual(baseEmailModel.Attachmetns)));
    }

    /// <summary>
    /// <see cref="CommunicationService.SendEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the recipients list provided
    /// is empty.
    /// </para>
    /// </summary>
    [Fact]
    public async void SendEmailAsync_ThrowsOnEmptyRecipientsList()
    {
        // Arrange
        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        var request = new EmailSendRequestModel(new([]),
                                                templateName,
                                                culture,
                                                _defaultArgs);

        await CreateEmailTemplateAsync(templateName: templateName,
                                       culture: culture);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.SendEmailAsync(request));

        var result = exception.GetResult();

        // Assert

        result.AssertFail(BadRequest, EmptyRecipientsList);
    }

    /// <summary>
    /// <see cref="CommunicationService.SendEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since one of the recipients email addresses
    /// has an invalid format.
    /// </para>
    /// </summary>
    [Fact]
    public async void SendEmailAsync_ThrowsOnInvalidBlindCopyRecipientEmailFormat()
    {
        // Arrange
        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        await CreateEmailTemplateAsync(templateName: templateName,
                                       culture: culture);

        var request = new EmailSendRequestModel(new([TestEmail]) { Bcc = [InvalidEmailAddress] },
                                                templateName,
                                                culture);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.SendEmailAsync(request));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidEmail);
    }

    /// <summary>
    /// <see cref="CommunicationService.SendEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplate"/> being used
    /// failed to be formatted due to invalid body placeholders number provided.
    /// </para>
    /// </summary>
    [Fact]
    public async void SendEmailAsync_ThrowsOnInvalidBodyPlaceholdersNumber()
    {
        // Arrange
        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        var request = new EmailSendRequestModel(new([TestEmail]),
                                                templateName,
                                                culture,
                                                _defaultArgs);

        await CreateEmailTemplateAsync(templateName: templateName,
                                       culture: culture);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.SendEmailAsync(request));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(ServiceUnavailable, MessageTemplateInvalidPlaceholdersNumber);
    }

    /// <summary>
    /// <see cref="CommunicationService.SendEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since one of the recipients email addresses
    /// has an invalid format.
    /// </para>
    /// </summary>
    [Fact]
    public async void SendEmailAsync_ThrowsOnInvalidCopyRecipientEmailFormat()
    {
        // Arrange
        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        var request = new EmailSendRequestModel(new([TestEmail]) { Cc = [InvalidEmailAddress] },
                                                templateName,
                                                culture);

        await CreateEmailTemplateAsync(templateName: templateName,
                                       culture: culture);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.SendEmailAsync(request));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidEmail);
    }

    /// <summary>
    /// <see cref="CommunicationService.SendEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since one of the recipients email addresses
    /// has an invalid format.
    /// </para>
    /// </summary>
    [Fact]
    public async void SendEmailAsync_ThrowsOnInvalidRecipientEmailFormat()
    {
        // Arrange
        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        var request = new EmailSendRequestModel(new([InvalidEmailAddress]),
                                                templateName,
                                                culture);

        await CreateEmailTemplateAsync(templateName: templateName,
                                       culture: culture);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.SendEmailAsync(request));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(BadRequest, InvalidEmail);
    }

    /// <summary>
    /// <see cref="CommunicationService.SendEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplate"/> being used
    /// failed to be formatted due to invalid subject placeholders number provided.
    /// </para>
    /// </summary>
    [Fact]
    public async void SendEmailAsync_ThrowsOnInvalidSubjectPlaceholdersNumber()
    {
        // Arrange
        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        var request = new EmailSendRequestModel(new([TestEmail]),
                                                templateName,
                                                culture,
                                                null,
                                                _defaultArgs);

        await CreateEmailTemplateAsync(templateName: templateName,
                                       culture: culture);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.SendEmailAsync(request));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(ServiceUnavailable, MessageTemplateInvalidPlaceholdersNumber);
    }

    /// <summary>
    /// <see cref="CommunicationService.SendEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplate"/> being used
    /// has more body placeholders than was provided.
    /// </para>
    /// </summary>
    [Fact]
    public async void SendEmailAsync_ThrowsOnMissingBodyPlaceholder()
    {
        // Arrange
        var templateName = "Test";
        var templateText = "Text without placeholder.";
        var culture = CultureInfo.InvariantCulture;

        var request = new EmailSendRequestModel(new([TestEmail]),
                                                templateName,
                                                culture,
                                                _defaultArgs);

        await CreateEmailTemplateAsync(templateName: templateName,
                                       culture: culture,
                                       templateText: templateText,
                                       placeholdersNumber: 1);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.SendEmailAsync(request));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(ServiceUnavailable, MessageTemplateMissingPlaceholder);
    }

    /// <summary>
    /// <see cref="CommunicationService.SendEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplate"/> being used
    /// has more subject placeholders than was provided.
    /// </para>
    /// </summary>
    [Fact]
    public async void SendEmailAsync_ThrowsOnMissingSubjectPlaceholder()
    {
        // Arrange
        var templateName = "Test";
        var subject = "Text without placeholder.";
        var culture = CultureInfo.InvariantCulture;

        var request = new EmailSendRequestModel(new([TestEmail]),
                                                templateName,
                                                culture,
                                                null,
                                                _defaultArgs);

        await CreateEmailTemplateAsync(templateName: templateName,
                                       culture: culture,
                                       subject: subject,
                                       subjectPlaceholdersNumber: 1);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.SendEmailAsync(request));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(ServiceUnavailable, MessageTemplateMissingPlaceholder);
    }

    /// <summary>
    /// <see cref="CommunicationService.SendEmailAsync"/> test method.
    /// <para>
    /// <strong>Expected result:</strong>
    /// throws a <see cref="ResultException"/> since the <see cref="EmailTemplate"/> required
    /// to send a message does not exist.
    /// </para>
    /// </summary>
    [Fact]
    public async void SendEmailAsync_ThrowsOnNonExistingEmailTemplate()
    {
        // Arrange
        var request = new EmailSendRequestModel(new([TestEmail]),
                                                string.Empty,
                                                null,
                                                _defaultArgs);

        // Act
        var exception = await Assert.ThrowsAsync<ResultException>(()
            => Service.SendEmailAsync(request));

        var result = exception.GetResult();

        // Assert
        result.AssertFail(NotFound, MessageTemplateNotFound);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates an <see cref="EmailTemplate"/> instance
    /// and saves its data into the test persistence storage.
    /// </summary>
    /// <param name="templateName">
    /// Template name.
    /// </param>
    /// <param name="culture">
    /// Template culture.
    /// </param>
    /// <param name="templateText">
    /// Template text.
    /// </param>
    /// <param name="subject">
    /// Email subject.
    /// </param>
    /// <param name="placeholderName">
    /// Placeholder name to be replaced with values during a message formatting.
    /// </param>
    /// <param name="placeholdersNumber">
    /// The number of placeholders to be replaced with values during the message formatting.
    /// </param>
    /// <param name="subjectPlaceholderName">
    /// Subject placeholder name to be replaced with values during a message formatting.
    /// </param>
    /// <param name="subjectPlaceholdersNumber">
    /// The number of subject placeholders to be replaced with values during the message formatting.
    /// </param>
    private async Task CreateEmailTemplateAsync(string templateName = "Test",
                                                CultureInfo? culture = null,
                                                string templateText = "Test",
                                                string subject = "Test",
                                                string? placeholderName = "{parameter}",
                                                ushort placeholdersNumber = 0,
                                                string? subjectPlaceholderName = "{parameter}",
                                                ushort subjectPlaceholdersNumber = 0)
    {
        var template = new EmailTemplate(templateName, templateText, subject)
        {
            Culture = culture,
            PlaceholderName = placeholderName,
            PlaceholdersNumber = placeholdersNumber,
            SubjectPlaceholderName = subjectPlaceholderName,
            SubjectPlaceholdersNumber = subjectPlaceholdersNumber
        };

        Source.Add(template);
        await Source.SaveChangesAsync();
    }
    #endregion

    #region Data generation
    /// <summary>
    /// Provides member data for <see cref="SendEmailAsync"/> method.
    /// </summary>
    public static TheoryData<BaseEmailModel> SendEmailAsync_MemberData => new()
    {
        { new BaseEmailModel([TestEmail]) },
        { new BaseEmailModel([TestEmail]) { Attachmetns = [new([], "Test", "text/plain")] } },
        { new BaseEmailModel([TestEmail]) { Cc = [TestEmail] } },
        { new BaseEmailModel([TestEmail]) { Bcc = [TestEmail] } }
    };
    #endregion
}