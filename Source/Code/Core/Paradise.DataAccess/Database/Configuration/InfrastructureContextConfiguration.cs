using Microsoft.EntityFrameworkCore;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Database.Configuration.Converters;
using Paradise.DataAccess.Database.Configuration.Extensions;
using Paradise.Domain.Base;
using static Paradise.DataAccess.Database.Configuration.Tables.InfrastructureContextTables;

namespace Paradise.DataAccess.Database.Configuration;

/// <summary>
/// Contains <see cref="InfrastructureContext"/> entities configuration.
/// </summary>
internal static class InfrastructureContextConfiguration
{
    #region Public methods
    /// <summary>
    /// Configures <see cref="InfrastructureContext"/> entities.
    /// </summary>
    /// <param name="modelBuilder">
    /// The builder being used to construct the model for this context.
    /// </param>
    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailTemplate>(builder =>
        {
            builder.ToTable(EmailTemplates);
            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.Id)
                   .ValueGeneratedNever();

            builder.HasIndex(nameof(EmailTemplate.TemplateName),
                            nameof(EmailTemplate.Culture)).IsUnique();

            builder.Property(entity => entity.Culture)
                   .HasConversion<CultureInfoConverter>();
        });

        modelBuilder.HasDefaultSchema(InfrastructureContext.SchemeName);

        modelBuilder.Model.MarkColumnAsReadOnly(nameof(IDomainObject.Created));
    }
    #endregion
}