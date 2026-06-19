using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Paradise.Tests.Doubles.Fakes.Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

/// <summary>
/// An extension that integrates the custom entity normalizers
/// into the internal <see cref="DbContext"/> service provider.
/// </summary>
public sealed class EntityNormalizerOptionsExtension : IDbContextOptionsExtension
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNormalizerOptionsExtension"/> class.
    /// </summary>
    public EntityNormalizerOptionsExtension()
        => Info = new EntityNormalizerOptionsExtensionInfo(this);
    #endregion

    #region Properties
    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info { get; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    /// <remarks>
    /// Register all <see cref="IEntityNormalizer"/> implementations here for further use in
    /// <see cref="FakeConventionSetBuilder"/>.
    /// </remarks>
    public void ApplyServices(IServiceCollection services) { }

    /// <inheritdoc/>
    public void Validate(IDbContextOptions options) { }
    #endregion

    #region Nested types
    /// <summary>
    /// Information/metadata for an <see cref="EntityNormalizerOptionsExtension"/>.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="EntityNormalizerOptionsExtensionInfo"/> class.
    /// </remarks>
    /// <param name="extension">
    /// The extension.
    /// </param>
    private sealed class EntityNormalizerOptionsExtensionInfo(EntityNormalizerOptionsExtension extension)
        : DbContextOptionsExtensionInfo(extension)
    {
        #region Properties
        /// <inheritdoc/>
        public override bool IsDatabaseProvider { get; }

        /// <inheritdoc/>
        public override string LogFragment { get; } = "";
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public override int GetServiceProviderHashCode()
            => 0;

        /// <inheritdoc/>
        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo) { }

        /// <inheritdoc/>
        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            => true;
        #endregion
    }
    #endregion
}