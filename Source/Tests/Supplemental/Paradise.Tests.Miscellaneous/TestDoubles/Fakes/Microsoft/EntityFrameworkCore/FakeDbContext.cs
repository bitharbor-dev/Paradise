using Microsoft.EntityFrameworkCore;
using Paradise.Tests.Miscellaneous.TestImplementations.Core.Domain.Base;
using Xunit;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.EntityFrameworkCore;

/// <summary>
/// Fake <see cref="DbContext"/> implementation.
/// </summary>
public sealed class FakeDbContext : DbContext
{
    #region Fields
    private readonly Action<ModelBuilder> _onModelCreating;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeDbContext"/> class.
    /// </summary>
    /// <param name="options">
    /// The options for this context.
    /// </param>
    /// <param name="onModelCreating">
    /// A delegate to configure the <see cref="DbContext.OnModelCreating"/>
    /// method at runtime.
    /// </param>
    public FakeDbContext(DbContextOptions<FakeDbContext> options, Action<ModelBuilder> onModelCreating) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(onModelCreating);

        _onModelCreating = onModelCreating;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeDbContext"/> class.
    /// </summary>
    /// <param name="onModelCreating">
    /// A delegate to configure the <see cref="DbContext.OnModelCreating"/>
    /// method at runtime.
    /// </param>
    public FakeDbContext(Action<ModelBuilder> onModelCreating) : base(GetDefaultOptions())
        => _onModelCreating = onModelCreating;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeDbContext"/> class.
    /// </summary>
    public FakeDbContext() : base(GetDefaultOptions())
    {
        _onModelCreating = modelBuilder =>
        {
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
        };
    }
    #endregion

    #region Protected methods
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _onModelCreating(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Creates and returns the default <see cref="DbContextOptions{TContext}"/>
    /// of <see cref="FakeDbContext"/>, configured to use
    /// an in-memory database scoped to the current test.
    /// </summary>
    /// <remarks>
    /// The database name is derived from <see cref="TestContext.Current"/>'s unique test ID
    /// to ensure isolation between tests.
    /// </remarks>
    /// <returns>
    /// A configured <see cref="DbContextOptions{TContext}"/> instance for use in tests.
    /// </returns>
    private static DbContextOptions<FakeDbContext> GetDefaultOptions()
    {
        var builder = new DbContextOptionsBuilder<FakeDbContext>()
            .UseInMemoryDatabase(TestContext.Current.Test!.UniqueID);

        return builder.Options;
    }
    #endregion
}