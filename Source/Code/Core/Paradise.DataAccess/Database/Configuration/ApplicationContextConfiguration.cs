using Microsoft.EntityFrameworkCore;
using Paradise.ApplicationLogic.Domain.MessageTemplates;
using Paradise.DataAccess.Database.Configuration.Extensions;
using Paradise.DataAccess.Database.Converters;
using Paradise.Domain.Base;
using static Paradise.DataAccess.Database.Tables.ApplicationContextTables;

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
        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.ToTable(EmailTemplates);
            entity.HasKey(emailTemplate => emailTemplate.Id);

            entity.HasIndex(nameof(EmailTemplate.TemplateName),
                            nameof(EmailTemplate.Culture)).IsUnique();

            entity.Property(emailTemplate => emailTemplate.Culture)
                  .HasConversion<CultureInfoConverter>();
        });

        modelBuilder.HasDefaultSchema(ApplicationContext.SchemeName);

        modelBuilder.MarkColumnAsReadOnly(nameof(IDatabaseRecord.Created));
    }
    #endregion
}