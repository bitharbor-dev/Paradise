using Paradise.ApplicationLogic.Infrastructure.Communication.Implementation;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Communication.Email;
using Paradise.Localization.ExceptionHandling;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;
using System.Globalization;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Communication.Implementation;

/// <summary>
/// <see cref="CommunicationClient"/> test class.
/// </summary>
public sealed partial class CommunicationClientTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="SendEmailAsync"/> method.
    /// </summary>
    public static TheoryData<BaseEmailModel> SendEmailAsync_MemberData { get; } = new()
    {
        { new BaseEmailModel([TestEmail])                                                       },
        { new BaseEmailModel([TestEmail]) { Attachments = [new(null!, "Test", "text/plain")] }  },
        { new BaseEmailModel([TestEmail]) { Cc = [TestEmail] }                                  },
        { new BaseEmailModel([TestEmail]) { Bcc = [TestEmail] }                                 }
    };

    /// <summary>
    /// Provides member data for <see cref="SendEmailAsync_ThrowsOnInvalidEmailAddressFormat"/> method.
    /// </summary>
    public static TheoryData<BaseEmailModel> SendEmailAsync_ThrowsOnInvalidEmailAddressFormat_MemberData { get; } = new()
    {
        { new BaseEmailModel([InvalidEmailAddress])                         },
        { new BaseEmailModel([TestEmail]) { Cc = [InvalidEmailAddress] }    },
        { new BaseEmailModel([TestEmail]) { Bcc = [InvalidEmailAddress] }   }
    };

    /// <summary>
    /// Provides member data for <see cref="Constructor_ThrowsOnInvalidUsername"/> method.
    /// </summary>
    public static TheoryData<string?> Constructor_ThrowsOnInvalidUsername_MemberData { get; } = new()
    {
        { null as string    },
        { string.Empty      },
        { " "               }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="CommunicationClient"/> constructor should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// <see cref="SmtpOptions"/> instance has
    /// <see langword="null"/> <see cref="SmtpOptions.Credentials"/>.
    /// </summary>
    [Fact]
    public void Constructor_ThrowsOnNullCredentials()
    {
        // Arrange
        Test.Options.Credentials = null;

        // Act
        var exception = Assert.Throws<InvalidOperationException>(Test.CreateClient);

        // Assert
        Assert.Equal(ExceptionMessages.GetMessageInvalidSmtpConfiguration(), exception.Message);
    }

    /// <summary>
    /// The <see cref="CommunicationClient"/> constructor should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// <see cref="SmtpOptions"/> instance
    /// has <see langword="null"/>, <see cref="string.Empty"/> or whitespace-only
    /// username value in <see cref="SmtpOptions.Credentials"/>.
    /// </summary>
    /// <param name="userName">
    /// The user name associated with the credentials.
    /// </param>
    [Theory, MemberData(nameof(Constructor_ThrowsOnInvalidUsername_MemberData))]
    public void Constructor_ThrowsOnInvalidUsername(string? userName)
    {
        // Arrange
        Test.Options.Credentials!.UserName = userName;

        // Act
        var exception = Assert.Throws<InvalidOperationException>(Test.CreateClient);

        // Assert
        Assert.Equal(ExceptionMessages.GetMessageInvalidSmtpConfiguration(), exception.Message);
    }

    /// <summary>
    /// The <see cref="CommunicationClient.SendEmailAsync"/> method should
    /// send an email message.
    /// </summary>
    /// <param name="model">
    /// Basic email information.
    /// </param>
    [Theory, MemberData(nameof(SendEmailAsync_MemberData))]
    public async Task SendEmailAsync(BaseEmailModel model)
    {
        // Arrange
        var client = Test.CreateClient();

        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        Test.AddEmailTemplate(templateName: templateName,
                              culture: culture,
                              templateText: "Body with {parameter}0",
                              subject: "Subject with {parameter}0",
                              placeholderName: "{parameter}",
                              placeholdersNumber: 1,
                              subjectPlaceholderName: "{parameter}",
                              subjectPlaceholdersNumber: 1);

        var request = new EmailSendRequestModel(model,
                                                templateName,
                                                culture,
                                                DefaultArgs,
                                                DefaultArgs);

        // Act
        var result = await client.SendEmailAsync(request, Token);

        // Assert
        Assert.Equivalent(Test.SentEmails.First(), result, true);
        Assert.Equal(Test.UtcNow, result.Sent);
    }

    /// <summary>
    /// The <see cref="CommunicationClient.SendEmailAsync"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// recipients list is empty.
    /// </summary>
    [Fact]
    public async Task SendEmailAsync_ThrowsOnEmptyRecipientsList()
    {
        // Arrange
        var client = Test.CreateClient();

        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        var request = new EmailSendRequestModel(new([]),
                                                templateName,
                                                culture,
                                                DefaultArgs);

        Test.AddEmailTemplate(templateName: templateName,
                              culture: culture);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => client.SendEmailAsync(request, Token));
    }

    /// <summary>
    /// The <see cref="CommunicationClient.SendEmailAsync"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// recipients' email address has an invalid format.
    /// </summary>
    /// <param name="model">
    /// Basic email information.
    /// </param>
    [Theory, MemberData(nameof(SendEmailAsync_ThrowsOnInvalidEmailAddressFormat_MemberData))]
    public async Task SendEmailAsync_ThrowsOnInvalidEmailAddressFormat(BaseEmailModel model)
    {
        // Arrange
        var client = Test.CreateClient();

        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        var request = new EmailSendRequestModel(model,
                                                templateName,
                                                culture,
                                                DefaultArgs);

        Test.AddEmailTemplate(templateName: templateName,
                              culture: culture);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => client.SendEmailAsync(request, Token));
    }

    /// <summary>
    /// The <see cref="CommunicationClient.SendEmailAsync"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the
    /// <see cref="EmailTemplate"/> being used
    /// failed to be formatted due to invalid body placeholders number provided.
    /// </summary>
    [Fact]
    public async Task SendEmailAsync_ThrowsOnInvalidBodyPlaceholdersNumber()
    {
        // Arrange
        var client = Test.CreateClient();

        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        Test.AddEmailTemplate(templateName: templateName,
                              culture: culture);

        var request = new EmailSendRequestModel(new([TestEmail]),
                                                templateName,
                                                culture,
                                                DefaultArgs);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => client.SendEmailAsync(request, Token));
    }

    /// <summary>
    /// The <see cref="CommunicationClient.SendEmailAsync"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the
    /// <see cref="EmailTemplate"/> being used
    /// failed to be formatted due to invalid subject placeholders number provided.
    /// </summary>
    [Fact]
    public async Task SendEmailAsync_ThrowsOnInvalidSubjectPlaceholdersNumber()
    {
        // Arrange
        var client = Test.CreateClient();

        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        Test.AddEmailTemplate(templateName: templateName,
                              culture: culture);

        var request = new EmailSendRequestModel(new([TestEmail]),
                                                templateName,
                                                culture,
                                                null,
                                                DefaultArgs);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => client.SendEmailAsync(request, Token));
    }

    /// <summary>
    /// The <see cref="CommunicationClient.SendEmailAsync"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the
    /// <see cref="EmailTemplate"/> being used
    /// has more body placeholders than was provided.
    /// </summary>
    [Fact]
    public async Task SendEmailAsync_ThrowsOnMissingBodyPlaceholder()
    {
        // Arrange
        var client = Test.CreateClient();

        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        Test.AddEmailTemplate(templateName: templateName,
                              culture: culture,
                              templateText: "Body with {parameter}0.",
                              placeholderName: "{parameter}",
                              placeholdersNumber: 1);

        var request = new EmailSendRequestModel(new([TestEmail]),
                                                templateName,
                                                culture,
                                                null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => client.SendEmailAsync(request, Token));
    }

    /// <summary>
    /// The <see cref="CommunicationClient.SendEmailAsync"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the
    /// <see cref="EmailTemplate"/> being used
    /// has more subject placeholders than was provided.
    /// </summary>
    [Fact]
    public async Task SendEmailAsync_ThrowsOnMissingSubjectPlaceholder()
    {
        // Arrange
        var client = Test.CreateClient();

        var templateName = "Test";
        var culture = CultureInfo.InvariantCulture;

        Test.AddEmailTemplate(templateName: templateName,
                              culture: culture,
                              subject: "Subject with {parameter}0.",
                              subjectPlaceholderName: "{parameter}",
                              subjectPlaceholdersNumber: 1);

        var request = new EmailSendRequestModel(new([TestEmail]),
                                                templateName,
                                                culture,
                                                null,
                                                null);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => client.SendEmailAsync(request, Token));
    }

    /// <summary>
    /// The <see cref="CommunicationClient.SendEmailAsync"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the
    /// <see cref="EmailTemplate"/> required
    /// to send a message does not exist.
    /// </summary>
    [Fact]
    public async Task SendEmailAsync_ThrowsOnNonExistingEmailTemplate()
    {
        // Arrange
        var client = Test.CreateClient();

        var request = new EmailSendRequestModel(new([TestEmail]),
                                                string.Empty,
                                                null,
                                                DefaultArgs);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => client.SendEmailAsync(request, Token));
    }
    #endregion
}