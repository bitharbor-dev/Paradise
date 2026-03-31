using Microsoft.EntityFrameworkCore.Migrations;

namespace Paradise.DataAccess.Database.Migrations.ApplicationLogic.Infrastructure.Domain;

/// <inheritdoc />
public partial class InitialState : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.EnsureSchema(
            name: "infrastructure");

        migrationBuilder.CreateTable(
            name: "DataProtectionKeys",
            schema: "infrastructure",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Xml = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_DataProtectionKeys", x => x.Id));

        migrationBuilder.CreateTable(
            name: "EmailTemplates",
            schema: "infrastructure",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SubjectPlaceholderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SubjectPlaceholdersNumber = table.Column<int>(type: "int", nullable: false),
                IsBodyHtml = table.Column<bool>(type: "bit", nullable: false),
                Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                TemplateName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Culture = table.Column<int>(type: "int", nullable: true),
                TemplateText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PlaceholderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PlaceholdersNumber = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_EmailTemplates", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_EmailTemplates_TemplateName_Culture",
            schema: "infrastructure",
            table: "EmailTemplates",
            columns: ["TemplateName", "Culture"],
            unique: true,
            filter: "[Culture] IS NOT NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropTable(
            name: "DataProtectionKeys",
            schema: "infrastructure");

        migrationBuilder.DropTable(
            name: "EmailTemplates",
            schema: "infrastructure");
    }
}