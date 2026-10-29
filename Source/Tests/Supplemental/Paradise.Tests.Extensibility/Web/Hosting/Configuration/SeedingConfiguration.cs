using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Options.Models.Infrastructure.Services.MessageTemplates;
using Paradise.DataAccess.Seed.Models.ApplicationLogic;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Seed.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Seed.Models.Domain;
using Paradise.DataAccess.Seed.Providers;
using Paradise.Primitives;
using Paradise.Tests.Doubles.Stubs.Core.DataAccess.Seed.Providers;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

namespace Paradise.Tests.Extensibility.Web.Hosting.Configuration;

/// <summary>
/// Replaces the default seeding mechanism with the in-memory alternative.
/// </summary>
public sealed class SeedingConfiguration : IWebApplicationServicesConfiguration
{
    #region Fields
    private readonly EmailTemplateOptions _emailTemplateOptions;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="SeedingConfiguration"/> class.
    /// </summary>
    /// <param name="emailTemplateOptions">
    /// The accessor used to access the <see cref="EmailTemplateOptions"/>.
    /// </param>
    public SeedingConfiguration(IOptions<EmailTemplateOptions> emailTemplateOptions)
    {
        ArgumentNullException.ThrowIfNull(emailTemplateOptions);

        _emailTemplateOptions = emailTemplateOptions.Value;

        DomainData = new();

        var emailTemplates = GetSeedEmailTemplates();
        var roles = GetSeedRoles();
        var users = GetSeedUsers();

        InfrastructureData = new(emailTemplates, roles, users);
    }
    #endregion

    #region Properties
    /// <summary>
    /// Contains domain seed data.
    /// </summary>
    public DomainDataSeedModel DomainData { get; set; }

    /// <summary>
    /// Contains infrastructure seed data.
    /// </summary>
    public InfrastructureDataSeedModel InfrastructureData { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services.RemoveAll<ISeedDataProvider>()
            .AddSingleton<ISeedDataProvider>(new StubSeedDataProvider(DomainData, InfrastructureData));
    }
    #endregion

    #region Private methods
    private static IEnumerable<SeedUserModel> GetSeedUsers()
    {
        yield return new("test@paradise.com", "tester", "Password123!", true, [RoleNames.User, RoleNames.Administrator]);
    }

    private static IEnumerable<SeedRoleModel> GetSeedRoles()
    {
        yield return new(RoleNames.User, true);
        yield return new(RoleNames.Administrator, false);
    }

    private IEnumerable<SeedEmailTemplateModel> GetSeedEmailTemplates()
    {
        yield return new(_emailTemplateOptions.EmailAddressChangedNotificationTemplateName, null, "Email address changed",
                         true, "{parameter}", 2, "{subjectParameter}", 0, "Parameter 1: {parameter}0; Parameter 2: {parameter}1");

        yield return new(_emailTemplateOptions.EmailAddressChangeLinkTemplateName, null, "Email address reset",
                         true, "{parameter}", 2, "{subjectParameter}", 0, "Parameter 1: {parameter}0; Parameter 2: {parameter}1");

        yield return new(_emailTemplateOptions.EmailAddressChangingNotificationTemplateName, null, "Email address is about to be changed",
                         true, "{parameter}", 2, "{subjectParameter}", 0, "Parameter 1: {parameter}0; Parameter 2: {parameter}1");

        yield return new(_emailTemplateOptions.EmailAddressConfirmationLinkTemplateName, null, "Email address confirmation",
                         true, "{parameter}", 1, "{subjectParameter}", 0, "Parameter 1: {parameter}0");

        yield return new(_emailTemplateOptions.PasswordChangedNotificationTemplateName, null, "Password changed",
                         true, "{parameter}", 0, "{subjectParameter}", 0, "Password changed");

        yield return new(_emailTemplateOptions.PasswordChangeLinkTemplateName, null, "Password reset",
                         true, "{parameter}", 1, "{subjectParameter}", 0, "Parameter 1: {parameter}0");

        yield return new(_emailTemplateOptions.TwoFactorVerificationTemplateName, null, "New login confirmation",
                         true, "{parameter}", 1, "{subjectParameter}", 0, "Parameter 1: {parameter}0");
    }
    #endregion
}