using Microsoft.EntityFrameworkCore;
using Paradise.DataAccess.Database.Configuration.Extensions;
using Paradise.Domain.Base;
using Paradise.Domain.Identity.Roles;
using Paradise.Domain.Identity.Users;
using static Paradise.DataAccess.Database.Configuration.Tables.DomainContextTables;

namespace Paradise.DataAccess.Database.Configuration;

/// <summary>
/// Contains <see cref="DomainContext"/> entities configuration.
/// </summary>
internal static class DomainContextConfiguration
{
    #region Public methods
    /// <summary>
    /// Configures <see cref="DomainContext"/> entities.
    /// </summary>
    /// <param name="modelBuilder">
    /// The builder being used to construct the model for this context.
    /// </param>
    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.Property(entity => entity.Id)
                   .ValueGeneratedNever();
        });

        modelBuilder.Entity<Role>(builder =>
        {
            builder.Property(entity => entity.Id)
                   .ValueGeneratedNever();
        });

        modelBuilder.Entity<UserRefreshToken>(builder =>
        {
            builder.ToTable(UserRefreshTokens);
            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.Id)
                   .ValueGeneratedNever();

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(entity => entity.OwnerId);

            builder.Property(entity => entity.OwnerId)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(entity => entity.ExpiryDateUtc)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.HasDefaultSchema(DomainContext.SchemeName);

        modelBuilder.Model.MarkColumnAsReadOnly(nameof(IDomainObject.Created));
    }
    #endregion
}