using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Paradise.DataAccess.Database;
using Paradise.DataAccess.Database.Interceptors;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity.Implementation;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates.Implementation;
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
        void ConfigureDbContextOptions(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
        {
            var connectionString = configuration.GetConnectionString(ApplicationContext.ConnectionStringName);

            builder.UseSqlServer(connectionString, options =>
            {
                options.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                options.EnableRetryOnFailure();
            }).EnableDetailedErrors();

            var interceptors = serviceProvider.GetServices<IInterceptor>();

            builder.AddInterceptors(interceptors);
        }

        services.TryAddSingleton(TimeProvider.System);

        services
            .AddSingleton<IInterceptor, OnCreatedInterceptor>()
            .AddSingleton<IInterceptor, OnModifiedInterceptor>()
            .AddDbContext<IDataSource, ApplicationContext>(ConfigureDbContextOptions);

        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IUserRefreshTokensRepository, UserRefreshTokensRepository>()
            .AddScoped<IEmailTemplatesRepository, EmailTemplatesRepository>();

        return services;
    }
    #endregion
}