using Paradise.DataAccess.Seed.Models.ApplicationLogic;
using Paradise.DataAccess.Seed.Models.Domain;
using Paradise.DataAccess.Seed.Providers;

namespace Paradise.Tests.Doubles.Stubs.Core.DataAccess.Seed.Providers;

/// <summary>
/// Stub <see cref="ISeedDataProvider"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StubSeedDataProvider"/> class.
/// </remarks>
/// <param name="domainData">
/// Contains domain seed data.
/// </param>
/// <param name="infrastructureData">
/// Contains infrastructure seed data.
/// </param>
public sealed class StubSeedDataProvider(DomainDataSeedModel domainData, InfrastructureDataSeedModel infrastructureData)
    : ISeedDataProvider
{
    #region Properties
    /// <inheritdoc/>
    public DomainDataSeedModel DomainData { get; } = domainData;

    /// <inheritdoc/>
    public InfrastructureDataSeedModel InfrastructureData { get; } = infrastructureData;
    #endregion
}