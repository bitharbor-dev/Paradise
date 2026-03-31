using Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Models;
using System.Globalization;

namespace Paradise.ApplicationLogic.Tests.Unit.EventListeners.Domain.Identity.Users;

/// <summary>
/// <see cref="SendEmailAddressChangeLink"/> test class.
/// </summary>
public sealed partial class SendEmailAddressChangeLinkTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="SendEmailAddressChangeLink.ProcessAsync"/> method should
    /// send an email containing the link to change the email address.
    /// </summary>
    [Fact]
    public async Task ProcessAsync()
    {
        // Arrange
        var currentEmailAddress = "old@email.com";
        var newEmailAddress = "new@email.com";

        var changeEmailAddressToken = "changeEmailAddressToken";
        var timeout = Test.ApplicationOptions.Timeout.ResetEmailAddressTimeout;
        var expiryDate = Test.UtcNow.Add(timeout);

        var identityTokenModel = new IdentityToken(currentEmailAddress, changeEmailAddressToken, newEmailAddress, expiryDate);
        var identityToken = Test.Protector.Protect(identityTokenModel);

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

        Assert.Contains(newEmailAddress, email.To);
        Assert.Contains(identityToken, email.Body, StringComparison.Ordinal);
    }

    /// <summary>
    /// The <see cref="SendEmailAddressChangeLink.ProcessingOrder"/> property should
    /// have fixed value of '1'.
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
        var expectedProcessingOrder = 1;

        // Act
        var result = Test.Target.ProcessingOrder;

        // Assert
        Assert.Equal(expectedProcessingOrder, result);
    }
    #endregion
}