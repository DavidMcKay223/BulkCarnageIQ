using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkCarnageIQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MealEntryMeasurementType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "MeasurementServings",
                table: "MealEntries",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeasurementType",
                table: "MealEntries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeasurementServings",
                table: "MealEntries");

            migrationBuilder.DropColumn(
                name: "MeasurementType",
                table: "MealEntries");
        }
    }
}
