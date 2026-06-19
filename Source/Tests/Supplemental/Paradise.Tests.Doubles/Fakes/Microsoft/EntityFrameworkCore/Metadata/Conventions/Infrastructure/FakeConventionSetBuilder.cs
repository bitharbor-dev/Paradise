using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

/// <summary>
/// Fake <see cref="IProviderConventionSetBuilder"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeConventionSetBuilder"/> class.
/// </remarks>
/// <param name="dependencies">
/// The core dependencies for this service.
/// </param>
/// <param name="relationalDependencies">
/// The relational dependencies for this service.
/// </param>
/// <param name="databaseProvider">
/// The database provider for this service.
/// </param>
/// <param name="entityNormalizers">
/// Entity normalizers.
/// </param>
public sealed class FakeConventionSetBuilder(ProviderConventionSetBuilderDependencies dependencies,
                                             RelationalConventionSetBuilderDependencies relationalDependencies,
                                             IDatabaseProvider databaseProvider,
                                             IEnumerable<IEntityNormalizer> entityNormalizers)
    : SqliteConventionSetBuilder(dependencies, relationalDependencies)
{
    #region Public methods
    /// <inheritdoc/>
    public override ConventionSet CreateConventionSet()
    {
        var set = base.CreateConventionSet();

        set.ModelFinalizingConventions.Add(new FakeModelFinalizingConvention(databaseProvider, entityNormalizers));

        return set;
    }
    #endregion
}