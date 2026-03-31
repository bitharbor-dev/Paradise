using Paradise.ApplicationLogic.Services.Identity.Roles;
using Paradise.Models;
using Paradise.Models.Domain.Identity.Roles;

namespace Paradise.Tests.Miscellaneous.TestDoubles.Stubs.Core.ApplicationLogic.Services.Identity.Roles;

/// <summary>
/// Stub <see cref="IRoleService"/> implementation.
/// </summary>
public sealed class StubRoleService : IRoleService
{
    #region Properties
    /// <summary>
    /// <see cref="GetAllAsync"/> result.
    /// </summary>
    public Func<Task<Result<IEnumerable<RoleModel>>>>? GetAllAsyncResult { get; set; }

    /// <summary>
    /// <see cref="GetByIdAsync"/> result.
    /// </summary>
    public Func<Task<Result<RoleModel>>>? GetByIdAsyncResult { get; set; }

    /// <summary>
    /// <see cref="GetUserRolesAsync"/> result.
    /// </summary>
    public Func<Task<Result<IEnumerable<RoleModel>>>>? GetUserRolesAsyncResult { get; set; }

    /// <summary>
    /// <see cref="CreateAsync"/> result.
    /// </summary>
    public Func<Task<Result<RoleModel>>>? CreateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="UpdateAsync"/> result.
    /// </summary>
    public Func<Task<Result<RoleModel>>>? UpdateAsyncResult { get; set; }

    /// <summary>
    /// <see cref="DeleteAsync"/> result.
    /// </summary>
    public Func<Task<Result<IEnumerable<RoleModel>>>>? DeleteAsyncResult { get; set; }

    /// <summary>
    /// <see cref="AssignAsync"/> result.
    /// </summary>
    public Func<Task<Result<IEnumerable<RoleModel>>>>? AssignAsyncResult { get; set; }

    /// <summary>
    /// <see cref="UnassignAsync"/> result.
    /// </summary>
    public Func<Task<Result<IEnumerable<RoleModel>>>>? UnassignAsyncResult { get; set; }
    #endregion

    #region Public methods
    /// <inheritdoc/>
    public Task<Result<IEnumerable<RoleModel>>> GetAllAsync(bool? isDefault = null, CancellationToken cancellationToken = default)
        => GetAllAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<RoleModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => GetByIdAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<IEnumerable<RoleModel>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        => GetUserRolesAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<RoleModel>> CreateAsync(RoleCreationModel model, CancellationToken cancellationToken = default)
        => CreateAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<RoleModel>> UpdateAsync(Guid id, RoleUpdateModel model, CancellationToken cancellationToken = default)
        => UpdateAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<IEnumerable<RoleModel>>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => DeleteAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<IEnumerable<RoleModel>>> AssignAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default)
        => AssignAsyncResult!();

    /// <inheritdoc/>
    public Task<Result<IEnumerable<RoleModel>>> UnassignAsync(Guid roleId, Guid userId, CancellationToken cancellationToken = default)
        => UnassignAsyncResult!();
    #endregion
}