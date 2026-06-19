using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Paradise.DataAccess.Database;
using Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Migrations;
using Paradise.Tests.Extensibility.Web.Hosting.Configuration.Base;

namespace Paradise.Tests.Extensibility.Web.Hosting.Configuration;

/// <summary>
/// Replaces the application's data source registration with a SQLite-backed
/// <see cref="DbContext"/> using the provided <paramref name="connection"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DataSourceConfiguration"/> class.
/// </remarks>
/// <param name="connection">
/// A <see cref="SqliteConnection"/> to be used by the created <see cref="DbContext"/> instances.
/// </param>
public sealed class DataSourceConfiguration(SqliteConnection connection) : IWebApplicationServicesConfiguration
{
    #region Properties
    /// <summary>
    /// An action to be executed upon logging SQL activities.
    /// </summary>
    public Action<string>? SqlLoggerDelegate { get; set; }

    /// <summary>
    /// The minimum log level for logging SQL activities.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="LogLevel.Warning"/>.
    /// </remarks>
    public LogLevel SqlLoggerLevel { get; set; } = LogLevel.Warning;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
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
                (builder as IDbContextOptionsBuilderInfrastructure).AddOrUpdateExtension(new EntityNormalizerOptionsExtension());

                if (SqlLoggerDelegate is not null)
                    builder.LogTo(SqlLoggerDelegate, SqlLoggerLevel);
            }, ServiceLifetime.Scoped);
    }
    #endregion
}