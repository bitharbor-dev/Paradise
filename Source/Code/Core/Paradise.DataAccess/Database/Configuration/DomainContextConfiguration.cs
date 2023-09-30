using Microsoft.EntityFrameworkCore;
using Paradise.DataAccess.Database.Configuration.Extensions;
using Paradise.Domain.Base;
using Paradise.Domain.Users;
using static Paradise.DataAccess.Database.Tables.DomainContextTables;

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
        modelBuilder.Entity<UserRefreshToken>(entity =>
        {
            entity.ToTable(UserRefreshTokens).HasKey(nameof(UserRefreshToken.Id));

            entity.HasOne(userRefreshToken => userRefreshToken.Owner)
                  .WithMany(user => user.RefreshTokens)
                  .HasForeignKey(userRefreshToken => userRefreshToken.OwnerId);
        });

        modelBuilder.MarkColumnAsReadOnly(nameof(IDatabaseRecord.Created));
    }
    #endregion
}