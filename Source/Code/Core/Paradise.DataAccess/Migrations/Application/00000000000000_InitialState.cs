using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paradise.DataAccess.Migrations.Application;

/// <inheritdoc />
public partial class InitialState : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DataProtectionKeys",
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
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SubjectPlaceholderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SubjectPlaceholdersNumber = table.Column<int>(type: "int", nullable: false),
                IsBodyHtml = table.Column<bool>(type: "bit", nullable: false),
                Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                TemplateName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Culture = table.Column<int>(type: "int", nullable: true),
                TemplateText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PlaceholderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PlaceholdersNumber = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_EmailTemplates", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_EmailTemplates_TemplateName_Culture",
            table: "EmailTemplates",
            columns: new[] { "TemplateName", "Culture" },
            unique: true,
            filter: "[Culture] IS NOT NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DataProtectionKeys");

        migrationBuilder.DropTable(
            name: "EmailTemplates");
    }
}