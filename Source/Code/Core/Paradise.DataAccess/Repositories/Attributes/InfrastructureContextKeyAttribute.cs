using Microsoft.Extensions.DependencyInjection;
using Paradise.DataAccess.Database;

namespace Paradise.DataAccess.Repositories.Attributes;

/// <summary>
/// A shortcut class for resolving the keyed <see cref="IDataSource"/> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="InfrastructureContextKeyAttribute"/> class.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class InfrastructureContextKeyAttribute() : FromKeyedServicesAttribute(ContextKey)
{
    #region Constants
    /// <summary>
    /// The database context service key.
    /// </summary>
    public const string ContextKey = InfrastructureContext.SchemeName;
    #endregion
}