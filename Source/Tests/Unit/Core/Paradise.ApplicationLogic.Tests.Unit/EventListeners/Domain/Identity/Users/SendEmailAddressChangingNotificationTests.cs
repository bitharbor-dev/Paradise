using Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;
using Paradise.Domain.Events.Identity.Users;
using System.Globalization;

namespace Paradise.ApplicationLogic.Tests.Unit.EventListeners.Domain.Identity.Users;

/// <summary>
/// <see cref="SendEmailAddressChangingNotification"/> test class.
/// </summary>
public sealed partial class SendEmailAddressChangingNotificationTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="SendEmailAddressChangingNotification.ProcessAsync"/> method should
    /// send a notification email to the old email address.
    /// </summary>
    [Fact]
    public async Task ProcessAsync()
    {
        // Arrange
        var currentEmailAddress = "old@email.com";
        var newEmailAddress = "new@email.com";

        var changeEmailAddressToken = "changeEmailAddressToken";

        var domainEvent = new EmailAddressResetRequestedEvent(
            occurredOn: Test.UtcNow,
            userName: "TestUser",
            changeEmailAddressToken: changeEmailAddressToken,
            currentEmailAddress: currentEmailAddress,
            newEmailAddress: newEmailAddress,
            userCulture: CultureInfo.GetCultureInfo("en-US"));

        // Act
        await Test.Target.ProcessAsync(domainEvent, Token);

        // Assert
        var email = Assert.Single(Test.SentEmails);

        Assert.Contains(currentEmailAddress, email.To);
    }

    /// <summary>
    /// The <see cref="SendEmailAddressChangingNotification.ProcessingOrder"/> property should
    /// have fixed value of '0'.
    /// </summary>
    /// <remarks>
    /// This value is business-critical.
    /// <para/>
    /// Domain event listeners are executed in order of their processing order.
    /// Changing this value may alter the sequence in which side effects are
    /// observed externally, potentially breaking business workflows that rely
    /// on a specific execution order.
    /// <para/>
    /// This test exists to prevent accidental reordering during refactoring.
    /// </remarks>
    [Fact]
    public void ProcessingOrder()
    {
        // Arrange
        var expectedProcessingOrder = 0;

        // Act
        var result = Test.Target.ProcessingOrder;

        // Assert
        Assert.Equal(expectedProcessingOrder, result);
    }
    #endregion
}