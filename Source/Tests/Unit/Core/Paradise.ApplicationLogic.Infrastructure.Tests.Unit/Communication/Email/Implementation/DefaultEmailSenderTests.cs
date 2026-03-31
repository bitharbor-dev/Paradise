using Paradise.ApplicationLogic.Infrastructure.Communication.Email.Implementation;
using Paradise.Models.ApplicationLogic.Infrastructure.Communication.Email;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Communication.Email.Implementation;

/// <summary>
/// <see cref="DefaultEmailSender"/> test class.
/// </summary>
public sealed partial class DefaultEmailSenderTests
{
    #region Properties
    /// <summary>
    /// Provides member data for <see cref="SendAsync"/> method.
    /// </summary>
    public static TheoryData<bool, string[]?, string[]?> SendAsync_MemberData { get; } = new()
    {
        { true,     null,           null        },
        { true,     [],             null        },
        { true,     [TestEmail],    null        },
        { true,     null,           []          },
        { true,     null,           [TestEmail] },
        { false,    null,           null        },
        { false,    [],             null        },
        { false,    [TestEmail],    null        },
        { false,    null,           []          },
        { false,    null,           [TestEmail] }
    };

    /// <summary>
    /// Provides member data for <see cref="SendAsync_ThrowsOnInvalidFromAddress"/> method.
    /// </summary>
    public static TheoryData<string?> SendAsync_ThrowsOnInvalidFromAddress_MemberData { get; } = new()
    {
        { null as string        },
        { string.Empty          },
        { InvalidEmailAddress   }
    };
    #endregion

    #region Public methods
    /// <summary>
    /// The <see cref="DefaultEmailSender.SendAsync"/> method should
    /// send an email message.
    /// </summary>
    /// <param name="isBodyHtml">
    /// Indicates whether the email body is an HTML document.
    /// </param>
    /// <param name="cc">
    /// Copy recipients' email addresses.
    /// </param>
    /// <param name="bcc">
    /// Blind copy recipients' email addresses.
    /// </param>
    [Theory, MemberData(nameof(SendAsync_MemberData))]
    public async Task SendAsync(bool isBodyHtml, string[]? cc, string[]? bcc)
    {
        // Arrange
        using var attachmentStream = new MemoryStream([1]);
        var model = new EmailModel("Subject", "Body", TestEmail, [TestEmail])
        {
            Attachments = [new(attachmentStream, "test", "text/plain")],
            Bcc = bcc,
            Cc = cc,
            IsBodyHtml = isBodyHtml
        };

        // Act
        await Test.Target.SendAsync(model, Token);

        // Assert
        var message = Assert.Single(Test.SentEmails);

        var modelAttachment = Assert.Single(model.Attachments);

        Assert.NotNull(message.Attachments);
        var messageAttachment = Assert.Single(message.Attachments);

        Assert.Equivalent(modelAttachment, messageAttachment);

        Assert.Equivalent(model.Bcc ?? [], message.Bcc ?? []);
        Assert.Equivalent(model.Cc ?? [], message.Cc ?? []);
        Assert.Equivalent(model.To, message.To ?? []);

        Assert.Equal(model.From, message.From);
        Assert.Equal(model.Subject, message.Subject);
        Assert.Equal(model.Body, message.Body);
        Assert.Equal(model.IsBodyHtml, message.IsBodyHtml);
    }

    /// <summary>
    /// The <see cref="DefaultEmailSender.SendAsync"/> method should
    /// throw the <see cref="ArgumentNullException"/> if the input
    /// <see cref="EmailModel"/> is equal to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task SendAsync_ThrowsOnNull()
    {
        // Arrange
        var model = null as EmailModel;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(()
            => Test.Target.SendAsync(model!, Token));
    }

    /// <summary>
    /// The <see cref="DefaultEmailSender.SendAsync"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// sender email address has an invalid format.
    /// </summary>
    /// <param name="from">
    /// Sender email address.
    /// </param>
    [Theory, MemberData(nameof(SendAsync_ThrowsOnInvalidFromAddress_MemberData))]
    public async Task SendAsync_ThrowsOnInvalidFromAddress(string? from)
    {
        // Arrange
        var model = new EmailModel("Subject", "Body", from!, [TestEmail]);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Test.Target.SendAsync(model, Token));
    }

    /// <summary>
    /// The <see cref="DefaultEmailSender.SendAsync"/> method should
    /// throw the <see cref="InvalidOperationException"/> if the input
    /// recipients list is empty.
    /// </summary>
    [Fact]
    public async Task SendAsync_ThrowsOnEmptyRecipientsList()
    {
        // Arrange
        var model = new EmailModel("Subject", "Body", TestEmail, []);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(()
            => Test.Target.SendAsync(model, Token));
    }
    #endregion
}