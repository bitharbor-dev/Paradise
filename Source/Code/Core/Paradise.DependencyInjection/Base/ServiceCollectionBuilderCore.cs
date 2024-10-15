using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Identity;
using Paradise.ApplicationLogic.Services.Application;
using Paradise.ApplicationLogic.Services.Application.Implementation;
using Paradise.Common.Extensions;
using Paradise.DataAccess.Database;
using Paradise.DataAccess.Database.Interceptors;
using Paradise.DataAccess.Database.Interceptors.Base;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.Application;
using Paradise.DataAccess.Repositories.Application.Implementation;
using Paradise.DataAccess.Repositories.Domain;
using Paradise.DataAccess.Repositories.Domain.Implementation;
using Paradise.DataAccess.Seed.Providers;
using Paradise.DataAccess.Seed.Providers.Implementation;
using Paradise.Domain.Roles;
using Paradise.Domain.Users;
using Paradise.Options.Models;
using Paradise.Options.Origins.Base;
using System.Text.Json;

namespace Paradise.DependencyInjection.Base;

/// <summary>
/// Contains common application services configuration.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ServiceCollectionBuilderCore"/> class.
/// </remarks>
/// <param name="services">
/// The service collection the builder will operate over.
/// </param>
/// <param name="configurationOrigin">
/// Configuration origin.
/// </param>
public abstract class ServiceCollectionBuilderCore(IServiceCollection services, IConfigurationOrigin configurationOrigin) : ServiceCollectionBuilderBase(services, configurationOrigin)
{
    #region Constants
    /// <summary>
    /// The application name.
    /// </summary>
    private const string ApplicationName = "Paradise";
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override void AddMiscellaneous()
    {
        base.AddMiscellaneous();

        AddOptions<ApplicationOptions>(
            postConfigure: null,
            validateOnStartup: true,
            validateDataAnnotations: true);

        AddOptions<JsonSerializerOptions>(
            postConfigure: null,
            validateOnStartup: true,
            validateDataAnnotations: true);

        AddIdentity();

        Services.AddScoped<ISeedDataProvider, JsonSeedDataProvider>();

        Services.AddDataProtection()
                .SetApplicationName(ApplicationName)
                .PersistKeysToDbContext<ApplicationContext>();

        Services.AddLogging();
    }

    /// <inheritdoc/>
    protected override void AddRepositories()
    {
        base.AddRepositories();

        AddDbContexts();

        Services.AddScoped<IEmailTemplatesRepository, EmailTemplatesRepository>();
        Services.AddScoped<IUserRefreshTokensRepository, UserRefreshTokensRepository>();
    }

    /// <inheritdoc/>
    protected override void AddServices()
    {
        base.AddServices();

        Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        Services.AddScoped<IDatabaseService, DatabaseService>();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Adds the identity services.
    /// </summary>
    private void AddIdentity()
    {
        AddOptions<IdentityOptions>(
            postConfigure: null,
            validateOnStartup: true,
            validateDataAnnotations: true);

        void SetUpIdentity(IdentityOptions options)
            => Configuration.BindSection(options);

        Services.AddIdentity<User, Role>(SetUpIdentity)
            .AddEntityFrameworkStores<DomainContext>()
            .AddRoleManager<RoleManager<Role>>()
            .AddUserManager<UserManager>()
            .AddDefaultTokenProviders();
    }

    /// <summary>
    /// Adds the database contexts.
    /// </summary>
    private void AddDbContexts()
    {
        Services.AddSingleton<IInterceptor, ValidateStateInterceptor>();
        Services.AddSingleton<IInterceptor, SetCreatedInterceptor>();
        Services.AddSingleton<IInterceptor, SetModifiedInterceptor>();

        AddDbContext<IApplicationDataSource, ApplicationContext>(ApplicationContext.ConnectionStringName,
                                                                 ApplicationContext.SchemeName);

        AddDbContext<IDomainDataSource, DomainContext>(DomainContext.ConnectionStringName,
                                                       DomainContext.SchemeName);
    }

    /// <summary>
    /// Adds the database context.
    /// </summary>
    /// <typeparam name="TContextService">
    /// Database context service type.
    /// </typeparam>
    /// <typeparam name="TContextImplementation">
    /// Database context implementation type.
    /// </typeparam>
    /// <param name="connectionStringName">
    /// The name of the configuration value,
    /// which contains the connection string.
    /// </param>
    /// /// <param name="schemeName">
    /// Database scheme name.
    /// </param>
    private void AddDbContext<TContextService, TContextImplementation>(string connectionStringName, string schemeName)
        where TContextImplementation : DbContext, TContextService
    {
        void ConfigureDbContextOptions(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
        {
            var connectionString = Configuration.GetConnectionString(connectionStringName);
            var interceptors = serviceProvider.GetService<IEnumerable<IInterceptor>>();

            builder.UseSqlServer(connectionString, options =>
            {
                options.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schemeName);
                options.EnableRetryOnFailure();
            });

            if (interceptors is not null)
            {
                var interceptorsToAdd = interceptors
                    .Where(interceptor => interceptor is not IDbContextSpecificInterceptor specificInterceptor
                                       || specificInterceptor.DbContextType == typeof(TContextService)
                                       || specificInterceptor.DbContextType == typeof(TContextImplementation));

                builder.AddInterceptors(interceptorsToAdd);
            }
        }

        Services.AddDbContext<TContextService, TContextImplementation>(ConfigureDbContextOptions);
    }
    #endregion
}