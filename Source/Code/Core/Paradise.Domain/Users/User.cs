using Microsoft.AspNetCore.Identity;
using Paradise.Common.Extensions;
using Paradise.Domain.Base;
using Paradise.Domain.Base.EqualityComparers;
using Paradise.Domain.Base.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Users;

/// <inheritdoc/>
public sealed class User : IdentityUser<Guid>, IEntity, IDatabaseRecord, IEquatable<User>
{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="email">
    /// Email address.
    /// </param>
    /// <param name="userName">
    /// User-name.
    /// </param>
    /// <remarks>
    /// User lock out is enabled by default.
    /// </remarks>
    public User(string email, string userName) : base(userName)
        => Email = email;
    #endregion

    #region Properties
    /// <inheritdoc/>
    public DateTime Created { get; set; }

    /// <inheritdoc/>
    public DateTime Modified { get; set; }

    /// <inheritdoc/>
    [NotNull]
    public override string? Email
    {
        get => base.Email!;
        set => base.Email = value;
    }

    /// <inheritdoc/>
    [NotNull]
    public override string? UserName
    {
        get => base.UserName!;
        set => base.UserName = value;
    }

    /// <summary>
    /// Indicates whether the user is pending deletion.
    /// </summary>
    [NotMapped]
    public bool IsPendingDeletion
        => DeletionRequestSubmitted.HasValue;

    /// <summary>
    /// Deletion request submission date.
    /// </summary>
    public DateTime? DeletionRequestSubmitted { get; set; }

    /// <summary>
    /// User's refresh tokens.
    /// </summary>
    public ICollection<UserRefreshToken>? RefreshTokens { get; private set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    [MemberNotNull(nameof(Email))]
    [MemberNotNull(nameof(UserName))]
    public void ValidateState()
    {
        if (Email.IsNullOrWhiteSpace())
            InvalidEntityStateException.Throw<User>(Email);

        if (UserName.IsNullOrWhiteSpace())
            InvalidEntityStateException.Throw<User>(UserName);
    }

    /// <summary>
    /// Cancels the user's deletion request by
    /// setting <see cref="DeletionRequestSubmitted"/> to <see langword="null"/>.
    /// </summary>
    public void CancelDeletionRequest()
        => DeletionRequestSubmitted = null;

    /// <summary>
    /// Gets the <see cref="bool"/> value
    /// indicating whether the current user's
    /// deletion request is outdated.
    /// </summary>
    /// <param name="lifetime">
    /// Deletion request lifetime.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if deletion request is outdated,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public bool IsDeletionRequestOutdated(TimeSpan lifetime)
        => DeletionRequestSubmitted.HasValue
        && DeletionRequestSubmitted.Value.Add(lifetime) < DateTime.UtcNow;

    /// <summary>
    /// Gets the <see cref="bool"/> value
    /// indicating whether the current user's
    /// email address confirmation
    /// period is exceeded.
    /// </summary>
    /// <param name="confirmationPeriod">
    /// Email confirmation period.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if email address confirmation period is exceeded,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public bool IsEmailConfirmationPeriodExceeded(TimeSpan confirmationPeriod)
        => Created.Add(confirmationPeriod) < DateTime.UtcNow;

    /// <inheritdoc/>
    public bool Equals(User? other)
        => other == this;

    /// <inheritdoc/>
    public sealed override bool Equals(object? obj)
        => obj is User entity && Equals(entity);

    /// <inheritdoc/>
    public sealed override int GetHashCode()
        => EntityEqualityComparer.Instance.GetHashCode(this);
    #endregion

    #region Operators
    /// <summary>
    /// Compares the given <paramref name="left"/> and <paramref name="right"/>
    /// objects for equality.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="User"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="User"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> equals <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator ==(User? left, User? right)
        => EntityEqualityComparer.Instance.Equals(left, right);

    /// <summary>
    /// Compares the given <paramref name="left"/> and <paramref name="right"/>
    /// objects for inequality.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="User"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="User"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> does not equal <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator !=(User? left, User? right)
        => !(left == right);
    #endregion
}