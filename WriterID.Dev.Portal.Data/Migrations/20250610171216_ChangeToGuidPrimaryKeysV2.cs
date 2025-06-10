using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WriterID.Dev.Portal.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToGuidPrimaryKeysV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Models_ModelId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Datasets_DatasetId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Models_Datasets_TrainingDatasetId",
                table: "Models");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_Tasks_ModelId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DatasetId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Models_TrainingDatasetId",
                table: "Models");

            // Change column types
            migrationBuilder.AlterColumn<Guid>(
                name: "ModelId",
                table: "Tasks",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "DatasetId",
                table: "Tasks",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Tasks",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<Guid>(
                name: "TrainingDatasetId",
                table: "Models",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Models",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Models",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Datasets",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            // Recreate indexes
            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DatasetId",
                table: "Tasks",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ModelId",
                table: "Tasks",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_TrainingDatasetId",
                table: "Models",
                column: "TrainingDatasetId");

            // Recreate foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Models_Datasets_TrainingDatasetId",
                table: "Models",
                column: "TrainingDatasetId",
                principalTable: "Datasets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Datasets_DatasetId",
                table: "Tasks",
                column: "DatasetId",
                principalTable: "Datasets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Models_ModelId",
                table: "Tasks",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Models_ModelId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Datasets_DatasetId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Models_Datasets_TrainingDatasetId",
                table: "Models");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_Tasks_ModelId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DatasetId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Models_TrainingDatasetId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Models");

            // Change column types back
            migrationBuilder.AlterColumn<int>(
                name: "ModelId",
                table: "Tasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "DatasetId",
                table: "Tasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "TrainingDatasetId",
                table: "Models",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Models",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Datasets",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            // Recreate indexes
            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DatasetId",
                table: "Tasks",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ModelId",
                table: "Tasks",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_TrainingDatasetId",
                table: "Models",
                column: "TrainingDatasetId");

            // Recreate foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Models_Datasets_TrainingDatasetId",
                table: "Models",
                column: "TrainingDatasetId",
                principalTable: "Datasets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Datasets_DatasetId",
                table: "Tasks",
                column: "DatasetId",
                principalTable: "Datasets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Models_ModelId",
                table: "Tasks",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
