using Microsoft.Extensions.DependencyInjection;
using Paradise.ApplicationLogic.Infrastructure.Domain.Events.Identity;
using Paradise.ApplicationLogic.Infrastructure.Services.Identity;
using Paradise.Domain.Base.Events;
using Paradise.Models;

namespace Paradise.ApplicationLogic.Infrastructure.EventListeners.Domain.Identity;

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

        await using (scope.ConfigureAwait(false))
        {

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

            if (errors.Count > 0)
            {
                var message = string.Join(Environment.NewLine, errors);

                throw new InvalidOperationException(message);
            }
        }
    }
    #endregion
}