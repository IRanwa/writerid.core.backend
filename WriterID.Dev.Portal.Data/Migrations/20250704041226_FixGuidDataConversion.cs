using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WriterID.Dev.Portal.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixGuidDataConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clear existing data with corrupted GUID values
            // This is safe for development - in production, you would need a proper data migration
            migrationBuilder.Sql("DELETE FROM Tasks WHERE Id RLIKE '^[0-9]+$'");
            migrationBuilder.Sql("DELETE FROM Models WHERE Id RLIKE '^[0-9]+$'");
            migrationBuilder.Sql("DELETE FROM Datasets WHERE Id RLIKE '^[0-9]+$'");
            
            // Alternative approach: Clear all data to start fresh
            // migrationBuilder.Sql("DELETE FROM Tasks");
            // migrationBuilder.Sql("DELETE FROM Models");  
            // migrationBuilder.Sql("DELETE FROM Datasets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No rollback needed - data was already corrupted
            // This migration only cleans up corrupted data
        }
    }
}
