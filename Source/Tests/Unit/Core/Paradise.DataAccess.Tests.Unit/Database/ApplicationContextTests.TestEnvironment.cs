using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Paradise.DataAccess.Database;
using Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Migrations;
using Paradise.Tests.Surrogates.Core.Domain.Base;
using System.Data.Common;

namespace Paradise.DataAccess.Tests.Unit.Database;

public sealed partial class ApplicationContextTests : IDisposable
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void Dispose()
        => Test.Dispose();
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior-check methods for the <see cref="ApplicationContextTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly DbConnection _connection;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }

        /// <summary>
        /// Creates and persists an <see cref="TestNamedEntity"/> entity in the in-memory database.
        /// </summary>
        /// <remarks>
        /// This method creates a short-lived <see cref="ApplicationContext"/> bound to the shared
        /// in-memory database connection, ensures the database schema exists, and saves a new
        /// <see cref="TestNamedEntity"/> instance.
        /// </remarks>
        /// <returns>
        /// The persisted <see cref="TestNamedEntity"/> entity.
        /// </returns>
        public TestNamedEntity AddEntity()
        {
            var options = GetContextOptions(_connection, true);
            using var context = new ApplicationContext(options);

            context.Database.EnsureCreated();

            var entity = new TestNamedEntity();

            context.Add(entity);
            context.SaveChanges();

            return entity;
        }

        /// <summary>
        /// Creates a new <see cref="ApplicationContext"/> instance using the shared in-memory database connection.
        /// </summary>
        /// <param name="ensureCreated">
        /// Indicates whether <see cref="DatabaseFacade.EnsureCreated"/> should be invoked
        /// to create the database schema if it does not already exist.
        /// </param>
        /// <param name="useFakeModel">
        /// Indicates whether the fake <see cref="IModel"/> instance should be used to override
        /// the default <see cref="ApplicationContext"/> model.
        /// </param>
        /// <returns>
        /// System under test.
        /// </returns>
        public ApplicationContext CreateContext(bool ensureCreated = true, bool useFakeModel = true)
        {
            var options = GetContextOptions(_connection, useFakeModel);

            var context = new ApplicationContext(options);

            if (ensureCreated)
                context.Database.EnsureCreated();

            return context;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Builds <see cref="DbContextOptions{TContext}"/> for <see cref="ApplicationContext"/>
        /// using the given database <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">
        /// An open <see cref="DbConnection"/> that backs the in-memory database.
        /// </param>
        /// <param name="useFakeModel">
        /// Indicates whether the fake <see cref="IModel"/> instance should be used to override
        /// the default <see cref="ApplicationContext"/> model.
        /// </param>
        /// <returns>
        /// Configured <see cref="DbContextOptions{TContext}"/> instance.
        /// </returns>
        private static DbContextOptions<ApplicationContext> GetContextOptions(DbConnection connection, bool useFakeModel)
        {
            var builder = new DbContextOptionsBuilder<ApplicationContext>()
                .UseSqlite(connection)
                .ReplaceService<IMigrator, FakeMigrator>()
                .ReplaceService<IProviderConventionSetBuilder, FakeConventionSetBuilder>();

            if (useFakeModel)
                builder.UseModel(BuildFakeModel());

            return builder.Options;
        }

        /// <summary>
        /// Builds a fake model used to replace the original <see cref="ApplicationContext"/> model.
        /// </summary>
        /// <returns>
        /// Fake <see cref="IModel"/> instance.
        /// </returns>
        private static IModel BuildFakeModel()
        {
            var modelBuilder = new ModelBuilder(SqliteConventionSetBuilder.Build());

            modelBuilder.Entity<TestNamedEntity>(builder =>
            {
                builder.ToTable("NamedEntities");
                builder.HasKey(entity => entity.Id);
            });

            modelBuilder.Entity<TestRelationalEntity>(builder =>
            {
                builder.ToTable("RelationalEntities");
                builder.HasKey(entity => entity.Id);

                builder.HasOne(entity => entity.Child)
                       .WithOne(entity => entity.Parent)
                       .HasForeignKey<TestRelationalValueObject>(entity => entity.ParentId);
            });

            modelBuilder.Entity<TestRelationalValueObject>(builder =>
            {
                builder.ToTable("RelationalValueObjects");
                builder.HasKey(entity => entity.Id);
            });

            return modelBuilder.FinalizeModel();
        }
        #endregion
    }
    #endregion
}