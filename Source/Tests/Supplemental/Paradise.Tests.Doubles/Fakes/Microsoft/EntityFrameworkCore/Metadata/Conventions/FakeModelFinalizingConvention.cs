using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage;
using Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.Globalization;
using System.Text;

namespace Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Metadata.Conventions;

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
/// <param name="entityNormalizers">
/// Entity normalizers.
/// </param>
internal class FakeModelFinalizingConvention(IDatabaseProvider databaseProvider, IEnumerable<IEntityNormalizer> entityNormalizers)
    : IModelFinalizingConvention
{
    #region Constants
    private const string SqliteProviderName = "Microsoft.EntityFrameworkCore.Sqlite";
    #endregion

    #region Fields
    private static readonly CompositeFormat _invalidProviderMessageFormat = CompositeFormat
        .Parse("{0} is not supported by this model finalizing convention");
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        var providerName = databaseProvider.Name;

        if (providerName is not SqliteProviderName)
        {
            var message = string.Format(CultureInfo.InvariantCulture,
                                        _invalidProviderMessageFormat,
                                        providerName);

            throw new InvalidOperationException(message);
        }

        foreach (var entity in modelBuilder.Metadata.GetEntityTypes())
        {
            foreach (var normalizer in entityNormalizers)
                normalizer.TryNormalize(entity);
        }
    }
    #endregion
}