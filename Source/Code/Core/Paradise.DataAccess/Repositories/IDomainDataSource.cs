using Paradise.DataAccess.Database;
using Paradise.DataAccess.Repositories.Base;

namespace Paradise.DataAccess.Repositories;

/// <summary>
/// An <see cref="IDataSource"/> interface segregation
/// to be used by the <see cref="DomainContext"/> class.
/// </summary>
public interface IDomainDataSource : IDataSource;