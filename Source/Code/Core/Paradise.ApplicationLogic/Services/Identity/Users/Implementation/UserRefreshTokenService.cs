using Paradise.ApplicationLogic.DataConverters.Domain.Identity.Users;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.DataAccess.Repositories;
using Paradise.Domain.Identity.Users;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Users;
using static Paradise.Models.ErrorCode;
using static Paradise.Models.OperationStatus;

namespace Paradise.ApplicationLogic.Services.Identity.Users.Implementation;

/// <summary>
/// Provides users' refresh tokens management functionalities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserRefreshTokenService"/> class.
/// </remarks>
/// <param name="timeProvider">
/// Time provider.
/// </param>
/// <param name="userManager">
/// User manager.
/// </param>
/// <param name="unitOfWork">
/// Unit of work.
/// </param>
internal sealed class UserRefreshTokenService(TimeProvider timeProvider, IUserManager<User> userManager, IDomainUnitOfWork unitOfWork)
    : IUserRefreshTokenService
{
    #region Public methods
    /// <inheritdoc/>
    public async Task<Result<UserRefreshTokenModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var refreshToken = await unitOfWork
            .UserRefreshTokensRepository
            .GetByIdAsync(id, cancellationToken)
            .ConfigureAwait(false);

        return refreshToken is null
            ? new(Missing, UserRefreshTokenIdNotFound, id)
            : new(refreshToken.ToModel(), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<UserRefreshTokenModel>> CreateAsync(Guid userId, TimeSpan lifetime,
                                                                 CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId)
            .ConfigureAwait(false);

        if (user is null)
            return new(Missing, UserIdNotFound, userId);

        var expiryDateUtc = timeProvider.GetUtcNow().Add(lifetime);

        var refreshtToken = new UserRefreshToken(user.Id, expiryDateUtc)
        {
            IsActive = true
        };

        unitOfWork.UserRefreshTokensRepository.Add(refreshtToken);

        await unitOfWork.CommitAsync(cancellationToken)
            .ConfigureAwait(false);

        return new(refreshtToken.ToModel(), Created);
    }

    /// <inheritdoc/>
    public async Task<Result<UserRefreshTokenModel>> DeactivateAsync(Guid userId, Guid id,
                                                                     CancellationToken cancellationToken = default)
    {
        var refreshTokens = await unitOfWork
            .UserRefreshTokensRepository
            .GetUserTokensAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        var refreshToken = refreshTokens.FirstOrDefault(token => token.Id == id);

        if (refreshToken is null)
            return new(Missing, UserRefreshTokenIdNotFound, id);

        refreshToken.IsActive = false;

        await unitOfWork.CommitAsync(cancellationToken)
            .ConfigureAwait(false);

        return new(refreshToken.ToModel(), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<UserRefreshTokenModel>>> DeactivateAllAsync(Guid userId,
                                                                                     CancellationToken cancellationToken = default)
    {
        var refreshTokens = await unitOfWork
            .UserRefreshTokensRepository
            .GetUserTokensAsync(userId, cancellationToken)
            .ConfigureAwait(false);

        refreshTokens.ForEach(refreshToken => refreshToken.IsActive = false);

        await unitOfWork.CommitAsync(cancellationToken)
            .ConfigureAwait(false);

        return new(refreshTokens.Select(refreshToken => refreshToken.ToModel()), Success);
    }

    /// <inheritdoc/>
    public async Task<Result<int>> DeleteExpiredAsync(CancellationToken cancellationToken = default)
    {
        var currentTime = timeProvider.GetUtcNow();

        unitOfWork.UserRefreshTokensRepository.RemoveWhere(token => token.ExpiryDateUtc < currentTime);

        var itemsNumber = await unitOfWork.CommitAsync(cancellationToken)
            .ConfigureAwait(false);

        return new(itemsNumber, Success);
    }
    #endregion
}