using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Services.Identity.Roles;
using Paradise.Domain.Base.Events;
using Paradise.Domain.Events.Identity.Users;
using Paradise.Models;

namespace Paradise.ApplicationLogic.EventListeners.Domain.Identity.Users;

/// <summary>
/// Reacts to the <see cref="EmailAddressConfirmedEvent"/> by assigning
/// default application roles to the confirmed user.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AssignDefaultUserRoles"/> class.
/// </remarks>
/// <param name="serviceProvider">
/// The <see cref="IServiceProvider"/> instance used to resolve scoped dependencies.
/// </param>
internal sealed class AssignDefaultUserRoles(IServiceProvider serviceProvider) : IDomainEventListener<EmailAddressConfirmedEvent>
{
    #region Public methods
    /// <inheritdoc/>
    public async Task ProcessAsync(EmailAddressConfirmedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var scope = serviceProvider.CreateAsyncScope();
        var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();

        var errors = new List<ApplicationError>();

        var defaultRolesResult = await roleService.GetAllAsync(true, cancellationToken)
            .ConfigureAwait(false);

        if (defaultRolesResult.Value is not null)
        {
            foreach (var role in defaultRolesResult.Value)
            {
                var assignResult = await roleService.AssignAsync(role.Id, domainEvent.UserId, cancellationToken)
                    .ConfigureAwait(false);

                errors.AddRange(assignResult.Errors);
            }
        }

        await scope.DisposeAsync()
            .ConfigureAwait(false);

        if (errors.Count > 0)
        {
            var message = string.Join(Environment.NewLine, errors);

            throw new InvalidOperationException(message);
        }
    }
    #endregion
}