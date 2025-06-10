using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WriterID.Dev.Portal.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskStructureForNewWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WriterIds",
                table: "Tasks",
                newName: "SelectedWriters");

            migrationBuilder.AddColumn<string>(
                name: "QueryImagePath",
                table: "Tasks",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ResultsJson",
                table: "Tasks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QueryImagePath",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ResultsJson",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "SelectedWriters",
                table: "Tasks",
                newName: "WriterIds");
        }
    }
}
