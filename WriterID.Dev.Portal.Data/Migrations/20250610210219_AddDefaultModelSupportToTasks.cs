using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WriterID.Dev.Portal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultModelSupportToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ModelId",
                table: "Tasks",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<bool>(
                name: "UseDefaultModel",
                table: "Tasks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseDefaultModel",
                table: "Tasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModelId",
                table: "Tasks",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
