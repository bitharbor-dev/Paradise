using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.EntityFrameworkCore.Migrations;

/// <summary>
/// Fake <see cref="IMigrator"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeMigrator"/> class.
/// </remarks>
/// <param name="databaseCreator">
/// Database creator.
/// </param>
public sealed class FakeMigrator(IDatabaseCreator databaseCreator) : IMigrator
{
    #region Public methods
    /// <inheritdoc/>
    public string GenerateScript(string? fromMigration = null, string? toMigration = null,
                                 MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
        => string.Empty;

    /// <inheritdoc/>
    public bool HasPendingModelChanges()
        => false;

    /// <inheritdoc/>
    public void Migrate(string? targetMigration = null)
        => databaseCreator.EnsureCreated();

    /// <inheritdoc/>
    public Task MigrateAsync(string? targetMigration = null, CancellationToken cancellationToken = default)
        => databaseCreator.EnsureCreatedAsync(cancellationToken);
    #endregion
}