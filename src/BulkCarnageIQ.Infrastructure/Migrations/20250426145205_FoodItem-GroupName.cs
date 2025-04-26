using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkCarnageIQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FoodItemGroupName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "FoodItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "FoodItems");
        }
    }
}
