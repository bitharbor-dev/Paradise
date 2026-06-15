using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Paradise.DataAccess.Database;
using Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Migrations;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

namespace Paradise.Tests.Extensibility.Web.Hosting.Configuration;

/// <summary>
/// Replaces the application's data source registration with a SQLite-backed
/// <see cref="DbContext"/> created from a custom connection factory.
/// </summary>
/// <param name="connection">
/// A <see cref="SqliteConnection"/> to be used by the created <see cref="DbContext"/> instances.
/// </param>
public sealed class DataSourceConfiguration(SqliteConnection connection) : IWebApplicationServicesConfiguration
{
    #region Public methods
    /// <inheritdoc/>
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .RemoveAll<IDbContextOptionsConfiguration<ApplicationContext>>()
            .ConfigureDbContext<ApplicationContext>((provider, builder) =>
            {
                builder
                    .UseSqlite(connection)
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging();

                var interceptors = provider.GetServices<IInterceptor>();

                builder.AddInterceptors(interceptors);
                builder.ReplaceService<IMigrator, FakeMigrator>();
                builder.ReplaceService<IProviderConventionSetBuilder, FakeConventionSetBuilder>();
            }, ServiceLifetime.Scoped);
    }
    #endregion
}