using Microsoft.Data.Sqlite;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Tests.Extensibility;

/// <summary>
/// Contains extension methods for the <see cref="SqliteConnection"/> <see langword="class"/>.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "False positive on extension members.")]
public static class SqliteConnectionExtensions
{
    extension(SqliteConnection)
    {
        #region Public methods
        /// <summary>
        /// Creates and opens an in-memory <see cref="SqliteConnection"/> that is isolated
        /// to the current test execution and can be shared across multiple database contexts.
        /// </summary>
        /// <param name="dataSource">
        /// Database name.
        /// </param>
        /// <returns>
        /// The opened in-memory <see cref="SqliteConnection"/>.
        /// </returns>
        public static SqliteConnection InitializeInMemoryConnection(string dataSource)
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = dataSource,
                Mode = SqliteOpenMode.Memory,
                Cache = SqliteCacheMode.Shared
            }.ToString();

            var connection = new SqliteConnection(connectionString);
            connection.Open();

            return connection;
        }
        #endregion
    }
}