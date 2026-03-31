using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Paradise.DataAccess.Database;
using Paradise.DataAccess.Database.Interceptors;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates.Implementation;
using Paradise.DataAccess.Repositories.Attributes;
using Paradise.DataAccess.Repositories.Domain.Identity.Users;
using Paradise.DataAccess.Repositories.Domain.Identity.Users.Implementation;
using Paradise.DataAccess.Repositories.Implementation;

namespace Paradise.DataAccess.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="IServiceCollection"/> <see langword="interface"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    #region Public methods
    /// <summary>
    /// Registers the data access services, such as repositories, interceptors and database context.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance containing the connection strings.
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(TimeProvider.System);

        services
            .AddSingleton<IInterceptor, OnCreatedInterceptor>()
            .AddSingleton<IInterceptor, OnModifiedInterceptor>();

        services.AddKeyedDbContext<IDataSource, InfrastructureContext>(configuration,
                                                                       InfrastructureContext.ConnectionStringName,
                                                                       InfrastructureContext.SchemeName,
                                                                       InfrastructureContextKeyAttribute.ContextKey);

        services
            .AddScoped<IInfrastructureUnitOfWork, InfrastructureUnitOfWork>()
            .AddScoped<IEmailTemplatesRepository, EmailTemplatesRepository>();

        services.AddKeyedDbContext<IDataSource, DomainContext>(configuration,
                                                               DomainContext.ConnectionStringName,
                                                               DomainContext.SchemeName,
                                                               DomainContextKeyAttribute.ContextKey);

        services
            .AddScoped<IDomainUnitOfWork, DomainUnitOfWork>()
            .AddScoped<IUserRefreshTokensRepository, UserRefreshTokensRepository>();

        return services;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Registers a database context and its associated service type as a keyed service,
    /// using a specified <paramref name="connectionStringName"/> and <paramref name="schema"/>.
    /// </summary>
    /// <typeparam name="TContextService">
    /// The service abstraction type for the database context.
    /// </typeparam>
    /// <typeparam name="TContextImplementation">
    /// The concrete implementation of the database context.
    /// </typeparam>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configuration">
    /// The <see cref="IConfiguration"/> instance containing the connection strings.
    /// </param>
    /// <param name="connectionStringName">
    /// The name of the connection string in the configuration.
    /// </param>
    /// <param name="schema">
    /// A <see langword="string"/> value representing the database schema name.
    /// </param>
    /// <param name="key">
    /// The <see cref="ServiceDescriptor.ServiceKey"/> of the service.
    /// </param>
    private static void AddKeyedDbContext<TContextService, TContextImplementation>(this IServiceCollection services,
                                                                                   IConfiguration configuration,
                                                                                   string connectionStringName,
                                                                                   string schema,
                                                                                   object key)
        where TContextService : class
        where TContextImplementation : DbContext, TContextService
    {
        void ConfigureDbContextOptions(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
        {
            var connectionString = configuration.GetConnectionString(connectionStringName);

            builder.UseSqlServer(connectionString, options =>
            {
                options.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema);
                options.EnableRetryOnFailure();
            }).EnableDetailedErrors();

            var globalInterceptors = serviceProvider.GetServices<IInterceptor>();
            var keyedInterceptors = serviceProvider.GetKeyedServices<IInterceptor>(key);

            builder.AddInterceptors(globalInterceptors);
            builder.AddInterceptors(keyedInterceptors);
        }

        services.AddDbContext<TContextImplementation>(ConfigureDbContextOptions);

        services.AddKeyedScoped<TContextService>(key, (provider, _) => provider.GetRequiredService<TContextImplementation>());
    }
    #endregion
}