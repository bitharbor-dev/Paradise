using Paradise.DataAccess.Seed.Models.ApplicationLogic;
using Paradise.DataAccess.Seed.Models.Domain;

namespace Paradise.DataAccess.Seed.Providers;

/// <summary>
/// An abstraction to provide the database seed data.
/// </summary>
public interface ISeedDataProvider
{
    #region Properties
    /// <summary>
    /// Contains domain seed data.
    /// </summary>
    DomainDataSeedModel DomainData { get; }

    /// <summary>
    /// Contains infrastructure seed data.
    /// </summary>
    InfrastructureDataSeedModel InfrastructureData { get; }
    #endregion
}