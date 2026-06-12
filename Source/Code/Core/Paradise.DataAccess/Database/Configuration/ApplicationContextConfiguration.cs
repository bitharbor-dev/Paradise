using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Paradise.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Database.Configuration.Converters;
using Paradise.DataAccess.Database.Configuration.Extensions;
using Paradise.Domain.Base;
using static Paradise.DataAccess.Database.ApplicationContext;
using static Paradise.DataAccess.Database.Configuration.ApplicationContextTables;

namespace Paradise.DataAccess.Database.Configuration;

/// <summary>
/// Contains <see cref="ApplicationContext"/> entities configuration.
/// </summary>
internal static class ApplicationContextConfiguration
{
    #region Public methods
    /// <summary>
    /// Configures <see cref="ApplicationContext"/> entities.
    /// </summary>
    /// <param name="modelBuilder">
    /// The builder being used to construct the model for this context.
    /// </param>
    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRefreshToken>(builder =>
        {
            builder.ToTable(UserRefreshTokens, InfrastructureSchemeName);
            builder.HasKey(entity => entity.Id);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(entity => entity.OwnerId);

            builder.Property(entity => entity.OwnerId)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(entity => entity.ExpiryDateUtc)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<EmailTemplate>(builder =>
        {
            builder.ToTable(EmailTemplates, InfrastructureSchemeName);
            builder.HasKey(entity => entity.Id);

            builder.HasIndex(nameof(EmailTemplate.TemplateName),
                             nameof(EmailTemplate.Culture)).IsUnique();

            builder.Property(entity => entity.Culture)
                   .HasConversion<CultureInfoConverter>();
        });

        modelBuilder.Model.MarkPropertyAsReadOnly(nameof(IDomainObject.Created));
        modelBuilder.Model.DisableValueGenerationFor(nameof(User.Id), typeof(User));
        modelBuilder.Model.DisableValueGenerationFor(nameof(Role.Id), typeof(Role));
        modelBuilder.Model.DisableValueGenerationFor(nameof(Entity.Id), typeof(Entity));
        modelBuilder.Model.DisableValueGenerationFor(nameof(ValueObject.Id), typeof(ValueObject));

        modelBuilder.Model.SetSchemaFor<DataProtectionKey>(InfrastructureSchemeName);
        modelBuilder.Model.SetSchemaFor<User>(InfrastructureSchemeName);
        modelBuilder.Model.SetSchemaFor<Role>(InfrastructureSchemeName);
        modelBuilder.Model.SetSchemaFor<IdentityUserClaim<Guid>>(InfrastructureSchemeName);
        modelBuilder.Model.SetSchemaFor<IdentityRoleClaim<Guid>>(InfrastructureSchemeName);
        modelBuilder.Model.SetSchemaFor<IdentityUserLogin<Guid>>(InfrastructureSchemeName);
        modelBuilder.Model.SetSchemaFor<IdentityUserToken<Guid>>(InfrastructureSchemeName);
        modelBuilder.Model.SetSchemaFor<IdentityUserRole<Guid>>(InfrastructureSchemeName);
    }
    #endregion
}