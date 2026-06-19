using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="SeedingConfiguration"/> class.
    /// </summary>
    public SeedingConfiguration()
    {
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

    private static IEnumerable<SeedEmailTemplateModel> GetSeedEmailTemplates()
    {
        yield return new("EmailAddressChangedNotification", null, "Email address changed", true, "{parameter}",
                         2, "{subjectParameter}", 0, "Parameter 1: {parameter}0; Parameter 2: {parameter}1");

        yield return new("EmailAddressChangeLink", null, "Email address reset", true, "{parameter}",
                         2, "{subjectParameter}", 0, "Parameter 1: {parameter}0; Parameter 2: {parameter}1");

        yield return new("EmailAddressChangingNotification", null, "Email address is about to be changed", true, "{parameter}",
                         2, "{subjectParameter}", 0, "Parameter 1: {parameter}0; Parameter 2: {parameter}1");

        yield return new("EmailAddressConfirmationLink", null, "Email address confirmation", true, "{parameter}",
                         1, "{subjectParameter}", 0, "Parameter 1: {parameter}0");

        yield return new("PasswordChangedNotification", null, "Password changed", true, "{parameter}",
                         0, "{subjectParameter}", 0, "Password changed");

        yield return new("PasswordChangeLink", null, "Password reset", true, "{parameter}",
                         1, "{subjectParameter}", 0, "Parameter 1: {parameter}0");

        yield return new("TwoFactorVerification", null, "New login confirmation", true, "{parameter}",
                         1, "{subjectParameter}", 0, "Parameter 1: {parameter}0");
    }
    #endregion
}