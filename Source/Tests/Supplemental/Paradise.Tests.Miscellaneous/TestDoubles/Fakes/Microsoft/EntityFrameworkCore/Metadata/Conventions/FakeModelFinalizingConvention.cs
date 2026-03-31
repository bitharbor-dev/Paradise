using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.EntityFrameworkCore.Metadata.Conventions;

/// <summary>
/// Fake <see cref="IModelFinalizingConvention"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeModelFinalizingConvention"/> class.
/// </remarks>
/// <param name="databaseProvider">
/// The <see cref="IDatabaseProvider"/> used to
/// determine the database provider name and
/// perform the model finalizing.
/// </param>
internal class FakeModelFinalizingConvention(IDatabaseProvider databaseProvider) : IModelFinalizingConvention
{
    #region Constants
    private const string SqliteProviderName = "Microsoft.EntityFrameworkCore.Sqlite";
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        var providerName = databaseProvider.Name;

        if (providerName is not SqliteProviderName)
        {
            var message = $"{providerName} is not supported by this model finalizing convention.";

            throw new InvalidOperationException(message);
        }

        foreach (var entity in modelBuilder.Metadata.GetEntityTypes())
            NormalizeEntity(entity);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Normalizes entity-level relational metadata.
    /// </summary>
    /// <param name="entity">
    /// The <see cref="IConventionEntityType"/> which properties to normalize.
    /// </param>
    /// <remarks>
    /// Add any entity-wide normalization to this method.
    /// <para>
    /// Add any property-wide normalization to the <see cref="NormalizeProperty"/> method.
    /// </para>
    /// </remarks>
    private static void NormalizeEntity(IConventionEntityType entity)
    {
        foreach (var property in entity.GetProperties())
            NormalizeProperty(property);
    }

    /// <summary>
    /// Reads the property metadata and performs
    /// database provider-specific normalization
    /// to SQLite database provider for proper tests execution.
    /// </summary>
    /// <param name="property">
    /// The <see cref="IConventionProperty"/> to normalize.
    /// </param>
    private static void NormalizeProperty(IConventionProperty property)
        => Debug.WriteLine(property.Name);
    #endregion
}