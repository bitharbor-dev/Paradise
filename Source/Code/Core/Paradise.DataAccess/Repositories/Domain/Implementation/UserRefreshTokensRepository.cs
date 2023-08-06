using Paradise.DataAccess.Repositories.Base.Implementation;
using Paradise.Domain.Users;

namespace Paradise.DataAccess.Repositories.Domain.Implementation;

/// <summary>
/// <see cref="UserRefreshToken"/> repository class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRefreshTokensRepository"/> class.
/// </remarks>
/// <param name="source">
/// Repository data source.
/// </param>
public sealed class UserRefreshTokensRepository(IDomainDataSource source) : Repository<UserRefreshToken>(source), IUserRefreshTokensRepository
{

}