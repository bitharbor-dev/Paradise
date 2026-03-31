using Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;
using Paradise.Domain.Events.Identity.Users;
using System.Globalization;

namespace Paradise.ApplicationLogic.Tests.Unit.EventListeners.Domain.Identity.Users;

/// <summary>
/// <see cref="SendEmailAddressChangedNotification"/> test class.
/// </summary>
public sealed partial class SendEmailAddressChangedNotificationTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="SendEmailAddressChangedNotification.ProcessAsync"/> method should
    /// send a notification email to both the old and the new email address.
    /// </summary>
    [Fact]
    public async Task ProcessAsync()
    {
        // Arrange
        var oldEmailAddress = "old@email.com";
        var newEmailAddress = "new@email.com";

        var domainEvent = new EmailAddressResetCompletedEvent(
            occurredOn: Test.UtcNow,
            userName: "TestUser",
            oldEmailAddress: oldEmailAddress,
            newEmailAddress: newEmailAddress,
            userCulture: CultureInfo.GetCultureInfo("en-US"));

        // Act
        await Test.Target.ProcessAsync(domainEvent, Token);

        // Assert
        var email = Assert.Single(Test.SentEmails);

        Assert.Contains(oldEmailAddress, email.To);
        Assert.Contains(newEmailAddress, email.To);
    }
    #endregion
}