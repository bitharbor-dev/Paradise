using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Paradise.DataAccess.Database.Interceptors.Base;

/// <summary>
/// The base interface for all Entity Framework <see cref="DbContext"/>-specific interceptors.
/// </summary>
public interface IDbContextSpecificInterceptor : IInterceptor
{
    #region Properties
    /// <summary>
    /// Target <see cref="DbContext"/> type.
    /// </summary>
    Type DbContextType { get; }
    #endregion
}