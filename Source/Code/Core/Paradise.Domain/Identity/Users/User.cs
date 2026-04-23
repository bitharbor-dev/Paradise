using Microsoft.AspNetCore.Identity;
using Paradise.Common.Extensions;
using Paradise.Domain.Base;
using Paradise.Domain.Base.EqualityComparers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Identity.Users;

/// <inheritdoc/>
public sealed class User : IdentityUser<Guid>, IEntity, IEquatable<User>
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
    public DateTimeOffset Created { get; private set; }

    /// <inheritdoc/>
    public DateTimeOffset Modified { get; private set; }

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
    public DateTimeOffset? DeletionRequestSubmitted { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    [MemberNotNull(nameof(Email))]
    [MemberNotNull(nameof(UserName))]
    public void ValidateState()
    {
        if (Email.IsNullOrWhiteSpace())
            throw new InvalidOperationException(new DomainStateError<User>(Email));

        if (UserName.IsNullOrWhiteSpace())
            throw new InvalidOperationException(new DomainStateError<User>(UserName));
    }

    /// <inheritdoc/>
    public void OnCreated(DateTimeOffset utcNow)
    {
        Id = Guid.CreateVersion7(utcNow);
        Created = utcNow;

        ValidateState();
    }

    /// <inheritdoc/>
    public void OnModified(DateTimeOffset utcNow)
    {
        Modified = utcNow;

        ValidateState();
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
    /// <param name="currentTime">
    /// Current time.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if deletion request is outdated,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public bool CanBeDeleted(TimeSpan lifetime, DateTimeOffset currentTime)
        => DeletionRequestSubmitted.HasValue
        && DeletionRequestSubmitted.Value.Add(lifetime) > currentTime;

    /// <summary>
    /// Gets the <see cref="bool"/> value
    /// indicating whether the current user's
    /// email address confirmation
    /// period is exceeded.
    /// </summary>
    /// <param name="confirmationPeriod">
    /// Email confirmation period.
    /// </param>
    /// <param name="currentTime">
    /// Current time.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if email address confirmation period is exceeded,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public bool CanConfirmEmailAddress(TimeSpan confirmationPeriod, DateTimeOffset currentTime)
        => Created.Add(confirmationPeriod) < currentTime;

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