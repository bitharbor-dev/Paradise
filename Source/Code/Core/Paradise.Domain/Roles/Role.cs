using Microsoft.AspNetCore.Identity;
using Paradise.Common.Extensions;
using Paradise.Domain.Base;
using Paradise.Domain.Base.EqualityComparers;
using Paradise.Domain.Base.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Paradise.Domain.Roles;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Role"/> class.
/// </remarks>
/// <param name="name">
/// Role name.
/// </param>
/// <param name="isDefault">
/// Indicates whether the role is default and should be
/// assigned automatically when a user has been created.
/// </param>
public sealed class Role(string name, bool isDefault) : IdentityRole<Guid>(name), IEntity, IDatabaseRecord, IEquatable<Role>
{
    #region Properties
    /// <inheritdoc/>
    public DateTime Created { get; set; }

    /// <inheritdoc/>
    public DateTime Modified { get; set; }

    /// <inheritdoc/>
    [NotNull]
    public override string? Name
    {
        get => base.Name!;
        set => base.Name = value;
    }

    /// <summary>
    /// Indicates whether the role is default and should be
    /// assigned automatically when a user has been created.
    /// </summary>
    public bool IsDefault { get; set; } = isDefault;
    #endregion

    #region Public methods
    /// <inheritdoc/>
    [MemberNotNull(nameof(Name))]
    public void ValidateState()
    {
        if (Name.IsNullOrWhiteSpace())
            InvalidEntityStateException.Throw<Role>(Name);
    }

    /// <inheritdoc/>
    public bool Equals(Role? other)
        => other == this;

    /// <inheritdoc/>
    public sealed override bool Equals(object? obj)
        => obj is Role entity && Equals(entity);

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
    /// The first <see cref="Role"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="Role"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> equals <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Role? left, Role? right)
        => EntityEqualityComparer.Instance.Equals(left, right);

    /// <summary>
    /// Compares the given <paramref name="left"/> and <paramref name="right"/>
    /// objects for inequality.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="Role"/> to be compared.
    /// </param>
    /// <param name="right">
    /// The second <see cref="Role"/> to be compared.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> does not equal <paramref name="right"/>,
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Role? left, Role? right)
        => !(left == right);
    #endregion
}