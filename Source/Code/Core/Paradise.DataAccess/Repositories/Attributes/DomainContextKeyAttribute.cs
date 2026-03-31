using Microsoft.Extensions.DependencyInjection;
using Paradise.DataAccess.Database;

namespace Paradise.DataAccess.Repositories.Attributes;

/// <summary>
/// A shortcut class for resolving the keyed <see cref="IDataSource"/> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DomainContextKeyAttribute"/> class.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class DomainContextKeyAttribute() : FromKeyedServicesAttribute(ContextKey)
{
    #region Constants
    /// <summary>
    /// The database context service key.
    /// </summary>
    public const string ContextKey = DomainContext.SchemeName;
    #endregion
}